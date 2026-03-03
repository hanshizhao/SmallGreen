using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        private readonly IUserService userService;
        public DialogCloseListener RequestClose { get; }

        private string message;
        private string userName;
        private string password;
        private bool isSave;

        public string Title { get; set; } = "aaaaaaa";

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public bool IsSave
        {
            get { return isSave; }
            set { SetProperty(ref isSave, value); }
        }


        public DelegateCommand CloseWindowCommand { get; }
        public DelegateCommand LoginCommand { get; }



        public LoginViewModel(IContainer container)
        {
            userService = container.Resolve<IUserService>();

            CloseWindowCommand = new DelegateCommand(CloseWindow);
            LoginCommand = new DelegateCommand(Login);

            // 初始化关闭监听器
            RequestClose = new DialogCloseListener();

            message = string.Empty;
            userName = string.Empty;
            password = string.Empty;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        private async void Login()
        {
            Message = string.Empty;

            if (string.IsNullOrEmpty(userName))
            {
                Message = "请输入用户名";
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Message = "请输入密码";
                return;
            }

            var user = new UserDto
            {
                Password = password,
                Username = userName
            };

            //if (user.Username == "admin" && user.Password == "123")
            //{
            //    GlobalParam.GetInstance().User = user;
            //    // 保存用户信息
            //    if (isSave)
            //    {
            //        Settings.Default.Username = userName;
            //        Settings.Default.Password = password;
            //        Settings.Default.IsSave = true;
            //        Settings.Default.Save();
            //    }
            //    else
            //    {
            //        Settings.Default.Username = "";
            //        Settings.Default.Password = "";
            //        Settings.Default.IsSave = false;
            //        Settings.Default.Save();
            //    }


            //    RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            //}
            //else
            //{
            //    Message = "登录失败：用户名或密码错误";
            //}



            var result = await userService.Login(user);
            if (!result.IsSuccess)
            {
                Message = result.Message;
                return;
            }
            var content = result.Content;
            if (content == null)
            {
                Message = "登录失败：服务器返回空对象";
                return;
            }
            GlobalParam.GetInstance().User = content;


            // 保存用户信息
            if (isSave)
            {
                Settings.Default.Username = userName;
                Settings.Default.Password = password;
                Settings.Default.IsSave = true;
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.Username = "";
                Settings.Default.Password = "";
                Settings.Default.IsSave = false;
                Settings.Default.Save();
            }
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void CloseWindow()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.No));
        }

        /// <summary>
        /// 是否可关闭
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// 窗口关闭时
        /// </summary>
        public void OnDialogClosed()
        {
            //RequestClose.Invoke(new DialogResult(ButtonResult.No));
        }

        /// <summary>
        /// 窗口打开时
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            UserName = Settings.Default.Username;
            Password = Settings.Default.Password;
            IsSave = Settings.Default.IsSave;
        }
    }
}
