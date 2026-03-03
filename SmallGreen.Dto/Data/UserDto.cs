namespace SmallGreen.Dto.Data
{
    /// <summary>
    /// 用户数据传输对象
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
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

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = "";

        public string StringEnabled { get { return IsEnabled ? "已启用" : "已禁用"; } }

    }

}
