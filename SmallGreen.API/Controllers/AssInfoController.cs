using Microsoft.AspNetCore.Mvc;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;
using SmallGreen.Dto.Machine;

namespace SmallGreen.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AssInfoController : ControllerBase
    {
        private readonly IAssInfoService assInfoService;

        public AssInfoController(IAssInfoService _assInfoService)
        {
            assInfoService = _assInfoService;
        }

        [HttpGet]
        public async Task<ApiResponse<PageInfo<AssInfoDto>>> GetList()
        {
            return await assInfoService.GetList();
        }

        [HttpGet]
        public async Task<ApiResponse<List<MixedDetailDto>>> GetDetail(long prcsDataId)
        {
            return await assInfoService.GetDetail(prcsDataId);
        }

        #region 助剂桶管理接口

        /// <summary>
        /// 获取所有助剂桶列表（包含混合组分）
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<List<AssBucketDto>>> Buckets()
        {
            return await assInfoService.GetBuckets();
        }

        /// <summary>
        /// 更新助剂桶基本信息
        /// </summary>
        [HttpPut]
        [Route("Buckets/{id}")]
        public async Task<ApiResponse<AssBucketDto>> UpdateBucket(long id, [FromBody] UpdateAssBucketDto dto)
        {
            if (id != dto.Id)
            {
                return new ApiResponse<AssBucketDto>
                {
                    IsSuccess = false,
                    Message = "ID 不匹配"
                };
            }
            return await assInfoService.UpdateBucket(dto);
        }

        /// <summary>
        /// 新增混合组分
        /// </summary>
        [HttpPost]
        public async Task<ApiResponse<MixedComponentDto>> MixedComponent([FromBody] CreateMixedComponentDto dto)
        {
            return await assInfoService.CreateMixedComponent(dto);
        }

        /// <summary>
        /// 更新混合组分
        /// </summary>
        [HttpPut]
        [Route("MixedComponent/{id}")]
        public async Task<ApiResponse<MixedComponentDto>> UpdateMixedComponent(long id, [FromBody] UpdateMixedComponentDto dto)
        {
            if (id != dto.Id)
            {
                return new ApiResponse<MixedComponentDto>
                {
                    IsSuccess = false,
                    Message = "ID 不匹配"
                };
            }
            return await assInfoService.UpdateMixedComponent(dto);
        }

        /// <summary>
        /// 删除混合组分
        /// </summary>
        [HttpDelete]
        [Route("MixedComponent/{id}")]
        public async Task<ApiResponse<bool>> DeleteMixedComponent(long id)
        {
            return await assInfoService.DeleteMixedComponent(id);
        }

        #endregion
    }
}
