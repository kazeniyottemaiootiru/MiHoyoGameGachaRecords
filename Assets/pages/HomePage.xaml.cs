using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords.Assets.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            var resourcesLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
        }

        public record HomeCard(string Title, string Description, string PageKey);
        // log函数
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bt && bt.Tag is string tag && tag != null)
            {
                    App.MainWindowInstance?.Natigate(tag);
                    MenuViewIndex(tag);
            }
        }

        private static void MenuViewIndex(string key)
        {
            // 尝试在主窗口的视觉树中找到NavigationView并标记对应项为选中
            var root = App.MainWindowInstance?.Content as DependencyObject;
            if (root != null)
            {
                var nav = FindDescendant<NavigationView>(root);
                if (nav != null)
                {
                    // 先尝试根据Tag/Name/Content匹配NavigationViewItem
                    NavigationViewItem? target = nav.MenuItems
                        .OfType<NavigationViewItem>()
                        .FirstOrDefault(i =>
                            (i.Tag != null && i.Tag.ToString() == key) ||
                            (i.Name == key) ||
                            (i.Content?.ToString()?.Equals(key, StringComparison.OrdinalIgnoreCase) == true));

                    // 没找到使用预设索引映射
                    if (target == null)
                    {
                        int index = key switch
                        {
                            "genshin" => 1,
                            "hsr" => 2,
                            "zzz" => 3,
                            _ => 0
                        };
                        if (index >= 0 && nav.MenuItems.Count > index)
                        {
                            target = nav.MenuItems[index] as NavigationViewItem;
                        }
                    }

                    if (target != null)
                    {
                        nav.SelectedItem = target;
                    }
                }
            }
            log.Info($"跳转至{key}页面");
        }

        // 在视觉树中递归查找第一个指定类型的子元素
        private static T? FindDescendant<T>(DependencyObject root) where T : DependencyObject
        {
            if (root == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T t) return t;
                var result = FindDescendant<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
