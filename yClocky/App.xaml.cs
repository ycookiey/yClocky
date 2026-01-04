using System.Configuration;
using System.Data;
using System.Windows;
using System.IO;

namespace yClocky;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Global exception handling
        AppDomain.CurrentDomain.UnhandledException += (s, args) => LogException(args.ExceptionObject as Exception);
        DispatcherUnhandledException += (s, args) =>
        {
            LogException(args.Exception);
            args.Handled = true;
        };

        SettingsManager.Load();

        // コマンドライン引数をチェック
        if (e.Args.Length > 0)
        {
            if (e.Args.Contains("--reset-position") || e.Args.Contains("-rp"))
            {
                SettingsManager.ResetPosition();
                // 位置をリセット後、アプリは継続実行される
            }
        }

        if (!SettingsManager.Current.AllowMultipleInstances)
        {
            const string appName = "yClocky_SingleInstance_Mutex";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // App is already running!
                Shutdown();
                return;
            }
        }

        base.OnStartup(e);
    }

    private void LogException(Exception? ex)
    {
        if (ex == null) return;
        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "yClocky", "crash.log");
            
            // Ensure directory exists
            var dir = Path.GetDirectoryName(logPath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.AppendAllText(logPath, 
                $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n");
        }
        catch { }
    }
}
