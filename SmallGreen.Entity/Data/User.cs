using SmallGreen.Dto.Data;
using SmallGreen.Entity.Basic;
using SqlSugar;

namespace SmallGreen.Entity.Data
{
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegistrationTime { get; set; }

        public UserDto ToDto()
        {
            return new UserDto
            {
                Id = Id,
                Username = Username,
                Password = "******",
                IsEnabled = IsEnabled,
                RegistrationTime = RegistrationTime
            };
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>OperateResult对象</returns>
        public static async Task<OperateResult<UserDto>> Login(string username, string password)
        {
            try
            {
                var user = await new Repository<User>().AsQueryable()
                    .Where(u => u.Username == username && u.Password == password && u.IsEnabled)
                    .FirstAsync();

                if (user == null)
                {
                    return new OperateResult<UserDto>("用户名或密码错误");
                }

                return new OperateResult<UserDto> { IsSuccess = true, Content = user.ToDto() };
            }
            catch (Exception ex)
            {
                return new OperateResult<UserDto>(ex.Message);
            }
        }

    }
}
