using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MiHoyoGameGachaRecords.Assets.pages.SettingPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Text.Json;
using Microsoft.Windows.Globalization;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI;
using Microsoft.UI.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords.Assets.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private int langState;
        private int themeState;

        private bool isInitializing = true;

        public static string localPath { get; } = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        public static string settingFilePath { get; } = Path.Combine(localPath, "setting.json");

        private readonly string[] LangList = ["简体中文", "繁體中文", "日本語", "English"];
        public SettingPage()
        {
            InitializeComponent();
            this.Loaded += SettingPage_Loaded;

            LoadSettings(localPath);    // 加载设置
        }

        private void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            isInitializing = false;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SettingPage));

        // 设置加载函数
        private void LoadSettings(string LocalPath)
        {
            // 如果设置文件不存在，则创建默认设置文件
            if (!File.Exists(settingFilePath))
            {
                var defaultSettings = Setting.ResetSetting(settingFilePath);

                lang.SelectedIndex = 0;
                theme.SelectedIndex = 0;
                return;
            }
            else
            {
                var SettingItem = JsonSerializer.Deserialize<SettingsData>
                    (File.ReadAllText(settingFilePath));
                if (SettingItem == null) { 
                    log.Warn("setting.json文件为空，重置设置。");
                    Setting.ResetSetting (settingFilePath);
                    lang.SelectedIndex = 0;
                    theme.SelectedIndex = 0;
                    return;
                }
                else
                {   // 加载内容
                    lang.SelectedIndex = SettingItem.language;
                    langState = SettingItem.language;
                    theme.SelectedIndex = SettingItem.theme;
                    themeState = SettingItem.theme;
                    return;
                }
            }
        }

        private bool SaveSettings(string localPath)
        {
            string settingFilePath = Path.Combine(localPath, "setting.json");
            if (!File.Exists(settingFilePath)) { LoadSettings(localPath); }
            var SettingItem = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(settingFilePath));
            if (SettingItem == null)
            {
                log.Warn("setting.json文件为空");
                return false;
            }
            else
            {
                bool needSave = false;
                // 获取json中自启开关状态是否与当前开关状态一致，不一致写变量
                if(langState != SettingItem.language)
                {
                    SettingItem.language = langState;
                    needSave = true;
                }
                if(themeState != SettingItem.theme)
                {
                    SettingItem.theme = themeState;
                    needSave = true;
                }
                
                // 保存
                if (needSave) {
                    string newJson = JsonSerializer.Serialize(SettingItem, 
                        new JsonSerializerOptions{ WriteIndented = true});

                    File.WriteAllText(settingFilePath, newJson);

                    log.Info("设置内容已更新");
                    return true;
                }
                return false;
            }
        }

        private async void LangSelect(object sender, SelectionChangedEventArgs e)
        { 
            if (isInitializing) return; // 加载不调设置
            string? lang = e.AddedItems[0]?.ToString();
            if (lang == null) return;

            if (lang == LangList[0])
                ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
            else if (lang == LangList[1])
                ApplicationLanguages.PrimaryLanguageOverride = "zh-TW";
            else if (lang == LangList[2])
                ApplicationLanguages.PrimaryLanguageOverride = "ja-JP";
            else if (lang == LangList[3])
                ApplicationLanguages.PrimaryLanguageOverride = "en-US";
            else
                ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";

            langState = Array.IndexOf(LangList, lang);
            bool flag = SaveSettings(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
            log.Info($"获取到语言状态为{langState}");

            await MessageBox.ShowLanguageRestartDialog();
            
            return;
        }

        private void ThemeSelect(object sender, SelectionChangedEventArgs e)
        {
            if (isInitializing) return;
            var box = sender as ComboBox;
            if (box == null || box.SelectedIndex < 0) return;

            themeState = box.SelectedIndex;

            SaveSettings(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
            log.Info($"获取到主题状态为{themeState}");
            SaveSettings(localPath);

            if (App.MainWindowInstance?.Content is not FrameworkElement root)
                return;

            switch (themeState)
            {
                case 0: // 跟随系统
                    root.RequestedTheme = ElementTheme.Default;
                    break;
                case 1: // 浅色
                    root.RequestedTheme = ElementTheme.Light;
                    break;
                case 2: // 深色
                    root.RequestedTheme = ElementTheme.Dark;
                    break;
            }
            return;
        }


        /// <summary>
        /// 跳转原神设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoGenshinSettingPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GenshinSettingPage));
            
            log.Info("跳转至原神设置页面");
            return;
        }

        /// <summary>
        /// 跳转崩坏：星穹铁道设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoHoukaiStarRailSettingPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HoukaiStarRailSettingPage));

            log.Info("跳转至崩坏：星穹铁道设置页面");
            return;
        }

        /// <summary>
        /// 跳转绝区零设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoZZZSettingPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ZZZSettingPage));

            log.Info("跳转至绝区零设置页面");
            return;
        }

        private async void OutputLogs(object sender, RoutedEventArgs e)
        {
            string? opPath = await Setting.OutptutToOnterPath();

            if (opPath != null && localPath != null)
            {
                string logPath = Path.Combine(localPath, "Logs");
                opPath = Path.Combine(opPath, "Logs");
                Directory.CreateDirectory(opPath);

                foreach (string path in Directory.GetFiles(logPath))
                {
                    string fileName = Path.GetFileName(path);
                    string destinationPath = Path.Combine(opPath, fileName);
                    File.Copy(path, destinationPath, true);
                }

                await MessageBox.OutPutFinish(opPath);
            }
            else return;
        }
    }
}
