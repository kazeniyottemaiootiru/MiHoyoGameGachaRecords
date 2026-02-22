using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MiHoyoGameGachaRecords.Assets.pages.HistoryPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords.Assets.pages;

public sealed partial class WelcomeGameControl : UserControl
{
    public string Game { get; private set; } = string.Empty;
    public string? InstallPath { get; private set; }
    public static string UID { get; private set; } = string.Empty;

    private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

    public WelcomeGameControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 初始化控件
    /// </summary>
    public void Initialize(string game, string? installPath)
    {
        Game = game;
        InstallPath = installPath;

        string temp = game switch
        {
            "Genshin" => loader.GetString("Main/genshin"),
            "HoukaiStarRail" => loader.GetString("Main/houkaiStarRail"),
            "ZZZ" => loader.GetString("Main/ZZZ"),
            _ => ""
        };

        TitleText.Text = string.Format(loader.GetString("Welcom/WelcomeText"), temp);
        PathText.Text = loader.GetString("Welcome/InstallPath") + InstallPath;
        UidComboBox.PlaceholderText = loader.GetString("Welcome/UidPlaceholder");

        LoadUidList();
    }

    /// <summary>
    /// 从History/{game}目录加载UID（文件夹名）
    /// </summary>
    private void LoadUidList()
    {
        UidComboBox.Items.Clear();

        var historyPath = Path.Combine(SettingPage.localPath, "History", Game);

        if (!Directory.Exists(historyPath))
            return;

        foreach (var dir in Directory.GetDirectories(historyPath))
        {
            var uid = Path.GetFileName(dir);
            UidComboBox.Items.Add(uid);
        }
    }

    private async void GetHistory_Click(object sender, RoutedEventArgs e)
    {
        var radioButtons = new RadioButtons
        {
            ItemsSource = Game switch
            {
                "Genshin" => new List<string>
                {
                    loader.GetString("Type/UP"),
                    loader.GetString("Type/weapon"),
                    loader.GetString("Type/permanent"),
                    loader.GetString("Type/all")
                },
                "HoukaiStarRail" => new List<string>
                {
                    loader.GetString("Type/UP"),
                    loader.GetString("Type/lightCone"),
                    loader.GetString("Type/collaboration"),
                    loader.GetString("Type/permanent"),
                    loader.GetString("Type/all")
                },
                "ZZZ" => new List<string>
                {
                    loader.GetString("Type/agent"),
                    loader.GetString("Type/WEngine"),
                    loader.GetString("Type/permanent"),
                    loader.GetString("Type/bangboo"),
                    loader.GetString("Type/all")
                },
                _ => ""
            },
            SelectedIndex = 0
        };
        var dialog = new ContentDialog
        {
            Title = loader.GetString("Welcome/HistoryTitle"),
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                new TextBlock
                {
                    Text = loader.GetString("Welcome/HContentText"),
                    Opacity = 0.8
                },
                radioButtons
                }
            },
            PrimaryButtonText = loader.GetString("MessageBox/Primary"),
            CloseButtonText = loader.GetString("MessageBox/cancel"),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        await MessageBox.Info(loader.GetString("Welcome/HGStart"));
        int? gachaType = null;
        if (Game == "Genshin")
        {
            gachaType = radioButtons.SelectedIndex switch
            {
                0 => 301,
                1 => 302,
                2 => 200,
                _ => null
            };
        }
        else if (Game == "HoukaiStarRail")
        {
            gachaType = radioButtons.SelectedIndex switch {
                0 => 11,
                1 => 12,
                2 => 21,
                3 => 1,
                _ => null
            };
        }
        else if(Game == "ZZZ")
        {
            gachaType = radioButtons.SelectedIndex switch
            {
                0 => 2001,
                1 => 3001,
                2 => 1001,
                3 => 5001,
                _ => null
            };
        }

        if (InstallPath == null) return;

        GetWebJsonItems jsonItems = new(InstallPath);
        if (gachaType != null)
            await jsonItems.CreatCSVFile(await jsonItems.NeedJson(gachaType.Value, SelectInfo), 
                gachaType.Value, SelectInfo);
        else
        {
            if (Game == "Genshin")
            {
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(301, SelectInfo), 301, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(302, SelectInfo), 302, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(200, SelectInfo), 200, SelectInfo);
            }
            else if (Game == "HoukaiStarRail")
            {
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(11, SelectInfo), 11, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(12, SelectInfo), 12, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(21, SelectInfo), 21, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(1, SelectInfo), 1, SelectInfo);
            }
            else if (Game == "ZZZ")
            {
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(2001, SelectInfo), 2001, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(3001, SelectInfo), 3001, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(1001, SelectInfo), 1001, SelectInfo);
                await jsonItems.CreatCSVFile(await jsonItems.NeedJson(5001, SelectInfo), 5001, SelectInfo);
            }
        }
        await MessageBox.Info(loader.GetString("Welcome/GetFinish"));
        LoadUidList();
        return;
    }

    private async void ViewHistory_Click(object sender, RoutedEventArgs e)
    {
        if (UidComboBox.SelectedItem is string uid)
        {
            UID = uid;
        }
        if(UidComboBox.SelectedIndex < 0)
        {
            await MessageBox.Warn("请选择UID！");
            return;
        }

        var frame = FindHostFrame();
        if (frame == null) return;

        switch (Game)
        {
            case "Genshin":
                frame.Navigate(typeof(GenshinHistoryPage));
                break;
            case "HoukaiStarRail":
                frame.Navigate(typeof(HSRHistoryPage));
                break;
            case "ZZZ":
                frame.Navigate(typeof(ZZZHistoryPage));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 在视觉树或父链中查找最近的Frame实例用于导航。
    /// </summary>
    private Frame? FindHostFrame()
    {
        DependencyObject? current = this;
        while (current != null)
        {
            if (current is Frame f)
                return f;

            // 返回父级对象
            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private async void LaunchGame_Click(object sender, RoutedEventArgs e)
    {
        if(InstallPath != null) await Setting.RunGame(InstallPath);
        else
        {
            await MessageBox.Warn(loader.GetString("Welcome/NoPath"));
            return;
        }
    }
}
