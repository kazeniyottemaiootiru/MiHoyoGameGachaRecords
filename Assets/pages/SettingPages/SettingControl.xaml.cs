using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords.Assets.pages.SettingPages
{
    public sealed partial class SettingControl : UserControl
    {
        private SettingsData Settings { get; set; }
        private string Game { get; set; } = string.Empty;

        private static readonly ILog log = LogManager.GetLogger(typeof(SettingControl));
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
        public SettingControl()
        {
            InitializeComponent();
            Settings = Setting.LoadSetting();
        }

        public void Initialize(string game)
        {
            Game = game;
            Title.Text = Game switch {
                "Genshin" => loader.GetString("GenshinSettingPage/title"),
                "HoukaiStarRail" => loader.GetString("SettingControl/HSRSetting"),
                "ZZZ" => loader.GetString("SettingControl/ZZZSetting"),
                _ => ""
            };
        }

        private async void ClickGameSerch(object sender, RoutedEventArgs e)
        {
            await Setting.LoadGamePath(Game, Settings);
        }

        private async void ClickRmMemory(object sender, RoutedEventArgs e)
        {
            var isRemove = await MessageBox.InfoYesOrNo(loader.GetString("SettingControl/CleanRecords"));

            if (isRemove == false) return;
            else
            {
                log.Info($"用户选择清除{Game}数据");
                // 清除数据逻辑
                string historyDir = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "History", Game);
                Directory.Delete(historyDir, true);
            }
        }

        private void CreatFile(string path, List<PermanentCharacters> permanentCharacters)
        {
            using StreamWriter writer = new(path, false, Encoding.UTF8);
            writer.WriteLine("角色,时间");
            foreach (var item in permanentCharacters) writer.WriteLine($"{item.name},{item.time}");
        }

        public static List<PermanentCharacters> ReadPerFile(string path)
        {
            List<PermanentCharacters> result = new();
            if (!File.Exists(path)) return result;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 2)
                {
                    result.Add(new PermanentCharacters { name = parts[0], time = parts[1] });
                }
            }
            return result;
        }

        private List<PermanentCharacters> CharacterItems(string lang)
        {
            List<PermanentCharacters> items = [];

            if (Game == "Genshin")
            {
                items.AddRange(lang switch
                {
                    "zh-CN" => new List<PermanentCharacters>
                    {
                        new() { name = "七七", time = "2020-09-28" },
                        new() { name = "刻晴", time = "2020-09-28" },
                        new() { name = "迪卢克", time = "2020-09-28"},
                        new() { name = "琴", time = "2020-09-28" },
                        new() { name = "莫娜", time = "2020-09-28" },
                        new() { name = "提纳里", time = "2022-09-28" },
                        new() { name = "迪希雅", time = "2023-04-12" },
                        new() { name = "梦见月瑞希", time = "2025-03-26" },
                        new() { name = "天空之刃", time = "2020-09-28" },
new() { name = "风鹰剑", time = "2020-09-28" },
new() { name = "狼的末路", time = "2020-09-28" },
new() { name = "天空之傲", time = "2020-09-28" },
new() { name = "和璞鸢", time = "2020-09-28" },
new() { name = "天空之脊", time = "2020-09-28" },
new() { name = "四风原典", time = "2020-09-28" },
new() { name = "天空之卷", time = "2020-09-28" },
new() { name = "阿莫斯之弓", time = "2020-09-28" },
new() { name = "天空之翼", time = "2020-09-28" }
                    },
                    "zh-TW" => new List<PermanentCharacters>
                    {
                        new() { name = "七七", time = "2020-09-28" },
                        new() { name = "刻晴", time = "2020-09-28" },
                        new() { name = "迪盧克", time = "2020-09-28"},
                        new() { name = "琴", time = "2020-09-28" },
                        new() { name = "莫娜", time = "2020-09-28" },
                        new() { name = "提納里", time = "2022-09-28" },
                        new() { name = "迪希雅", time = "2023-04-12" },
                        new() { name = "夢見月瑞希", time = "2025-03-26" },
                        new() { name = "天空之刃", time = "2020-09-28" },
new() { name = "風鷹劍", time = "2020-09-28" },
new() { name = "狼的末路", time = "2020-09-28" },
new() { name = "天空之傲", time = "2020-09-28" },
new() { name = "和璞鳶", time = "2020-09-28" },
new() { name = "天空之脊", time = "2020-09-28" },
new() { name = "四風原典", time = "2020-09-28" },
new() { name = "天空之卷", time = "2020-09-28" },
new() { name = "阿莫斯之弓", time = "2020-09-28" },
new() { name = "天空之翼", time = "2020-09-28" }
                    },
                    "ja-JP" => new List<PermanentCharacters>
                    {
                        new() { name = "七七", time = "2020-09-28" },
                        new() { name = "刻晴", time = "2020-09-28" },
                        new() { name = "ディルック", time = "2020-09-28"},
                        new() { name = "ジン", time = "2020-09-28" },
                        new() { name = "モナ", time = "2020-09-28" },
                        new() { name = "ティナリ", time = "2022-09-28" },
                        new() { name = "ディシア", time = "2023-04-12" },
                        new() { name = "夢見月瑞希", time = "2025-03-26" },
                        new() { name = "天空の刃", time = "2020-09-28" },
new() { name = "風鷹剣", time = "2020-09-28" },
new() { name = "狼の末路", time = "2020-09-28" },
new() { name = "天空の傲", time = "2020-09-28" },
new() { name = "和璞鳶", time = "2020-09-28" },
new() { name = "天空の脊", time = "2020-09-28" },
new() { name = "四風原典", time = "2020-09-28" },
new() { name = "天空の巻", time = "2020-09-28" },
new() { name = "アモスの弓", time = "2020-09-28" },
new() { name = "天空の翼", time = "2020-09-28" }
                    },
                    "en-US" => new List<PermanentCharacters>
                    {
                        new() { name = "Qiqi", time = "2020-09-28" },
                        new() { name = "Keqing", time = "2020-09-28" },
                        new() { name = "Diluc", time = "2020-09-28"},
                        new() { name = "Jean", time = "2020-09-28" },
                        new() { name = "Mona", time = "2020-09-28" },
                        new() { name = "Tighnari", time = "2022-09-28" },
                        new() { name = "Dehya", time = "2023-04-12" },
                        new() { name = "Yumemizuki Mizuki", time = "2025-03-26" },
                        new() { name = "Skyward Blade", time = "2020-09-28" },
new() { name = "Aquila Favonia", time = "2020-09-28" },
new() { name = "Wolf's Gravestone", time = "2020-09-28" },
new() { name = "Skyward Pride", time = "2020-09-28" },
new() { name = "Primordial Jade Winged-Spear", time = "2020-09-28" },
new() { name = "Skyward Spine", time = "2020-09-28" },
new() { name = "Lost Prayer to the Sacred Winds", time = "2020-09-28" },
new() { name = "Skyward Atlas", time = "2020-09-28" },
new() { name = "Amos' Bow", time = "2020-09-28" },
new() { name = "Skyward Harp", time = "2020-09-28" }
                    },
                    _ => []
                });
            }
            else if (Game == "HoukaiStarRail")
            {
                items.AddRange(lang switch
                {
                    "zh-CN" => new List<PermanentCharacters>
                    {
                        new() {name = "瓦尔特", time = "2023-04-26"},
                        new() {name = "布洛妮娅", time = "2023-04-26"},
                        new() {name = "彦卿", time = "2023-04-26"},
                        new() {name = "白露", time = "2023-04-26"},
                        new() {name = "克拉拉", time = "2023-04-26"},
                        new() {name = "杰帕德", time = "2023-04-26"},
                        new() {name = "姬子", time = "2023-04-26"},
                        new() { name = "银河铁道之夜", time = "2023-04-26" },
new() { name = "无可取代的东西", time = "2023-04-26" },
new() { name = "但战斗还未结束", time = "2023-04-26" },
new() { name = "以世界之名", time = "2023-04-26" },
new() { name = "制胜的瞬间", time = "2023-04-26" },
new() { name = "如泥酣眠", time = "2023-04-26" },
new() { name = "时节不居", time = "2023-04-26" }
                    },
                    "zh-TW" => new List<PermanentCharacters>
                    {
                        new() {name = "瓦爾特", time = "2023-04-26"},
                        new() {name = "布洛妮婭", time = "2023-04-26"},
                        new() {name = "彥卿", time = "2023-04-26"},
                        new() {name = "白露", time = "2023-04-26"},
                        new() {name = "克拉拉", time = "2023-04-26"},
                        new() {name = "傑帕德", time = "2023-04-26"},
                        new() {name = "姬子", time = "2023-04-26"},
                        new() { name = "銀河鐵道之夜", time = "2023-04-26" },
new() { name = "無可取代的東西", time = "2023-04-26" },
new() { name = "但戰鬥還未結束", time = "2023-04-26" },
new() { name = "以世界之名", time = "2023-04-26" },
new() { name = "制勝的瞬間", time = "2023-04-26" },
new() { name = "如泥酣眠", time = "2023-04-26" },
new() { name = "時節不居", time = "2023-04-26" }
                    },
                    "ja-JP" => new List<PermanentCharacters>
                    {
                        new() {name = "ヴェルト", time = "2023-04-26"},
                        new() {name = "ブローニヤ", time = "2023-04-26"},
                        new() {name = "彦卿", time = "2023-04-26"},
                        new() {name = "白露", time = "2023-04-26"},
                        new() {name = "クラーラ", time = "2023-04-26"},
                        new() {name = "ジェパード", time = "2023-04-26"},
                        new() {name = "姫子", time = "2023-04-26"},
                        new() { name = "銀河鉄道の夜", time = "2023-04-26" },
new() { name = "かけがえのないもの", time = "2023-04-26" },
new() { name = "だが戦争は終わらない", time = "2023-04-26" },
new() { name = "世界の名を以て", time = "2023-04-26" },
new() { name = "勝利の刹那", time = "2023-04-26" },
new() { name = "泥の如き眠り", time = "2023-04-26" },
new() { name = "時節は居らず", time = "2023-04-26" }
                    },
                    "en-US" => new List<PermanentCharacters>
                    {
                        new() {name = "Welt", time = "2023-04-26"},
                        new() {name = "Bronya", time = "2023-04-26"},
                        new() {name = "Yanqing", time = "2023-04-26"},
                        new() {name = "Bailu", time = "2023-04-26"},
                        new() {name = "Clara", time = "2023-04-26"},
                        new() {name = "Gepard", time = "2023-04-26"},
                        new() {name = "Himeko", time = "2023-04-26"},
                        new() { name = "Night on the Milky Way", time = "2023-04-26" },
new() { name = "Something Irreplaceable", time = "2023-04-26" },
new() { name = "But the Battle Isn't Over", time = "2023-04-26" },
new() { name = "In the Name of the World", time = "2023-04-26" },
new() { name = "Moment of Victory", time = "2023-04-26" },
new() { name = "Sleep Like the Dead", time = "2023-04-26" },
new() { name = "Time Waits for No One", time = "2023-04-26" }
                    },
                    _ => []
                });
            }
            else if (Game == "ZZZ")
            {
                items.AddRange(lang switch
                {
                    "zh-CN" => new List<PermanentCharacters>
                    {
                        new() { name = "「11号」", time = "2024-07-04"},
                        new() { name = "莱卡恩", time = "2024-07-04"},
                        new() { name = "珂蕾妲", time = "2024-07-04"},
                        new() { name = "猫又", time = "2024-07-04"},
                        new() { name = "格莉丝", time = "2024-07-04"},
                        new() { name = "丽娜", time = "2024-07-04"},
                        new() { name = "钢铁肉垫", time = "2024-07-04" },
new() { name = "硫磺石", time = "2024-07-04" },
new() { name = "燃狱齿轮", time = "2024-07-04" },
new() { name = "拘缚者", time = "2024-07-04" },
new() { name = "嵌合编译器", time = "2024-07-04" },
new() { name = "啜泣摇篮", time = "2024-07-04" }
                    },
                    "zh-TW" => new List<PermanentCharacters>
                    {
                        new() { name = "「11號」", time = "2024-07-04"},
                        new() { name = "莱卡恩", time = "2024-07-04"},
                        new() { name = "珂蕾妲", time = "2024-07-04"},
                        new() { name = "貓又", time = "2024-07-04"},
                        new() { name = "格莉絲", time = "2024-07-04"},
                        new() { name = "麗娜", time = "2024-07-04"},
                        new() { name = "鋼鐵肉墊", time = "2024-07-04" },
new() { name = "硫磺石", time = "2024-07-04" },
new() { name = "燃獄齒輪", time = "2024-07-04" },
new() { name = "拘縛者", time = "2024-07-04" },
new() { name = "嵌合編譯器", time = "2024-07-04" },
new() { name = "啜泣搖籃", time = "2024-07-04" }
                    },
                    "ja-JP" => new List<PermanentCharacters>
                    {
                        new() { name = "「11号」", time = "2024-07-04"},
                        new() { name = "ライカン", time = "2024-07-04"},
                        new() { name = "クレタ", time = "2024-07-04"},
                        new() { name = "猫又", time = "2024-07-04"},
                        new() { name = "グレース", time = "2024-07-04"},
                        new() { name = "リナ", time = "2024-07-04"},
                        new() { name = "鋼の肉球", time = "2024-07-04" },
new() { name = "ブリムストーン", time = "2024-07-04" },
new() { name = "燃獄ギア", time = "2024-07-04" },
new() { name = "拘縛されし者", time = "2024-07-04" },
new() { name = "複合コンパイラ", time = "2024-07-04" },
new() { name = "啜り泣くゆりかご", time = "2024-07-04" }
                    },
                    "en-US" => new List<PermanentCharacters>
                    {
                        new() { name = "Soldier 11", time = "2024-07-04"},
                        new() { name = "Lycaon", time = "2024-07-04"},
                        new() { name = "Koleda", time = "2024-07-04"},
                        new() { name = "Nekomata", time = "2024-07-04"},
                        new() { name = "Grace", time = "2024-07-04"},
                        new() { name = "Rina", time = "2024-07-04"},
                        new() { name = "Steel Cushion", time = "2024-07-04" },
new() { name = "The Brimstone", time = "2024-07-04" },
new() { name = "Hellfire Gears", time = "2024-07-04" },
new() { name = "The Restrained", time = "2024-07-04" },
new() { name = "Fusion Compiler", time = "2024-07-04" },
new() { name = "Weeping Cradle", time = "2024-07-04" }
                    },
                    _ => []
                });
            }

            return items;
        }

        public ObservableCollection<PermanentCharacter> Rows { get; } = [];

        private async void PerCha_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                "permanent_character", Game);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);    // 创建存储文件夹
                CreatFile(Path.Combine(path, "zh-CN.csv"), CharacterItems("zh-CN"));
                CreatFile(Path.Combine(path, "zh-TW.csv"), CharacterItems("zh-TW"));
                CreatFile(Path.Combine(path, "ja-JP.csv"), CharacterItems("ja-JP"));
                CreatFile(Path.Combine(path, "en-US.csv"), CharacterItems("en-US"));
            }

            List<string> langs = [];
            List<string> files = ["zh-CN.csv", "zh-TW.csv", "ja-JP.csv", "en-US.csv"];
            int t = Directory.GetFiles(path, "*.*").Length;
            if (Directory.GetFiles(path, "*.*").Length < 4)
            {
                foreach (var file in Directory.GetFiles(path, "*.*"))
                    langs.Add(Path.GetFileName(file));

                foreach (var a in langs)
                {
                    if (a == "zh-CN.csv" || a == "zh-TW.csv" || a == "ja-JP.csv" || a == "en-US.csv")
                        files.Remove(a);
                }
                // 创建文件
                foreach (var a in files) CreatFile(Path.Combine(path, a), CharacterItems(a.Split(".")[0]));
            }


            var listView = new ListView
            {
                ItemsSource = Rows,
                BorderThickness = new Thickness(1),
                BorderBrush = (Brush)Microsoft.UI.Xaml.Application.Current.Resources["DividerStrokeColorDefaultBrush"]
            };

            listView.ItemTemplate = CreateRowTemplate();

            var addButton = new Button
            {
                Content = loader.GetString("Common/Add")
            };

            var resetButton = new Button
            {
                Content = loader.GetString("Common/Reset"),
                Margin = new Thickness(8, 0, 0, 0)
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children = { addButton, resetButton }
            };

            addButton.Click += (_, __) =>
            {
                var newRow = new PermanentCharacter();
                newRow.DeleteCommand = new RelayCommand(() =>
                {
                    if (Rows.Contains(newRow)) Rows.Remove(newRow);
                });
                Rows.Add(newRow);
            };

            resetButton.Click += (_, __) =>
            {
                Rows.Clear();   // 先清空

                var items = CharacterItems(
                    Settings.language switch
                    {
                        0 => "zh-CN",
                        1 => "zh-TW",
                        2 => "ja-JP",
                        3 => "en-US",
                        _ => ""
                    });

                foreach (var i in items)
                {
                    var r = new PermanentCharacter
                    {
                        Character = i.name,
                        Year = i.time.Split('-')[0],
                        Month = i.time.Split('-')[1],
                        Day = i.time.Split('-')[2]
                    };
                    r.DeleteCommand = new RelayCommand(() =>
                    {
                        if (Rows.Contains(r)) Rows.Remove(r);
                    });
                    Rows.Add(r);
                }
            };

            // 为每一行注入DeleteCommand
            Rows.Clear();
            string langFilePath = Path.Combine(path, files[Settings.language]);

            foreach (var b in ReadPerFile(langFilePath))
            {
                var r = new PermanentCharacter
                {
                    Character = b.name,
                    Year = b.time.Split("-")[0],
                    Month = b.time.Split("-")[1],
                    Day = b.time.Split("-")[2]
                };
                r.DeleteCommand = new RelayCommand(() =>
                {
                    if (Rows.Contains(r)) Rows.Remove(r);
                });
                Rows.Add(r);
            }

            // 常驻设置不全修复
            var rootGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            Grid.SetRow(buttonPanel, 0);
            Grid.SetRow(listView, 1);

            rootGrid.Children.Add(buttonPanel);
            rootGrid.Children.Add(listView);

            ContentDialog dialog = new ContentDialog
            {
                Title = loader.GetString("GameSetting/PermanentCharacterSetting"),
                Content = rootGrid,
                PrimaryButtonText = loader.GetString("MessageBox/Primary"),
                CloseButtonText = loader.GetString("MessageBox/cancel"),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Secondary) return;

            List<PermanentCharacters> pc = [];
            foreach (var row in Rows)
                pc.Add(new PermanentCharacters
                {
                    name = row.Character,
                    time = row.Year + "-" + row.Month + "-" + row.Day
                });
            //log.Info($"row1{characters.name}\ttime:{characters.time}");
            CreatFile(Path.Combine(path, files[Settings.language]), pc);
        }

        private DataTemplate CreateRowTemplate()
        {
            string deleteText = WebUtility.HtmlEncode(loader.GetString("Common/Delete"));

            int startYear = DateTime.Now.Year - 100;
            int endYear = DateTime.Now.Year + 100;

            string Years() =>
                string.Join("", Enumerable.Range(startYear, endYear - startYear + 1)
                    .Select(y => $"<ComboBoxItem Content='{y}'/>"));

            string Months() =>
                string.Join("", Enumerable.Range(1, 12)
                    .Select(m => $"<ComboBoxItem Content='{m:00}'/>"));

            string Days() =>
                string.Join("", Enumerable.Range(1, 31)
                    .Select(d => $"<ComboBoxItem Content='{d:00}'/>"));

            string xaml =
        $@"
<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
    <Grid Padding='8'>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width='*'/>
            <ColumnDefinition Width='Auto'/>
            <ColumnDefinition Width='Auto'/>
            <ColumnDefinition Width='Auto'/>
            <ColumnDefinition Width='Auto'/>
        </Grid.ColumnDefinitions>

        <TextBox Text='{{Binding Character, Mode=TwoWay}}'/>

        <ComboBox Grid.Column='1' Width='90'
                  SelectedValuePath='Content'
                  SelectedValue='{{Binding Year, Mode=TwoWay}}'>
            {Years()}
        </ComboBox>

        <ComboBox Grid.Column='2' Width='70'
                  SelectedValuePath='Content'
                  SelectedValue='{{Binding Month, Mode=TwoWay}}'>
            {Months()}
        </ComboBox>

        <ComboBox Grid.Column='3' Width='70'
                  SelectedValuePath='Content'
                  SelectedValue='{{Binding Day, Mode=TwoWay}}'>
            {Days()}
        </ComboBox>

<!--????-->
        <Button Grid.Column='4'
                Content='{deleteText}'
                Foreground='Red'
                Command='{{Binding DeleteCommand}}'/>
    </Grid>
</DataTemplate>";

            return (DataTemplate)XamlReader.Load(xaml);
        }

        private async void ExportHistory(object sender, RoutedEventArgs e)
        {
            string hisPath = Path.Combine(SettingPage.localPath, "History", Game);
            if (!Directory.Exists(hisPath))
            {
                await MessageBox.Error(loader.GetString("SettingControl/NotFoundRecords"));
                return;
            }
            string? exPath = await Setting.OutptutToOnterPath();
            if (exPath == null || SettingPage.localPath == null) return;
            else
            {
                exPath = Path.Combine(exPath, Game);
                if (!Directory.Exists(exPath)) Directory.CreateDirectory(exPath);

                foreach (string path in Directory.GetFiles(hisPath))
                {
                    string fileName = Path.GetFileName(path);
                    string destinationPath = Path.Combine(exPath, fileName);
                    File.Copy(path, destinationPath, true);
                }

                await MessageBox.Info(loader.GetString("SettingControl/EXFinish"));
                return;
            }
        }

        // 导入文件情况复杂，此功能放弃
        //private async void ImportHistory(object sender, RoutedEventArgs e)
        //{
        //    await MessageBox.Info("请选择CSV文件且文件首行为“角色,抽数,时间”为格式的文件。");
        //    string? importFilePath = await Setting.GetFilePath(".csv");
        //    if (importFilePath == null) { await MessageBox.FilePathIsNull(); return; }
        //    List<Mem> impFile = Setting.CSVReader(importFilePath);
        //    string hisDirPath = Path.Combine(SettingPage.localPath, "History", Game);
        //    //List<Mem> lcFile = Setting.CSVReader();
        //    // 导入过程

        //    return;
        //}
        
    }
}
