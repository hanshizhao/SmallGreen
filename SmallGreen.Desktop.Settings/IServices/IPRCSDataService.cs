using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.IServices
{
    public interface IPRCSDataService
    {
        /// <summary>
        /// 获取抽料记录
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        Task<ApiResponse<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime);

        /// <summary>
        /// 获取导出到Excel的数据
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        Task<ApiResponse<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime);

        Task<ApiResponse<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId);
        
    }
}
