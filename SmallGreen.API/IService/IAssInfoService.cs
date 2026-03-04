using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;
using SmallGreen.Dto.Machine;

namespace SmallGreen.API.IService
{
    public interface IAssInfoService
    {
        Task<ApiResponse<PageInfo<AssInfoDto>>> GetList();

        Task<ApiResponse<List<MixedDetailDto>>> GetDetail(long prcsDataId);

        /// <summary>
        /// 获取所有助剂桶列表（包含混合组分）
        /// </summary>
        Task<ApiResponse<List<AssBucketDto>>> GetBuckets();

        /// <summary>
        /// 根据ID获取助剂桶详情
        /// </summary>
        Task<ApiResponse<AssBucketDto>> GetBucketById(long id);

        /// <summary>
        /// 更新助剂桶基本信息
        /// </summary>
        Task<ApiResponse<AssBucketDto>> UpdateBucket(UpdateAssBucketDto dto);

        /// <summary>
        /// 新增混合组分
        /// </summary>
        Task<ApiResponse<MixedComponentDto>> CreateMixedComponent(CreateMixedComponentDto dto);

        /// <summary>
        /// 更新混合组分
        /// </summary>
        Task<ApiResponse<MixedComponentDto>> UpdateMixedComponent(UpdateMixedComponentDto dto);

        /// <summary>
        /// 删除混合组分
        /// </summary>
        Task<ApiResponse<bool>> DeleteMixedComponent(long id);
    }
}
