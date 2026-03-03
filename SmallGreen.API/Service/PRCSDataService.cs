using Microsoft.IdentityModel.Tokens;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;
using SmallGreen.Entity.Data;

namespace SmallGreen.API.Service
{
    public class PRCSDataService : IPRCSDataService
    {
        public async Task<ApiResponse<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime)
        {
            var days = (endDateTime - startDateTime).TotalDays;
            if (days < 0) return new ApiResponse<List<PRCSDataDto>>("起始日期必须小于结束日期");
            if (days > 31) return new ApiResponse<List<PRCSDataDto>>("查询日期范围不允许超过31天");

            var result = await PRCSData.GetExcelData(startDateTime, endDateTime);
            if (!result.IsSuccess) return new ApiResponse<List<PRCSDataDto>>("查询配料记录失败：" + result.Message);
            if (result.Content == null) return new ApiResponse<List<PRCSDataDto>>("查询配料记录失败：数据库返回正确的空结果");
            return new ApiResponse<List<PRCSDataDto>> { IsSuccess = true, Content = result.Content };
        }

        public async Task<ApiResponse<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime)
        {
            var days = (endDateTime - startDateTime).TotalDays;
            if (days < 0) return new ApiResponse<PageInfo<PRCSDataDto>>("起始日期必须小于结束日期");
            if (days > 31) return new ApiResponse<PageInfo<PRCSDataDto>>("查询日期范围不允许超过31天");

            var result = await PRCSData.GetListByPage(pageNumber, pageSize, startDateTime, endDateTime);
            if (!result.IsSuccess) return new ApiResponse<PageInfo<PRCSDataDto>>("查询配料记录失败：" + result.Message);
            if (result.Content == null) return new ApiResponse<PageInfo<PRCSDataDto>>("查询配料记录失败：数据库返回正确的空结果");
            return new ApiResponse<PageInfo<PRCSDataDto>> { IsSuccess = true, Content = result.Content };
        }

        public async Task<ApiResponse<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId)
        {
            var result = await PRCSData.GetDetail(prcsDataId);
            if (!result.IsSuccess) return new ApiResponse<List<PRCSDataDetailDto>>("查询配料步骤失败：" + result.Message);
            if (result.Content == null) return new ApiResponse<List<PRCSDataDetailDto>>("查询配料步骤失败：数据库返回正确的空结果");
            return new ApiResponse<List<PRCSDataDetailDto>> { IsSuccess = true, Content = result.Content };
        }
    }
}
