using log4net.Config;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using MiHoyoGameGachaRecords.Assets.pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiHoyoGameGachaRecords
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        //private Window? _window;
        public static Type? LastPageType { get; set; }
        public static object? LastPageObject { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            try
            {
                var logsDir = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Logs");
                Directory.CreateDirectory(logsDir);

                var logFiles = Directory.GetFiles(logsDir, "logfile.log.*")
                    .Where(f => !f.EndsWith("all.log", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => File.GetCreationTimeUtc(f))
                    .ToList();

                if (logFiles.Count > 0)
                {
                    var newest = logFiles.OrderByDescending(f => File.GetCreationTimeUtc(f)).First();
                    var toMerge = logFiles.Where(f => !string.Equals(f, newest, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    var combinedPath = Path.Combine(logsDir, "all.log");

                    foreach (var file in toMerge)
                    {
                        try
                        {
                            // Append content to combined log
                            using (var read = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (var sr = new StreamReader(read))
                            using (var write = new FileStream(combinedPath, FileMode.Append, FileAccess.Write,
                                FileShare.Read))
                            using (var sw = new StreamWriter(write))
                            {
                                sw.Write(sr.ReadToEnd());
                            }

                            File.Delete(file);
                        }
                        catch
                        {
                            // 忽略报错
                        }
                    }
                }

                // 删日志
                try
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-1);
                    var allFiles = Directory.GetFiles(logsDir);
                    foreach (var file in allFiles)
                    {
                        try
                        {
                            if (string.Equals(Path.GetFileName(file), "all.log", StringComparison.OrdinalIgnoreCase))
                                continue;

                            var lastWrite = File.GetLastWriteTimeUtc(file);
                            if (lastWrite < cutoff)
                            {
                                File.Delete(file);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
            catch { }

            // Configure log4net to write into LocalFolder\Logs and ensure WARN+ are recorded
            try
            {
                var localLogsDir = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Logs");
                Directory.CreateDirectory(localLogsDir);

                var originalConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                if (File.Exists(originalConfigPath))
                {
                    var doc = System.Xml.Linq.XDocument.Load(originalConfigPath);

                    var fileElements = doc.Descendants().Where(e => e.Name.LocalName == "file").ToList();
                    foreach (var fe in fileElements)
                    {
                        fe.SetAttributeValue("value", Path.Combine(localLogsDir, "logfile.log"));
                    }

                    var levelElem = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "level" && e.Parent != null && e.Parent.Name.LocalName == "root");
                    levelElem?.SetAttributeValue("value", "WARN");  // 不为空执行

                    var runtimeConfigPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                        "log4net.runtime.config");
                    doc.Save(runtimeConfigPath);
                    var LogConfig = new FileInfo(runtimeConfigPath);
                    XmlConfigurator.Configure(LogConfig);
                }
                else
                {
                    var LogConfig = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "log4net.config"));
                    XmlConfigurator.Configure(LogConfig);
                }
            }
            catch { }

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //_window = new MainWindow();
            //_window.Activate();
            MainWindowInstance = new MainWindow();
            MainWindowInstance.Activate();
        }
        public static MainWindow? MainWindowInstance { get; private set; }
    }
}
