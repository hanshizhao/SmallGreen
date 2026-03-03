using SmallGreen.Dto.Machine;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Interface;
using SqlSugar;

namespace SmallGreen.Entity.Machine
{
    public class Bulk : IDomable
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string CodeNumber { get; set; } = null!;

        /// <summary>
        /// 所属机台Id
        /// </summary>
        public long EquipmentId { get; set; }

        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<float>? DataLevel { get; set; } = null!;

        /// <summary>
        /// 配液缸配液量
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<float>? DataPlanVolume { get; set; } = null!;

        /// <summary>
        /// 配方量字符串 
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? DataFomulaArray { get; set; } = null!;

        /// <summary>
        /// 配液完成标志位
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? TriggerComplete { get; set; } = null!;

        /// <summary>
        /// 助剂实际量字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? DataRealLitreArray { get; set; } = null!;

        /// <summary>
        /// 上次配液完成时间
        /// </summary>
        [SugarColumn(IsJson = true)]
        public DateTime LastCompleteTime { get; set; }

        public List<IDom> GetListDom()
        {
            var listDom = new List<IDom>();
            if (DataLevel != null) listDom.Add(DataLevel);
            if (DataPlanVolume != null) listDom.Add(DataPlanVolume);
            if (DataFomulaArray != null) listDom.Add(DataFomulaArray);
            if (TriggerComplete != null) listDom.Add(TriggerComplete);
            if (DataRealLitreArray != null) listDom.Add(DataRealLitreArray);
            return listDom;
        }

        public BulkDto ToDto()
        {
            return new BulkDto
            {
                Id = Id,
                CodeNumber = CodeNumber,
                Complete = TriggerComplete != null ? TriggerComplete.GetCurrentValue() : default,
                EquipmentId = EquipmentId,
                FomulaArray = DataFomulaArray != null ? DataFomulaArray.GetCurrentValue() : default,
                LastCompleteTime = LastCompleteTime,
                Level = DataLevel != null ? DataLevel.GetCurrentValue() : default,
                PlanVolume = DataPlanVolume != null ? DataPlanVolume.GetCurrentValue() : default,
                RealLitreArray = DataRealLitreArray != null ? DataRealLitreArray.GetCurrentValue() : default
            };
        }
    }
}
