using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;

namespace MiHoyoGameGachaRecords
{
    internal class MessageBox
    {
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
        private static XamlRoot? GetRoot()
        {
            return App.MainWindowInstance?.Content.XamlRoot;
        }

        public static async Task GamePathIsNull(string game)
        {
            var root = GetRoot();
            if (root == null) return;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/NotPathTitle"),
                Content = string.Format(loader.GetString("SettingControl/SelectGame"), game),
                CloseButtonText = loader.GetString("MessageBox/Primary"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }

        //public static async Task FilePathIsNull()
        //{
        //    var root = GetRoot();
        //    if (root == null) return;
        //    ContentDialog dialog = new ContentDialog
        //    {
        //        Title = "未选择文件",
        //        Content = "请重新选择需要导入的文件。",
        //        CloseButtonText = loader.GetString("MessageBox/Primary"),
        //        XamlRoot = root,
        //        DefaultButton = ContentDialogButton.Close
        //    };
        //    await dialog.ShowAsync();
        //}

        public static async Task<bool> ShowLanguageRestartDialog()
        {
            var root = GetRoot();
            if (root == null) return false;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/langRestartTitle"),
                Content = loader.GetString("MessageBox/langRestartContent"),
                PrimaryButtonText = loader.GetString("MessageBox/Primary"),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = root
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return true;
            else return false;
        }

        public static async Task OutPutFinish(string path)
        {
            var root = GetRoot();
            if (root == null) return;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/info"),
                Content = loader.GetString("MessageBox/copyFinish") + $"{path}",
                CloseButtonText = loader.GetString("MessageBox/Primary"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }

        public static async Task Error(string errorInfo)
        {
            var root = GetRoot();
            if (root == null) return;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/Error"),
                Content = errorInfo,
                CloseButtonText = loader.GetString("MessageBox/Primary"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }

        public static async Task Info(string infoString)
        {
            var root = GetRoot();
            if (root == null) return;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/info"),
                Content = infoString,
                CloseButtonText = loader.GetString("MessageBox/Primary"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }

        public static async Task Warn(string warnString)
        {
            var root = GetRoot();
            if (root == null) return;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/warn"),
                Content = warnString,
                CloseButtonText = loader.GetString("MessageBox/Primary"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }

        public static async Task<bool> InfoYesOrNo(string infoString)
        {
            var root = GetRoot();
            if (root == null) return false;
            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("MessageBox/info"),
                Content = infoString,
                CloseButtonText = loader.GetString("MessageBox/No"),
                PrimaryButtonText = loader.GetString("MessageBox/Yes"),
                XamlRoot = root,
                DefaultButton = ContentDialogButton.Primary
            };
            var result =  await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary) return true;
            else return false;
        }
    }
}
