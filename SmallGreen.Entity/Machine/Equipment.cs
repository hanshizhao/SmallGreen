using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Data;
using SmallGreen.Entity.Interface;
using SqlSugar;

namespace SmallGreen.Entity.Machine
{
    public class Equipment : IDomable
    {
        /// <summary>
        /// ID 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 所属子系统ID
        /// </summary>
        public long SubSystemId { get; set; }

        /// <summary>
        /// 机台名称
        /// </summary>
        public string CodeNumber { get; set; } = null!;

        /// <summary>
        /// 机台名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 工步号（用于存储过程参数）
        /// </summary>
        public int StepNo { get; set; } = 1;

        /// <summary>
        /// 当前生产订单的GUID（运行时设置，不持久化）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string uGuid { get; set; } = string.Empty;

        /// <summary>
        /// 配液缸组
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Bulk.EquipmentId))]
        public List<Bulk> ListBulk { get; set; } = null!;

        /// <summary>
        /// 翻页按钮
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<bool>? BtnPageChange { get; set; } = null!;

        /// <summary>
        /// 开始生产触发按钮
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<bool>? BtnStart { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? DataCurrentPage { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? DataTotalPage { get; set; }

        /// <summary>
        /// 当前机台生产订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? DataWorkColorID { get; set; }

        /// <summary>
        /// 当前机台生产订单的详细信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? DataWorkOrderInfoArray { get; set; }

        /// <summary>
        /// 订单完成类型
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? DataFinishedType { get; set; }


        /// <summary>
        /// 处方下发状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? DataFomulaQueryStatus { get; set; }

        /// <summary>
        /// 第1行订单的状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line1Status { get; set; }


        /// <summary>
        /// 第2行订单的状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line2Status { get; set; }

        /// <summary>
        /// 第3行订单的状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line3Status { get; set; }
        /// <summary>
        /// 第4行订单的状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line4Status { get; set; }
        /// <summary>
        /// 第5行订单的状态
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line5Status { get; set; }

        /// <summary>
        /// 第1行订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line1ColorID { get; set; }

        /// <summary>
        /// 第2行订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line2ColorID { get; set; }

        /// <summary>
        /// 第3行订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line3ColorID { get; set; }

        /// <summary>
        /// 第4行订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line4ColorID { get; set; }

        /// <summary>
        /// 第5行订单的颜色ID
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<ushort>? Line5ColorID { get; set; }

        /// <summary>
        /// 第1行订单的信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? Line1WrokInfoArray { get; set; }

        /// <summary>
        /// 第2行订单的信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? Line2WrokInfoArray { get; set; }

        /// <summary>
        /// 第3行订单的信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? Line3WrokInfoArray { get; set; }

        /// <summary>
        /// 第4行订单的信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? Line4WrokInfoArray { get; set; }

        /// <summary>
        /// 第5行订单的信息字符串
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<string>? Line5WrokInfoArray { get; set; }

        /// <summary>
        /// 订单完成触发变量
        /// </summary>
        [SugarColumn(IsJson = true, ColumnDataType = "varchar(200)")]
        public Dom<bool>? TriggerFinished { get; set; }

        public List<IDom> GetListDom()
        {
            var listDom = new List<IDom>();
            if (BtnPageChange != null) listDom.Add(BtnPageChange);
            if (BtnStart != null) listDom.Add(BtnStart);
            if (DataCurrentPage != null) listDom.Add(DataCurrentPage);
            if (DataTotalPage != null) listDom.Add(DataTotalPage);
            if (DataWorkColorID != null) listDom.Add(DataWorkColorID);
            if (DataWorkOrderInfoArray != null) listDom.Add(DataWorkOrderInfoArray);
            if (DataFinishedType != null) listDom.Add(DataFinishedType);
            if (DataFomulaQueryStatus != null) listDom.Add(DataFomulaQueryStatus);
            if (Line1Status != null) listDom.Add(Line1Status);
            if (Line2Status != null) listDom.Add(Line2Status);
            if (Line3Status != null) listDom.Add(Line3Status);
            if (Line4Status != null) listDom.Add(Line4Status);
            if (Line5Status != null) listDom.Add(Line5Status);
            if (Line1ColorID != null) listDom.Add(Line1ColorID);
            if (Line2ColorID != null) listDom.Add(Line2ColorID);
            if (Line3ColorID != null) listDom.Add(Line3ColorID);
            if (Line4ColorID != null) listDom.Add(Line4ColorID);
            if (Line5ColorID != null) listDom.Add(Line5ColorID);
            if (Line1WrokInfoArray != null) listDom.Add(Line1WrokInfoArray);
            if (Line2WrokInfoArray != null) listDom.Add(Line2WrokInfoArray);
            if (Line3WrokInfoArray != null) listDom.Add(Line3WrokInfoArray);
            if (Line4WrokInfoArray != null) listDom.Add(Line4WrokInfoArray);
            if (Line5WrokInfoArray != null) listDom.Add(Line5WrokInfoArray);
            if (TriggerFinished != null) listDom.Add(TriggerFinished);

            if (ListBulk != null && ListBulk.Count > 0)
            {
                foreach (var bulk in ListBulk)
                {
                    listDom.AddRange(bulk.GetListDom());
                }
            }

            return listDom;
        }
    }
}
