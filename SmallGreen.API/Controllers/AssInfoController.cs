using Microsoft.AspNetCore.Mvc;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

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
    }
}
