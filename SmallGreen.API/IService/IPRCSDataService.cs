using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.IService
{
    public interface IPRCSDataService
    {
        Task<ApiResponse<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime);

        Task<ApiResponse<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime);

        Task<ApiResponse<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId);
    }
}
