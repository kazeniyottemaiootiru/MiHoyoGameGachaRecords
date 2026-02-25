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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords.Assets.pages.HistoryPages
{
    public sealed partial class HistoryView : UserControl
    {
        private string Game = string.Empty;
        private string HisFilePath = string.Empty;

        private List<PermanentCharacters> Permanents = new();
        private List<Mem> Mems = new();

        private static readonly ILog log = LogManager.GetLogger(typeof(HistoryView));
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

        private SettingsData Settings;

        public HistoryView()
        {
            InitializeComponent();
            Settings = Setting.LoadSetting();
            FilterComboBox.PlaceholderText = loader.GetString("HistoryView/GachaPool");
            //QueryButton.Content = loader.GetString("Common/Analysis");
        }

        public void Initialize(string game)
        {
            Game = game;
            HisFilePath = Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                "History",
                Game,
                WelcomeGameControl.UID
            );

            List<string> files = ["zh-CN.csv", "zh-TW.csv", "ja-JP.csv", "en-US.csv"];

            Permanents = SettingControl.ReadPerFile(
                Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "permanent_character",
                    Game,
                    files[Settings.language]
                ));

            FilterComboBox.ItemsSource = Game switch
            {
                "Genshin" => new[]
                {
                    loader.GetString("Type/UP"),
                    loader.GetString("Type/weapon"),
                    loader.GetString("Type/MixPool"),
                    loader.GetString("Type/permanent")
                },
                "HoukaiStarRail" => new[]
                {
                    loader.GetString("Type/UP"),
                    loader.GetString("Type/lightCone"),
                    loader.GetString("Type/permanent"),
                    loader.GetString("Type/collaboration")
                },
                "ZZZ" => new[]
                {
                    loader.GetString("Type/agent"),
                    loader.GetString("Type/WEngine"),
                    loader.GetString("Type/permanent"),
                    loader.GetString("Type/bangboo")
                },
                _ => Array.Empty<string>()
            };

            //QueryButton.Content = loader.GetString("Common/Query");
        }

        private async void GetMems_Click(object sender, RoutedEventArgs e)
        {
            if (FilterComboBox.SelectedIndex < 0)
            {
                await MessageBox.Warn(loader.GetString("HistoryView/NoGachaPool"));
                return;
            }
            string? pool = GetPoolName();
            if (pool == null) return;

            string filePath = Path.Combine(HisFilePath, $"{pool}.csv");
            Mems = Setting.CSVReader(filePath);

            // 修复对应抽卡记录文件不存在时没有提示
            if (Mems == null || Mems.Count == 0)
            {
                await MessageBox.Warn(loader.GetString("HistoryView/NotFound"));
                return;
            }

            BuildView(pool);
        }

        private void BuildView(string pool)
        {
            int max = FilterComboBox.SelectedIndex == 1 ? 80 : 90;
            int warn = max == 90 ? 73 : 62;

            int offCount = 0;
            var list = new List<MemViewModel>();

            foreach (var mem in Mems)
            {
                if (mem.name == "") mem.name = "???";
                bool exists = Permanents.Any(p =>
                    p.name == mem.name &&
                    DateTime.Parse(p.time) < DateTime.Parse(mem.time));

                if (exists) offCount++;
                if (pool == "permanent") exists = false;

                list.Add(new MemViewModel(
                    mem,
                    max,
                    warn,
                    exists,
                    GetExistText()
                ));
            }

            MemList.ItemsSource = list;
            //SetInfomationText(list.Count == 0 ? 0 : (float)offCount / list.Count);
        }

        private string? GetPoolName()
        {
            return Game switch
            {
                "Genshin" => FilterComboBox.SelectedIndex switch
                {
                    0 => "up",
                    1 => "weapon",
                    2 => "mix",
                    3 => "permanent",
                    _ => null
                },
                "HoukaiStarRail" => FilterComboBox.SelectedIndex switch
                {
                    0 => "up",
                    1 => "light_cone",
                    2 => "permanent",
                    3 => "liandong",
                    _ => null
                },
                "ZZZ" => FilterComboBox.SelectedIndex switch
                {
                    0 => "up",
                    1 => "W-Engine",
                    2 => "permanent",
                    3 => "bangboo",
                    _ => null
                },
                _ => null
            };
        }

        /*private void SetInfomationText(float percent)
        {
            InfomationText.Text =
                percent > 0.8 ? "Text1 (This is a test text used to analyze the gacha situation; the issue here will be fixed later.)" :
                percent > 0.6 ? "Text2 (This is a test text used to analyze the gacha situation; the issue here will be fixed later.)" :
                percent > 0.4 ? "Text3 (This is a test text used to analyze the gacha situation; the issue here will be fixed later.)" :
                percent > 0.2 ? "Text4 (This is a test text used to analyze the gacha situation; the issue here will be fixed later.)" :
                "Text5 (This is a test text used to analyze the gacha situation; the issue here will be fixed later.)";
        }*/

        private string GetExistText()
        {
            return Settings.language switch
            {
                0 => "歪",
                1 => "歪",
                2 => "すり抜け",
                3 => "Off-banner",
                _ => "×"
            };
        }
    }
}
