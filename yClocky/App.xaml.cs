using System.Configuration;
using System.Data;
using System.Windows;

namespace yClocky;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        SettingsManager.Load();

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
}

