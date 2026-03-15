using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using MiHoyoGameGachaRecords.Assets.pages;
using MiHoyoGameGachaRecords.Assets.pages.SettingPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MiHoyoGameGachaRecords
{
    internal class Setting
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Setting));
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

        /// <summary>
        /// 调用系统API获取exe文件路径
        /// </summary>
        /// <returns></returns>
        public static async Task<string?> GetFilePath(string fileType)
        {
            var OpenFile = new FileOpenPicker();
            InitializeWithWindow.Initialize(OpenFile, App.MainWindowInstance!.GetWindowHandle());
            OpenFile.FileTypeFilter.Add(fileType);

            var file = await OpenFile.PickSingleFileAsync();
            if (file != null)
            {
                // 处理选择的文件
                log.Info($"选择文件：{file.Path}");
                return file.Path;
            }
            else
            {
                // 用户未选择文件
                return null;
            }
        }
        public static async Task<string?> OutptutToOnterPath()
        {
            var OpenFile = new FolderPicker();
            InitializeWithWindow.Initialize(OpenFile, App.MainWindowInstance!.GetWindowHandle());
            //OpenFile.FileTypeFilter.Add("*");
            
            var file = await OpenFile.PickSingleFolderAsync();
            if (file != null)
            {
                log.Info($"导出路径{file.Path}");
                return file.Path;
            }
            else { return null; }
        }

        public static async Task RunGame(string path)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = path,
                Verb = "runas",
                UseShellExecute = true
            };
            try
            {
                Process.Start(processStartInfo);
                
            }
            catch (Win32Exception e)
            {
                await MessageBox.Info(ResourceLoader.GetForViewIndependentUse().GetString("Setting/GameNotRun"));
                log.Error($"游戏未启动，情况：{e}");
            }
        }

        public static void CSVWriter(List<Mem> mems, string path)
        {
            using StreamWriter writer = new(path, false, Encoding.UTF8);
            writer.WriteLine("情况,抽数,抽取时间");
            foreach (var mem in mems) writer.WriteLine($"{mem.name},{mem.count},{mem.time}");
        }

        public static List<Mem> CSVReader(string path)
        {
            List<Mem> result = new();
            if (!File.Exists(path)) return result;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 3 && int.TryParse(parts[1], out int count))
                {
                    result.Add(new Mem { name = parts[0], count = count, time = parts[2] });
                }
            }
            return result;
        }

        /// <summary>
        /// 导入设置内容
        /// </summary>
        /// <returns>设置项</returns>
        public static SettingsData LoadSetting()
        {
            string settingFilePath = SettingPage.settingFilePath;
            if (!File.Exists(settingFilePath))
                return ResetSetting(settingFilePath);
            else
            {
                var settingItem = JsonSerializer.Deserialize<SettingsData>
                    (File.ReadAllText(settingFilePath));
                if (settingItem == null)
                {
                    log.Warn("setting.json文件为空，重置设置");
                    return ResetSetting(settingFilePath);
                }
                else return settingItem;
            }
        }

        public static SettingsData ResetSetting(string path)
        {
            var defaultSettings = new SettingsData
            {
                language = 0,
                theme = 0,
                GenshinPath = null,
                HoukaiStarRailPath = null,
                ZZZPath = null
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(defaultSettings, options);
            File.WriteAllText(path, jsonString);
            return defaultSettings;
        }

        public static async Task LoadGamePath(string game, SettingsData settings)
        {
            string gameExe = game switch
            {
                "Genshin" => "Yuanshen.exe",
                "HoukaiStarRail" => "StarRail.exe",
                "ZZZ" => "ZenlessZoneZero.exe",
                _ => string.Empty
            };
            await MessageBox.Info(string.Format(loader.GetString("SettingControl/SelectGame"), gameExe));

            string? filePath = await Setting.GetFilePath(".exe");

            if (filePath == null)
            {
                await MessageBox.GamePathIsNull(gameExe);
                return;
            }

            if (filePath != settings.GenshinPath && game == "Genshin")
            {
                settings.GenshinPath = filePath;

                string newJson = JsonSerializer.Serialize(settings,
                            new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(SettingPage.settingFilePath, newJson);
            }
            else if (filePath != settings.HoukaiStarRailPath && game == "HoukaiStarRail")
            {
                settings.HoukaiStarRailPath = filePath;

                string newJson = JsonSerializer.Serialize(settings,
                            new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(SettingPage.settingFilePath, newJson);
            }
            else if (filePath != settings.ZZZPath && game == "ZZZ")
            {
                settings.ZZZPath = filePath;

                string newJson = JsonSerializer.Serialize(settings,
                            new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(SettingPage.settingFilePath, newJson);
            }
        }
    }
}
