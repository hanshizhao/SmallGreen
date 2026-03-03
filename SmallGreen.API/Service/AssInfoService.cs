using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.Service
{
    public class AssInfoService : IAssInfoService
    {
        public Task<ApiResponse<List<MixedDetailDto>>> GetDetail(long prcsDataId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PageInfo<AssInfoDto>>> GetList()
        {
            throw new NotImplementedException();
        }
    }
}
