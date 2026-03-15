using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;

namespace MiHoyoGameGachaRecords
{
    
    class GetHistoryFile(string gamePath)
    {
        string gamePath = gamePath;
        private static readonly ILog log = LogManager.GetLogger(typeof(GetHistoryFile));
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

        /// <summary>
        /// 获取历史文件路径
        /// </summary>
        /// <returns>米哈游系列游戏的历史记录URL的文件路径</returns>
        private async Task<string?> HistoryFilePath()
        {
            string? dir = Path.GetDirectoryName(gamePath);
            string game = Path.GetFileName(gamePath);
            string? temp;
            //List<string>? webCachesDic = new List<string>();

            if (dir != null)
            {
                if (game == "YuanShen.exe")
                {
                    string webCache = Path.Combine(dir, "YuanShen_Data", "webCaches");
                    temp = await GetNeedDic(webCache);
                }
                else if (game == "")
                {
                    string webCache = Path.Combine(dir, "GenshinImpact_Data", "webCaches");
                    temp = await GetNeedDic(webCache);
                }
                else if (game == "StarRail.exe")
                {
                    string webCache = Path.Combine(dir, "StarRail_Data", "webCaches");
                    temp = await GetNeedDic(webCache);
                }
                else if (game == "ZenlessZoneZero.exe")
                {
                    string webCache = Path.Combine(dir, "ZenlessZoneZero_Data", "webCaches");
                    temp = await GetNeedDic(webCache);
                }
                else
                {
                    await MessageBox.Error(loader.GetString("GetHistoryFile/LoadGameFailed"));
                    log.Warn($"未知游戏或游戏版本，路径情况：{gamePath}");
                    return null;
                }

                if (temp == null)
                {
                    await MessageBox.Error(loader.GetString("GetHistoryFile/FilePathLoadFailed"));
                    log.Warn($"文件解析错误，为代码非支持的路径，传入文件路径{gamePath}");
                    return null;
                }
                else return temp;
            }
            else
            {
                await MessageBox.Error(loader.GetString("GetHistoryFile/notFound"));
                log.Error($"游戏安装目录解析失败，dir is null，路径情况：{gamePath}");
                return null;
            }
        }

        /// <summary>
        /// 获取最新的文件夹
        /// </summary>
        /// <param name="webCache">webCache的路径</param>
        /// <returns>最新的文件夹的路径并将其拼接</returns>
        private async Task<string?> GetNeedDic(string webCache)
        {
            List<string>? webCachesDic = new List<string>();
            string pattern = @"\d+.\d+.\d+.\d+";
            Version version;
            Version temp = new Version("0.0.0.0");
            foreach (string name in Directory.GetDirectories(webCache))
            {
                Match match = Regex.Match(name, pattern);
                if (match.Success)
                {
                    webCachesDic.Add(name);
                    string? foldName = GetFoldName(name);    // 获取文件夹名字
                    if (foldName == null)
                    {
                        await MessageBox.Error(loader.GetString("GetHistoryFile/failedToLoadFile"));  // 文件解析失败
                        return null;
                    }
                    else
                    {
                        version = new Version(foldName);
                        if (temp < version)
                        {
                            temp = version;
                        }
                    }
                }
            }

            string tempPath = Path.Combine(webCache, temp.ToString(), "Cache", "Cache_Data", "data_2");
            // 确认是否为文件
            if (File.Exists(tempPath)) return tempPath;
            else return null;
        }

        /// <summary>
        /// 获取完整路径中的文件夹名
        /// </summary>
        /// <param name="path">传入的路径</param>
        /// <returns>取出的文件名</returns>
        private static string? GetFoldName(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return directoryInfo.Name;
            }
            else
            {
                log.Error($"文件解析错误或为非文件夹，路径：{path}");
                return null;
            }
        }

        public async Task<List<string>?> ReadFile()
        {
            string? path = await HistoryFilePath();
            if (path == null) return null;
            byte[] bytes = [];
            try
            {
                bytes = File.ReadAllBytes(path); // 二进制文件
            }
            catch (IOException e)
            {
                await MessageBox.Error(loader.GetString("GetHistoryFile/failedToReadFile"));  // 读取文件失败
                log.Error($"文件读取失败，错误信息：{e}");
                return null;
            }
            catch(Exception e)
            {
                await MessageBox.Error(loader.GetString("GetHistory/LoadFileFailed"));
                log.Error($"未知错误在读文件处，错误信息：{e}");
                return null;
            }
            //StringBuilder sb = new StringBuilder();
            //List<string> result = new List<string>();
            List<string> urls = [];
            int flag = 0;
            string? temp = null;

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                
                if (b >= 32 && b <= 126)    // ascii
                {
                    temp += (char)b;
                }
                else if(b == 0 || b == 10 || b == 13)   // 控制符
                {
                        if (temp != null)
                            urls.Add(temp);
                        else continue;
                        flag++;
                        temp = null;
                }
            }

            // url清洗
            List<string> cleanUrls = [];
            string urlPattern = @"https://[^\s]+";

            foreach (string url in urls)
            {
                var a = Regex.Match(url, urlPattern);
                if (!(a.Value == null || a.Value == ""))
                    cleanUrls.Add(a.Value);
            }

            return cleanUrls;
        }
    }
}
