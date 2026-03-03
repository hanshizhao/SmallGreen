using SmallGreen.Dto.Data;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Machine;
using SqlSugar;

namespace SmallGreen.Entity.Data
{
    /// <summary>
    /// 配液完成时，记录的配液数据(主表)
    /// </summary>
    public class PRCSData
    {
        /// <summary>
        /// ID 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 俺也不知道是啥
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(200)")]
        public string? CardNo { get; set; }

        /// <summary>
        /// 机台Id
        /// </summary>
        public long EquipmentId { get; set; }

        /// <summary>
        /// 机台编号
        /// </summary>
        public string? EquipmentCodeNumber { get; set; }

        /// <summary>
        /// 机台名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 对应的机台
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(EquipmentId))]
        public Equipment? Equipment { get; set; }

        /// <summary>
        /// 配液缸Id
        /// </summary>
        public long BulkId { get; set; }

        /// <summary>
        /// 配液缸编号
        /// </summary>
        public string? BulkCodeNumber { get; set; }

        /// <summary>
        /// 对应的配液缸
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(BulkId))]
        public Bulk? Bulk { get; set; }

        /// <summary>
        /// 计划配液体积   500
        /// </summary>
        public float PlanVolume { get; set; }

        /// <summary>
        /// 实际配液体积
        /// </summary>
        public float ActualVolume { get; set; }

        /// <summary>
        /// 助剂消耗详情
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(PRCSDataDetail.ParentId))]
        public List<PRCSDataDetail> ListDetail { get; set; } = null!;

        /// <summary>
        /// 是否是换单
        /// </summary>
        public bool IsChange { get; set; } = false;

        /// <summary>
        /// 这是干嘛？以后再说吧
        /// </summary>
        //public bool IsLeft  { get; set; }

        /// <summary>
        /// 配液完成时间
        /// </summary>
        public DateTime CompletedDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 实体对象转Dto
        /// </summary>
        /// <returns></returns>
        public PRCSDataDto ToDto()
        {
            var listDetailDto = new List<PRCSDataDetailDto>();


            if (ListDetail != null)
            {
                foreach (var detail in ListDetail)
                {
                    listDetailDto.Add(detail.ToDto());
                }
            }

            return new PRCSDataDto
            {
                Id = Id,
                ActualVolume = ActualVolume,
                Bulk = null,
                ListDetail = listDetailDto,
                CompletedDateTime = CompletedDateTime,
                BulkCodeNumber = BulkCodeNumber,
                BulkId = BulkId,
                CardNo = CardNo,
                Equipment = null,
                EquipmentCodeNumber = EquipmentCodeNumber,
                EquipmentId = EquipmentId,
                EquipmentName = EquipmentName,
                IsChange = IsChange,
                PlanVolume = PlanVolume
            };
        }

        /// <summary>
        /// 获取导出到Excel的数据
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public static async Task<OperateResult<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                var list = await new Repository<PRCSData>().AsQueryable()
                    .Includes(co => co.ListDetail.OrderBy(item => item.AssSequence).ToList())
                    .Where(it => it.CompletedDateTime > startDateTime && it.CompletedDateTime < endDateTime)
                    .OrderBy(it => it.CompletedDateTime, OrderByType.Desc)
                    .ToListAsync();

                var listDto = new List<PRCSDataDto>();
                foreach (var item in list)
                {
                    listDto.Add(item.ToDto());
                }

                return new OperateResult<List<PRCSDataDto>> { IsSuccess = true, Content = listDto };

            }
            catch (Exception ex)
            {
                return new OperateResult<List<PRCSDataDto>>(ex.Message);
            }
        }


        public static async Task<OperateResult<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                RefAsync<int> totalCount = 0;
                var list = await new Repository<PRCSData>().AsQueryable()
                    //.Includes(co => co.ListCallerOrderItem.OrderBy(item => item.Sequence).ToList())
                    .Where(it => it.CompletedDateTime > startDateTime && it.CompletedDateTime < endDateTime)
                    .OrderBy(it => it.CompletedDateTime, OrderByType.Desc)
                    .ToPageListAsync(pageNumber, pageSize, totalCount);

                var listDto = new List<PRCSDataDto>();

                foreach (var item in list)
                {
                    listDto.Add(item.ToDto());
                }

                var pageInfo = new PageInfo<PRCSDataDto>
                {
                    Items = listDto,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItemCount = totalCount
                };

                return new OperateResult<PageInfo<PRCSDataDto>> { IsSuccess = true, Content = pageInfo };

            }
            catch (Exception ex)
            {
                return new OperateResult<PageInfo<PRCSDataDto>>(ex.Message);
            }
        }

        public static async Task<OperateResult<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId)
        {
            try
            {
                var list = await new Repository<PRCSDataDetail>().AsQueryable()
                    .Where(it => it.ParentId == prcsDataId)
                    .OrderBy(it => it.AssSequence, OrderByType.Asc)
                    .ToListAsync();

                var listDto = new List<PRCSDataDetailDto>();
                foreach (var item in list)
                {
                    listDto.Add(item.ToDto());
                }

                return new OperateResult<List<PRCSDataDetailDto>> { IsSuccess = true, Content = listDto };
            }
            catch (Exception ex)
            {
                return new OperateResult<List<PRCSDataDetailDto>>(ex.Message);
            }
        }
    }
}
