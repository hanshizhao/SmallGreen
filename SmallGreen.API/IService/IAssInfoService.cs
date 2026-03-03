using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.IService
{
    public interface IAssInfoService
    {
        Task<ApiResponse<PageInfo<AssInfoDto>>> GetList();

        Task<ApiResponse<List<MixedDetailDto>>> GetDetail(long prcsDataId);
    }
}
