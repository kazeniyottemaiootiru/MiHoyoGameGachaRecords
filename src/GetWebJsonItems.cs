using log4net;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;

namespace MiHoyoGameGachaRecords
{
    internal class GetWebJsonItems(string gamePath)
    {
        string gamePath { get; } = gamePath;
        string game { get; } = Path.GetFileName(gamePath);
        private string Text { get; set; } = string.Empty;
        private SettingsData Settings { get; set; } = Setting.LoadSetting();

        private static readonly ILog log = LogManager.GetLogger(typeof(GetWebJsonItems));
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

        /// <summary>
        /// 获取是否连通并能获取数据的url
        /// </summary>
        /// <param name="gachaType">卡池情况。
        /// 原神：up角色：301，武器池：302，常驻：200，集录：500；
        /// 崩坏星穹铁道：up角色：11，光锥：12，常驻：1，联动：21；
        /// 绝区零：up角色：2001，音擎：3001，常驻：1001，邦布：5001</param>
        /// <returns>可用URL</returns>
        public async Task<List<Item>?> NeedJson(int gachaType, TextBlock textBlock, string time = "")
        {
            // up
            if (gachaType == 301 || gachaType == 11) Text = loader.GetString("Type/UP");
            else if (gachaType == 2001) Text = loader.GetString("Type/agent");
            // 常驻
            else if (gachaType == 200 || gachaType == 1 || gachaType == 1001)
                Text = loader.GetString("Type/permanent");
            // 武器
            else if (gachaType == 302) Text = loader.GetString("Type/weapon");
            else if (gachaType == 12) Text = loader.GetString("Type/lightCone");
            else if (gachaType == 3001) Text = loader.GetString("Type/WEngine");
            // 特殊
            else if (gachaType == 5001) Text = loader.GetString("Type/bangboo");
            else if (gachaType == 21) Text = loader.GetString("Type/collaboration");
            else if (gachaType == 500) Text = loader.GetString("Type/MixPool");

            GetHistoryFile getHistoryFile = new(gamePath);
            List<string>? urls = await getHistoryFile.ReadFile();

            if (urls == null) return null;
            else
            {
                List<Item>? items = null;
                Item? jsonInfo = null;

                using HttpClient client = new();

                for (int i = urls.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        var webInfo = await client.GetStringAsync(urls[i]);
                        bool flag = false;

                        jsonInfo = JsonSerializer.Deserialize<Item>(webInfo);
                        
                        if (jsonInfo != null)
                        {
                            if (jsonInfo.data == null)
                            {
                                await MessageBox.Info(loader.GetString("GetWebJson/OpenGame"));
                                return null;
                            }
                            string[] LangList = ["zh-cn", "zh-tw", "ja-jp", "en-us"];
                            if (jsonInfo.data.list[0].lang != LangList[Settings.language])
                            {
                                bool isContinue = await MessageBox.InfoYesOrNo(loader.GetString("GetWebJson/Lang"));
                                if (!isContinue) return null;
                            }
                            string uid = jsonInfo.data.list[0].uid;
                            flag = await MessageBox.InfoYesOrNo($"{loader.GetString("GetWebJsonItems/isYourUID")}UID：{uid}");
                        }
                        if (!flag) continue;

                        // 如果内容符合要求就返回
                        if (jsonInfo != null && jsonInfo.retcode == 0)
                        {
                            items = [];
                            string trueUrl = UrlSet(urls[i], "0", gachaType);  // 保证从头开始
                            jsonInfo = null;
                            jsonInfo = JsonSerializer.Deserialize<Item>(await client.GetStringAsync(trueUrl));
                            if (jsonInfo != null)
                                items.Add(jsonInfo);
                            string idTemp = "";

                            bool wTime = true;
                            int page = 1;
                            while (wTime)
                            {
                                if (jsonInfo?.data?.list == null || jsonInfo.data.list.Count == 0) break;
                                string end_id = jsonInfo.data.list[^1].id;    // 取最后一个ID

                                if (idTemp != end_id)
                                {
                                    idTemp = end_id;
                                    trueUrl = UrlSet(trueUrl, end_id, gachaType);
                                    jsonInfo = null;
                                    jsonInfo = JsonSerializer.Deserialize<Item>(await client.GetStringAsync(trueUrl));
                                    textBlock.Text = string.Format(loader.GetString("GetWebJson/Select"), Text, page);
                                    page++;
                                    Random rnd = new(); // 生成随机值，模拟点击
                                    Thread.Sleep(rnd.Next(500, 1000));  // 防止访问过快导致无法访问

                                    if (jsonInfo != null && !(jsonInfo?.data?.list == null 
                                        ||jsonInfo.data.list.Count == 0))
                                        items.Add(jsonInfo);
                                    if (time != "")
                                    {
                                        DateTime time1 = DateTime.Parse(time);
                                        DateTime time2 = DateTime.Parse(jsonInfo.data.list[^1].time);
                                        if (time1 > time2) wTime = false;
                                    }
                                }
                                else break;
                            }
                            break;
                        }

                        // 如果最后一个链接不可用
                        if (i == urls.Count - 1)
                        {
                            await MessageBox.Info(loader.GetString("GetWebJson/OpenGame"));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBox.Error(loader.GetString("GetWebJsonItems/failedConnect"));
                        log.Error($"获取数据失败，错误信息：{ex}");
                    }
                }
                return items;
            }
        }

