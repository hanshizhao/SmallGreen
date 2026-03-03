using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmallGreen.Desktop.Settings.Extensions
{
    public static class DialogExtensions
    {
        /// <summary>
        /// 询问窗口
        /// </summary>
        /// <param name="dialogHost">指定的DialogHost会话主机</param>
        /// <param name="title">标题</param>
        /// <param name="message">询问内容</param>
        /// <param name="dialogHostName">会话主机名称(唯一)</param>
        /// <returns></returns>
        public static async Task<IDialogResult> Question(this IDialogHostService dialogHost,
            string title, string message, string dialogHostName = "Root"
            )
        {
            DialogParameters param = new DialogParameters();
            param.Add("Title", title);
            param.Add("Message", message);
            param.Add("dialogHostName", dialogHostName);
            var dialogResult = await dialogHost.ShowDialog("ConfirmOperateView", param, dialogHostName);
            return dialogResult;
        }

        public static async Task<IDialogResult> Confirm(this IDialogHostService dialogHost, string title, string content, string dialogHostName = "Root")
        {
            DialogParameters param = new()
            {
                { "Title", title },
                { "Content", content },
                { "dialogHostName", dialogHostName }
            };
            var dialogResult = await dialogHost.ShowDialog("ConfirmOperateView", param, dialogHostName);
            return dialogResult;
        }

        /// <summary>
        /// 打开窗体是否需要等待
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="isLoading"></param>
        public static void ShowLoading(this IEventAggregator aggregator, bool isLoading)
        {
            aggregator.GetEvent<LoadingEvent>().Publish(isLoading);
        }

        /// <summary>
        /// 注册等待
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void RegisterLoading(this IEventAggregator aggregator, Action<bool> action)
        {
            aggregator.GetEvent<LoadingEvent>().Subscribe(action);
        }

        /// <summary>
        /// 注册底部抽屉滑出事件
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void RegisterShowDrawerBottomEvent(this IEventAggregator aggregator, Action<UserControl> action)
        {
            aggregator.GetEvent<ShowBottomDrawerEvent>().Subscribe(action);
        }

        public static void ShowBottomDrawer(this IEventAggregator aggregator, UserControl control)
        {
            aggregator.GetEvent<ShowBottomDrawerEvent>().Publish(control);
        }



    }
}
