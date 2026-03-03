using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.Services
{
    public class AssBucketService : IAssBucketService
    {
        private readonly HttpRestClient client;

        public AssBucketService(HttpRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// 获取所有助剂桶列表
        /// </summary>
        public async Task<ApiResponse<List<AssBucketDto>>> GetBuckets()
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Get,
                Route = "AssInfo/Buckets"
            };
            return await client.ExecuteAsync<List<AssBucketDto>>(request);
        }

        /// <summary>
        /// 更新助剂桶基本信息
        /// </summary>
        public async Task<ApiResponse<AssBucketDto>> UpdateBucket(UpdateAssBucketDto dto)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Put,
                Route = $"AssInfo/Buckets/{dto.Id}",
                Parameter = dto
            };
            return await client.ExecuteAsync<AssBucketDto>(request);
        }

        /// <summary>
        /// 新增混合组分
        /// </summary>
        public async Task<ApiResponse<MixedComponentDto>> CreateMixedComponent(CreateMixedComponentDto dto)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Post,
                Route = "AssInfo/MixedComponent",
                Parameter = dto
            };
            return await client.ExecuteAsync<MixedComponentDto>(request);
        }

        /// <summary>
        /// 更新混合组分
        /// </summary>
        public async Task<ApiResponse<MixedComponentDto>> UpdateMixedComponent(UpdateMixedComponentDto dto)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Put,
                Route = $"AssInfo/MixedComponent/{dto.Id}",
                Parameter = dto
            };
            return await client.ExecuteAsync<MixedComponentDto>(request);
        }

        /// <summary>
        /// 删除混合组分
        /// </summary>
        public async Task<ApiResponse> DeleteMixedComponent(long id)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Delete,
                Route = $"AssInfo/MixedComponent/{id}"
            };
            return await client.ExecuteAsync(request);
        }
    }
}
