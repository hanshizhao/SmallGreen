using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class UserPasswordChangeViewModel : DialogBaseViewModel
    {
        private readonly IUserService userService;
        private string oldPassword;
        private string newPassword;
        private string comfirmPassword;
        private string errorMessage;

        public string OldPassword
        {
            get { return oldPassword; }
            set { SetProperty(ref oldPassword, value); }
        }

        public string NewPassword
        {
            get { return newPassword; }
            set { SetProperty(ref newPassword, value); }
        }

        public string ComfirmPassword
        {
            get { return comfirmPassword; }
            set { SetProperty(ref comfirmPassword, value); }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public DelegateCommand SaveCommand { get; }

        public UserPasswordChangeViewModel(IContainer container) : base(container)
        {
            userService = container.Resolve<IUserService>();

            SaveCommand = new DelegateCommand(Save);

            oldPassword = "";
            newPassword = "";
            comfirmPassword = "";
            errorMessage = "";
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(oldPassword))
            {
                ErrorMessage = "请输入旧密码";
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                ErrorMessage = "请输入新密码";
                return;
            }

            if (!newPassword.Equals(comfirmPassword))
            {
                ErrorMessage = "两次密码不一致";
                return;
            }

            UserDto userDto = GlobalParam.GetInstance().User;
            userDto.Password = oldPassword;
            userDto.NewPassword = comfirmPassword;

            var result = await userService.ChangePassword(userDto);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return;
            }

            await ShowSuccessMessage();
        }
    }
}
