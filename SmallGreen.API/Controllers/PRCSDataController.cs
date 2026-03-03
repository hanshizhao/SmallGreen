using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PRCSDataController : ControllerBase
    {
        private readonly IPRCSDataService prcsDataService;

        public PRCSDataController(IPRCSDataService _prcsDataService)
        {
            prcsDataService = _prcsDataService;
        }

        /// <summary>
        /// 获取小绿记录
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<PageInfo<PRCSDataDto>>> GetListByPage(int pageNumber, int pageSize, DateTime startDateTime, DateTime endDateTime)
        {
            return await prcsDataService.GetListByPage(pageNumber, pageSize, startDateTime, endDateTime);
        }

        /// <summary>
        /// 获取导出到Excel的数据
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<PRCSDataDto>>> GetExcelData(DateTime startDateTime, DateTime endDateTime)
        {
            return await prcsDataService.GetExcelData(startDateTime, endDateTime);
        }

        [HttpGet]
        public async Task<ApiResponse<List<PRCSDataDetailDto>>> GetDetail(long prcsDataId)
        {
            return await prcsDataService.GetDetail(prcsDataId);
        }
    }
}
