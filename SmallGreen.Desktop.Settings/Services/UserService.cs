using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.Services
{
    public class UserService : IUserService
    {
        private readonly HttpRestClient client;

        public UserService(HttpRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        public async Task<ApiResponse> AddUser(UserDto user)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Post,
                Route = $"User/AddUser",
                Parameter = user
            };
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        public async Task<ApiResponse> ChangePassword(UserDto user)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Put,
                Route = $"User/ChangePassword",
                Parameter = user
            };
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// 启用或禁用用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        public async Task<ApiResponse> EnableOrDisableUser(UserDto user)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Put,
                Route = $"User/EnableOrDisableUser",
                Parameter = user
            };
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        public async Task<ApiResponse<UserDto>> Login(UserDto user)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Post,
                Route = $"User/Login",
                Parameter = user
            };
            return await client.ExecuteAsync<UserDto>(request);
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ApiResponse<PageInfo<UserDto>>> GetList(int pageNumber, int pageSize)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Get,
                Route = $"User/GetList?pageNumber={pageNumber}&pageSize={pageSize}",
            };
            return await client.ExecuteAsync<PageInfo<UserDto>>(request);
        }
    }
}
