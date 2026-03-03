using Microsoft.AspNetCore.Mvc;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// 登录
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        [HttpPost]
        public async Task<ApiResponse<UserDto>> Login(UserDto user)
        {
            return await _userService.Login(user);
        }
    }
}
