using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.IServices
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        Task<ApiResponse<UserDto>> Login(UserDto user);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        Task<ApiResponse> AddUser(UserDto user);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        Task<ApiResponse> ChangePassword(UserDto user);

        /// <summary>
        /// 启用或禁用用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>ApiResponse对象</returns>
        Task<ApiResponse> EnableOrDisableUser(UserDto user);

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ApiResponse<PageInfo<UserDto>>> GetList(int pageNumber, int pageSize);
    }
}
