using RestSharp;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.Services
{
    public class PRCSDataService : IPRCSDataService
    {
        private readonly HttpRestClient client;

        public PRCSDataService(HttpRestClient client)
        {
            this.client = client;
        }

        public async Task<ApiResponse<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime)
        {
            BaseRequest request = new()
            {
                Method = Method.Get,
                Route = $"PRCSData/GetExcelData?startDateTime={startDateTime}&endDateTime={endDateTime}",
            };
            return await client.ExecuteAsync<List<PRCSDataDto>>(request);
        }

        public async Task<ApiResponse<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime)
        {
            BaseRequest request = new()
            {
                Method = Method.Get,
                Route = $"PRCSData/GetListByPage?pageNumber={pageNumber}&pageSize={pageSize}&startDateTime={startDateTime}&endDateTime={endDateTime}",
            };
            return await client.ExecuteAsync<PageInfo<PRCSDataDto>>(request);
        }

        public async Task<ApiResponse<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId)
        {
            BaseRequest request = new()
            {
                Method = Method.Get,
                Route = $"PRCSData/GetDetail?prcsDataId={prcsDataId}",
            };
            return await client.ExecuteAsync<List<PRCSDataDetailDto>>(request);
        }
    }
}
