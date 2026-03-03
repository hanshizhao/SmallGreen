namespace SmallGreen.Desktop.Settings.Common
{
    public interface IDrawerHostAware
    {
        /// <summary>
        /// 弹窗打开过程中执行
        /// </summary>
        /// <param name="parameters"></param>
        void OnDialogOpend(IDialogParameters parameters);
    }
}
