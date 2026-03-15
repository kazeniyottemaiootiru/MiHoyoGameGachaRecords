using log4net;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using MiHoyoGameGachaRecords.Assets.pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.ApplicationSettings;
using static MiHoyoGameGachaRecords.Assets.pages.SettingPage;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
        public MainWindow()
        {
            this.InitializeComponent();
            SystemBackdrop = new MicaBackdrop
            {
                Kind = MicaKind.Base
            };
            ExtendsContentIntoTitleBar = true;

            SetTitleBar(AppTitleBar);   // 设置标题栏

            TrySetBackdrop();

            // 初始导航到首页
            contentFrame.Navigate(typeof(Assets.pages.HomePage));

            // 确保侧边栏与首页保持一致
            SelectNavByTag("home");

            string LogDir = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Logs");
            if(!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);

            contentFrame.Navigated += BackPage;

            // 获取当前语言
            string area = System.Globalization.CultureInfo.CurrentUICulture.Name;

            // 绑定标题栏控件颜色和设置加载
            if (this.Content is FrameworkElement root)
            {
                root.Loaded += Root_Loaded;
                LoadSetting();
            }
            Title = loader.GetString("App/title/Text");
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement root)
            {
                root.ActualThemeChanged += Root_ActualThemeChanged;
                UpdateTitleBarForTheme(root.ActualTheme);
            }
        }
        private void Root_ActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateTitleBarForTheme(sender.ActualTheme);
        }

        private void UpdateTitleBarForTheme(ElementTheme theme)
        {
            var titleBar = this.AppWindow.TitleBar;

            if (theme == ElementTheme.Dark)
            {
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
            }
        }

        public IntPtr GetWindowHandle()
        {
            return WinRT.Interop.WindowNative.GetWindowHandle(this);
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        // 公开NavigationView
        public NavigationView NavView => guide;

        public void SelectNavByTag(string tag)
        {
            if (guide == null) return;

            try
            {
                if (string.Equals(tag, "settings", StringComparison.OrdinalIgnoreCase))
                {
                    // 侧栏的设置项是独立的SettingsItem
                    if (guide.SettingsItem != null)
                    {
                        guide.SelectedItem = guide.SettingsItem;
                    }
                    return;
                }

                var target = guide.MenuItems
                    .OfType<NavigationViewItem>()
                    .FirstOrDefault(i => (i.Tag?.ToString() ?? string.Empty)
                    .Equals(tag, StringComparison.OrdinalIgnoreCase));

                if (target != null)
                {
                    guide.SelectedItem = target;
                }
                else
                {
                    // 如果没有匹配项，清除选中
                    guide.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                log.Error($"设置侧边栏选中项失败：{ex}");
            }
        }

        private void navigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                // 内置设置项
                if (args.IsSettingsSelected)
                {
                    contentFrame.Navigate(typeof(Assets.pages.SettingPage));
                    return;
                }
                switch (args.SelectedItemContainer.Tag)
                {
                    case "home":
                        contentFrame.Navigate(typeof(Assets.pages.HomePage));
                        log.Info("点击侧栏跳转至首页");
                        break;
                    case "genshin":
                        contentFrame.Navigate(typeof(Assets.pages.GenshinPage));
                        log.Info("点击侧栏跳转至原神页面");
                        break;
                    case "hsr":
                        contentFrame.Navigate(typeof(Assets.pages.HoukaiStarrailPage));
                        log.Info("点击侧栏跳转至崩坏：星穹铁道页面");
                        break;
                    case "zzz":
                        contentFrame.Navigate(typeof(Assets.pages.zzzPage));
                        log.Info("点击侧栏跳转至绝区零页面");
                        break;
                    default:
                        break;
                }
            }
        }

        public void TrySetBackdrop()
        {
            // Win11：Mica
            if (MicaController.IsSupported())
            {
                this.SystemBackdrop = new MicaBackdrop()
                {
                    Kind = MicaKind.Base
                };
                log.Info("加载了win11的UI");
                return;
            }

            // Win10：Acrylic
            if (DesktopAcrylicController.IsSupported())
            {
                this.SystemBackdrop = new DesktopAcrylicBackdrop();
                log.Info("加载了win10的UI");
                return;
            }
        }

        public void SetMica()
        {
            if (MicaController.IsSupported())
            {
                this.SystemBackdrop = new MicaBackdrop()
                {
                    Kind = MicaKind.Base
                };
            }
        }

        public void SetAcrylic()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                this.SystemBackdrop = new DesktopAcrylicBackdrop();
            }
        }

        public void Natigate(string key)
        {
            contentFrame.Navigate(key switch
            {
                "home" => typeof(Assets.pages.HomePage),
                "genshin" => typeof(Assets.pages.GenshinPage),
                "hsr" => typeof(Assets.pages.HoukaiStarrailPage),
                "zzz" => typeof(Assets.pages.zzzPage),
                "settings" => typeof(Assets.pages.SettingPage),
                _ => null
            });
        }

        private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }

        private void BackPage(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // 更新后退按钮可用状态
            guide.IsBackEnabled = contentFrame.CanGoBack;

            // 同步侧边栏选中项到当前页面
            try
            {
                string? tag = e.SourcePageType switch
                {
                    _ when e.SourcePageType == typeof(Assets.pages.HomePage) => "home",
                    _ when e.SourcePageType == typeof(Assets.pages.GenshinPage) => "genshin",
                    _ when e.SourcePageType == typeof(Assets.pages.HoukaiStarrailPage) => "hsr",
                    _ when e.SourcePageType == typeof(Assets.pages.zzzPage) => "zzz",
                    _ when e.SourcePageType == typeof(Assets.pages.SettingPage) => "settings",
                    _ => null
                };

                if (!string.IsNullOrEmpty(tag))
                {
                    SelectNavByTag(tag);
                }
                else
                {
                    // 当前页面不是侧边栏管理的页面清除选中项
                    guide.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                log.Error($"BackPage同步侧边栏失败：{ex}");
            }
        }

        private void LoadSetting()
        {
            string settingFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "setting.json");
            SettingsData? defaultSetting;

            if (!File.Exists(settingFilePath))
            {
                string area = System.Globalization.CultureInfo.CurrentUICulture.Name;
                int langTemp = area switch
                {
                    "zh-CN" => 0,
                    "zh-TW" => 1,
                    "ja-JP" => 2,
                    "en-US" => 3,
                    _ => 3
                };
                defaultSetting = new SettingsData
                {
                    language = langTemp,
                    theme = 0,
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(defaultSetting, options);
                File.WriteAllText(settingFilePath, jsonString);
            }
            else
            {
                defaultSetting = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(settingFilePath));
                if (defaultSetting == null)
                {
                    log.Warn("setting.json文件为空");
                    return;
                }
            }
            SettingLoadLang(defaultSetting);
            SettingLoadTheme(defaultSetting);
        }

        private void SettingLoadLang(SettingsData data)
        {
            ApplicationLanguages.PrimaryLanguageOverride = data.language switch
            {
                0 => "zh-CN",
                1 => "zh-TW",
                2 => "ja-JP",
                3 => "en-US",
                _ => "en-US",
            };
            return;
        }

        private void SettingLoadTheme(SettingsData? data)
        {
            log.Info("初始化已开始");
            if (this.Content is not FrameworkElement root)
            {
                log.Info("未获取到窗口");
                return;
            }

            if (data != null)
            {
                log.Info($"{data.theme}");
                switch (data.theme)
                {
                    case 0: // 跟随系统
                        root.RequestedTheme = ElementTheme.Default;
                        log.Info("默认");
                        break;
                    case 1: // 浅色
                        root.RequestedTheme = ElementTheme.Light;
                        log.Info("浅色");
                        break;
                    case 2: // 深色
                        root.RequestedTheme = ElementTheme.Dark;
                        log.Info("深色");
                        break;
                    default:
                        root.RequestedTheme = ElementTheme.Default;
                        break;
                }
                return;
            }
            else
            {
                log.Info("data == null");
                root.RequestedTheme = ElementTheme.Default;
                return;
            }
        }
    }
}
