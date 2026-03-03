using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.IService
{
    public interface IUserService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        Task<ApiResponse<UserDto>> Login(UserDto user);
    }
}
