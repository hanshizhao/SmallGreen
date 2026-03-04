using SmallGreen.Common;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Data;
using SmallGreen.Entity.Interface;
using SqlSugar;
using Serilog;

namespace SmallGreen.Entity.Machine
{
    public class SubSystem : ISubSystem
    {
        /// <summary>
        /// ID 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public SubSystemName SubSystemName { get; set; }

        /// <summary>
        /// PLC ID
        /// </summary>
        public long PlcID { get; set; }

        /// <summary>
        /// PLC
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(PlcID))]
        public SiemensPLC PLC { get; set; } = null!;

        /// <summary>
        /// 助剂系数字符串   体积  --  质量
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? DataAssCoeArray { get; set; }

        /// <summary>
        /// 机台集合
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Equipment.SubSystemId))]
        public List<Equipment> ListEquipment { get; set; } = [];

        /// <summary>
        /// 子系统下所有的PLC点位
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<IDom>? ListDom { get; set; }


        public async Task<OperateResult> CheckRuntime()
        {
            var result = await PLC.RefreshDataPool();
            if (!result.IsSuccess) return result;

            ListDom ??= GetListDom();

            PLC.RefreshDomValue(ListDom);

            return new OperateResult { IsSuccess = true };
        }

        public async Task<OperateResult> CheckRuntimeOnlyTrigger()
        {
            var resultConnceted = await PLC.IsConnected();
            if (!resultConnceted.IsSuccess) return resultConnceted;

            var listDomTrigger = new List<IDom>();
            foreach (var equip in ListEquipment)
            {
                if (equip.BtnPageChange != null) listDomTrigger.Add(equip.BtnPageChange);
                if (equip.BtnStart != null) listDomTrigger.Add(equip.BtnStart);
                if (equip.TriggerFinished != null) listDomTrigger.Add(equip.TriggerFinished);

                foreach (var bulk in equip.ListBulk)
                {
                    // 配液缸配液完成标识
                    if (bulk.TriggerComplete != null) listDomTrigger.Add(bulk.TriggerComplete);
                }
            }

            if (listDomTrigger.Count == 0) return new OperateResult { IsSuccess = true };

            var result = await PLC.ReadMultipleVars(listDomTrigger);
            if (!result.IsSuccess) return result;

            return new OperateResult { IsSuccess = true };
        }

        private List<IDom> GetListDom()
        {
            var list = new List<IDom>();
            if (DataAssCoeArray != null) list.Add(DataAssCoeArray);
            if (ListEquipment != null && ListEquipment.Count > 0)
            {
                foreach (var equipment in ListEquipment)
                {
                    list.AddRange(equipment.GetListDom());
                }
            }
            return list;
        }

        public async Task<OperateResult<PRCSData>> SavePRCSData(Equipment equipment, Bulk bulk)
        {
            // 先将触发点位由1重置为0 防止重复向数据库写入消耗量信息
            if (bulk.TriggerComplete != null)
            {
                bulk.TriggerComplete.NewValue = 0;
                var r = await PLC.Write(bulk.TriggerComplete);
                if (!r.IsSuccess)
                {
                    return new OperateResult<PRCSData>
                    {
                        IsSuccess = false,
                        Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，重置触发点位失败:{r.Message}"
                    };
                }
            }

            // 查询助剂信息
            var resultAssList = await AssInfo.GetAssInfo();
            if (!resultAssList.IsSuccess)
            {
                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，从数据库中查询助剂信息失败:{resultAssList.Message}"
                };
            }
            // 查询出来的助剂信息
            var listAssInfo = resultAssList.Content;
            if (listAssInfo == null || listAssInfo.Count == 0)
            {
                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，从数据库中查询到的助剂信息为空"
                };
            }

            var listDom = new List<IDom>();
            if (bulk.DataLevel != null) listDom.Add(bulk.DataLevel); // 实际配液体积
            if (bulk.DataPlanVolume != null) listDom.Add(bulk.DataPlanVolume); // 计划配液体积
            if (bulk.DataFomulaArray != null) listDom.Add(bulk.DataFomulaArray); // 配方量？
            if (bulk.DataRealLitreArray != null) listDom.Add(bulk.DataRealLitreArray); // 实际使用体积？

            var result = await PLC.ReadMultipleVars(listDom);

            if (!result.IsSuccess)
            {
                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，读取PLC点位出现异常:{result.Message}"
                };
            }

            // 处理数据
            // 处理实际使用体积
            var realLitreArr = StrToFloat(bulk.DataRealLitreArray?.GetCurrentValue() ?? "");
            var formularArr = StrToFloat(bulk.DataFomulaArray?.GetCurrentValue() ?? "");

            // 如果实际体积数组的个数不等于配方量数组的个数，那么就返回错误
            if (realLitreArr.Length != formularArr.Length)
            {
                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，助剂实际量字符串{realLitreArr.Length} !== 配方量字符串{formularArr.Length}"
                };
            }

            var prcsData = new PRCSData
            {
                CardNo = "手动模式",
                EquipmentId = equipment.Id,
                EquipmentCodeNumber = equipment.CodeNumber,
                EquipmentName = equipment.Name,
                BulkId = bulk.Id,
                BulkCodeNumber = bulk.CodeNumber,
                ActualVolume = bulk.DataLevel?.GetCurrentValue() ?? 0f,
                PlanVolume = bulk.DataPlanVolume?.GetCurrentValue() ?? 0f
            };

            var listDetail = new List<PRCSDataDetail>();

            for (int i = 0; i < realLitreArr.Length; i++)
            {
                var realLitre = realLitreArr[i];
                if (realLitre == 0) continue;

                var formular = formularArr[i];

                // 固色系统(GS1)助剂 Sequence 从 13 开始，前处理从 1 开始
                var sequenceOffset = this.SubSystemName == SubSystemName.GS1 ? 12 : 0;
                var sequence = i + 1 + sequenceOffset;

                // 对应的助剂
                var ass = listAssInfo.Find(it => it.Sequence == sequence);
                if (ass == null)
                {
                    return new OperateResult<PRCSData>
                    {
                        IsSuccess = false,
                        Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，找不到对应顺序的助剂,Sequence-{sequence}"
                    };
                }

                // 检查是否是混合助剂
                if (ass.IsMixed)
                {
                    // 检查是否正确配置了混合助剂信息
                    var listMixedDetail = ass.ListMixedDetail;
                    if (listMixedDetail == null || listMixedDetail.Count == 0)
                    {
                        return new OperateResult<PRCSData>
                        {
                            IsSuccess = false,
                            Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，管道号<{sequence}>混合助剂配置不正确:混合助剂信息不存在"
                        };
                    }

                    // 检查混合助剂的比例是否设置正确
                    var sumRatio = 0d;

                    foreach (var it in listMixedDetail)
                    {
                        var ratio = it.Ratio;
                        if (ratio <= 0)
                        {
                            return new OperateResult<PRCSData>
                            {
                                IsSuccess = false,
                                Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，管道号<{sequence}>混合助剂配置不正确:混合助剂<{it.Name}>设置的混合比例<{ratio}> <= 0"
                            };
                        }
                        sumRatio += ratio;
                    }

                    foreach (var it in listMixedDetail)
                    {
                        var curRatio = it.Ratio / sumRatio;  // 当前助剂的混合占比

                        var assKg = (realLitre * it.Concentration * curRatio) / 1000d; // 混合助剂的实际质量
                        var effectiveAssKg = (realLitre * it.EffectiveConcentration * curRatio) / 1000d; // 财务消耗量
                        var planKg = (prcsData.PlanVolume * formular * curRatio) / 1000d; // 混合助剂的计划质量
                        var planVolumeWithWater = (prcsData.PlanVolume * formular * curRatio) / it.Concentration; // 混合助剂的计划体积
                        var p = new PRCSDataDetail
                        {
                            AssId = it.Id,
                            AssCodeNumber = it.CodeNumber,
                            AssName = it.Name,
                            AssGl = formular,
                            AssSequence = sequence,
                            AssKg = assKg,
                            EffectiveAssKg = effectiveAssKg,
                            PlanKg = planKg,
                            GramsPerLiter = it.Concentration,
                            EffectiveGramsPerLiter = it.EffectiveConcentration,
                            PlanVolumeWithWater = planVolumeWithWater,
                            RealVolumeWithWater = realLitre
                        };

                        listDetail.Add(p);
                    }
                }
                else
                {
                    // 非混合助剂，直接保存

                    /**
                     温馨提示:
                        1. ass.Concentration属性，表示1升助剂中有多少克的助剂质量;
                        2. 所以助剂用量（kg）= realLitre （流量计读取到的体积） * Concentration（1升溶液中含有的助剂质量） / 1000 （换算成公斤）
                        3. AssKg = (realLitre * ass.Concentration) / 1000d
                     */

                    var p = new PRCSDataDetail
                    {
                        AssGl = formular,
                        AssId = ass.Id,
                        AssSequence = sequence,
                        AssKg = (realLitre * ass.Concentration) / 1000d,
                        EffectiveAssKg = (realLitre * ass.EffectiveConcentration) / 1000d,
                        AssName = ass.Name,
                        AssCodeNumber = ass.CodeNumber,
                        PlanKg = (prcsData.PlanVolume * formular) / 1000d,
                        GramsPerLiter = ass.Concentration,
                        EffectiveGramsPerLiter = ass.EffectiveConcentration,
                        PlanVolumeWithWater = (prcsData.PlanVolume * formular) / ass.Concentration,
                        RealVolumeWithWater = realLitre
                    };

                    listDetail.Add(p);
                }


            }

            if (listDetail.Count < 1)
            {
                // 诊断日志：记录原始数据以分析问题原因
                var realLitreRaw = bulk.DataRealLitreArray?.GetCurrentValue() ?? "null";
                var formulaRaw = bulk.DataFomulaArray?.GetCurrentValue() ?? "null";
                var realLitreArrPreview = realLitreArr.Length > 0
                    ? string.Join(", ", realLitreArr.Take(5).Select(v => v.ToString("F2"))) + (realLitreArr.Length > 5 ? "..." : "")
                    : "(空数组)";
                var formularArrPreview = formularArr.Length > 0
                    ? string.Join(", ", formularArr.Take(5).Select(v => v.ToString("F2"))) + (formularArr.Length > 5 ? "..." : "")
                    : "(空数组)";

                Log.Warning(
                    "[诊断日志] {EquipmentName}-{BulkCodeNumber} 检测不到助剂消耗量 | " +
                    "DataRealLitreArray原始值: [{RealLitreRaw}] | " +
                    "DataFomulaArray原始值: [{FormulaRaw}] | " +
                    "realLitreArr解析结果: [{RealLitreArr}] (长度:{RealLen}) | " +
                    "formularArr解析结果: [{FormulaArr}] (长度:{FormulaLen})",
                    equipment.Name,
                    bulk.CodeNumber,
                    realLitreRaw,
                    formulaRaw,
                    realLitreArrPreview,
                    realLitreArr.Length,
                    formularArrPreview,
                    formularArr.Length
                );

                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，检测不到助剂消耗量"
                };
            }

            prcsData.ListDetail = listDetail;

            var n = await new Repository<PRCSData>().Context.InsertNav(prcsData)
                .Include(it => it.ListDetail)
                .ExecuteCommandAsync();

            if (!n)
            {
                return new OperateResult<PRCSData>
                {
                    IsSuccess = false,
                    Message = $"{equipment.Name}-{bulk.CodeNumber} 配液完成时，保存数据到数据库失败"
                };
            }

            return new OperateResult<PRCSData> { IsSuccess = true };
        }

        private static float[] StrToFloat(string strArr)
        {
            if (string.IsNullOrEmpty(strArr)) return [];

            // 使用 RemoveEmptyEntries 避免最后一位是 'A' 时产生空元素
            var parts = strArr.Split(['A'], StringSplitOptions.RemoveEmptyEntries);
            var result = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                // 过滤掉 PLC 字符串中的控制字符（如 0x18），只保留数字、小数点、负号
                var cleaned = new string(part.Where(c =>
                    char.IsDigit(c) || c == '.' || c == '-' || c == '+').ToArray());

                if (cleaned.Length > 0 &&
                    float.TryParse(cleaned, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    result[i] = value;
                }
                else
                {
                    result[i] = 0f;
                }
            }

            return result;
        }


    }
}
