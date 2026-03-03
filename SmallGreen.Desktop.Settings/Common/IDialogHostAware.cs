namespace SmallGreen.Desktop.Settings.Common
{
    public interface IDialogHostAware
    {
        /// <summary>
        /// DialogHost名称
        /// </summary>
        string HostName { get; set; }

        /// <summary>
        /// 弹窗打开过程中执行
        /// </summary>
        /// <param name="parameters"></param>
        void OnDialogOpend(IDialogParameters parameters);

        /// <summary>
        /// 关闭窗口
        /// </summary>
        DelegateCommand CloseWindowCommand { get; }

    }
}