        /// <summary>
        /// 用于修改URL方便查询
        /// </summary>
        /// <param name="urls">需要修改的URL</param>
        /// <param name="end_id">查询后的最后的end_id</param>
        /// <param name="gachaType">卡池情况。原神：up角色：301，武器池：302，常驻：200</param>
        /// <returns>修改后的URL</returns>
        private string UrlSet(string url, string end_id, int gachaType)
        {
            UriBuilder uri = new(url);
            // 星铁联动池特殊
            if(game == "StarRail.exe")
            {
                if(gachaType == 21)
                {
                    uri.Path = "common/gacha_record/api/getLdGachaLog";
                }
                else
                {
                    uri.Path = "common/gacha_record/api/getGachaLog";
                }
            }

            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            queryParams["gacha_type"] = gachaType.ToString();

            if (game == "ZenlessZoneZero.exe")
            {
                string realType = gachaType switch
                {
                    2001 => "2",    // 代理人
                    3001 => "3",    // 音擎
                    1001 => "1",    // 常驻
                    5001 => "5",    // 邦布
                    _ => ""
                };

                queryParams["real_gacha_type"] = realType;
            }

            if (queryParams.AllKeys.Contains("begin_id")) queryParams.Remove("begin_id");
            queryParams["end_id"] = end_id;
            uri.Query = queryParams.ToString();
            return uri.ToString();
        }

        public List<Mem> CountS(List<Item>? items)
        {
            string? name = "";
            int count = 0;
            string? time = items[0].data.list[0].time;

            List<Mem> mems = new List<Mem> { };

            foreach (var a in items)
            {
                foreach (var b in a.data.list)
                {
                    bool isGold = game == "ZenlessZoneZero.exe" ? b.rank_type == "4" : b.rank_type == "5";

                    if (isGold)
                    {
                        mems.Add(new Mem { name = name, time = time, count = count });
                        name = b.name;
                        time = b.time;
                        count = 1;
                    }
                    count++;
                }
            }
            mems.Add(new Mem { name = name, time = time, count = count });
            return mems;
        }

        ///<summary>创建CSV文件存储记录，并完成增量记录</summary>
        public async Task CreatCSVFile(List<Item>? items, int gachaType, TextBlock textBlock)
        {
            string dirPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "History");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string subDir = game switch
            {
                "YuanShen.exe" => "Genshin",
                "StarRail.exe" => "HoukaiStarRail",
                "ZenlessZoneZero.exe" => "ZZZ",
                _ => ""
            };
            if (string.IsNullOrEmpty(subDir)) return;

            string fileDirPath = "";
            fileDirPath = Path.Combine(dirPath, subDir);

            if (!Directory.Exists(fileDirPath)) Directory.CreateDirectory(fileDirPath);

            if (items == null || items.Count == 0 || items[0].data.list.Count == 0) return;
            fileDirPath = Path.Combine(fileDirPath, items[0].data.list[0].uid);
            if (!Directory.Exists(fileDirPath)) Directory.CreateDirectory(fileDirPath);

            string pool = gachaType switch
            {
                301 => "up",  // 原神，up
                11 => "up",  // 星铁，up
                2001 => "up",    //绝区零，up

                302 => "weapon",    // 原神，武器
                12 => "light_cone", // 星铁，光锥
                3001 => "W-Engine", // 绝区零，音擎

                200 => "permanent",  // 原神，常驻
                1 => "permanent",    // 星铁，常驻
                1001 => "permanent",  // 绝区零，常驻

                5001 => "bangboo",  // 绝区零，邦布
                21 => "liandong",     // 星铁，联动
                500 => "mix",   // 原神，混池
                _ => ""
            };

            string filePath = Path.Combine(fileDirPath, $"{pool}.csv");

            List<Mem> finalMems = [];
            if (!File.Exists(filePath)) { finalMems = CountS(items); }
            else
            {
                List<Mem> list = Setting.CSVReader(filePath);   // 获取文件
                if (list.Count == 0) finalMems = CountS(items);
                else
                {
                    string? name = "";
                    string? time = items[0].data.list[0].time;
                    if (list[0].name == "")
                    {
                        string? flag = list[0].time;
                        int newCount = 0;
                        bool isFinal = false;

                        foreach (var a in items)
                        {
                            foreach (var b in a.data.list)
                            {
                                if (b.time == flag)
                                {
                                    list[0].count += newCount;
                                    finalMems.Add(new Mem { name = name, time = time, count = list[0].count });
                                    finalMems.AddRange(list.Skip(1));
                                    isFinal = true;
                                    break;
                                }

                                bool isGold = game == "ZenlessZoneZero.exe" 
                                    ? b.rank_type == "4" 
                                    : b.rank_type == "5";

                                if (isGold)
                                {
                                    finalMems.Add(new Mem { name = name, time = time, count = newCount });
                                    name = b.name;
                                    time = b.time;
                                    newCount = 1;
                                }
                                newCount++;
                            }
                            if (isFinal) break;
                        }
                    }
                }
            }
            Setting.CSVWriter(finalMems, filePath);
            textBlock.Text = string.Format(loader.GetString("GetWebJson/finish"), Text);
        }
    }
}
