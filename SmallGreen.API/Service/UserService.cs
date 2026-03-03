using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;
using SmallGreen.Entity.Data;

namespace SmallGreen.API.Service
{
    public class UserService : IUserService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        public async Task<ApiResponse<UserDto>> Login(UserDto user)
        {
            var result = await User.Login(user.Username, user.Password);
            return result.IsSuccess ? ApiResponse<UserDto>.Success(result.Content) : ApiResponse<UserDto>.Fail(result.Message);
        }
    }
}
