using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using FlyleafLib;
using MediaFaceSearcher.ViewModels;
using MediaFaceSearcher.Views;

namespace MediaFaceSearcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Engine.Start(new EngineConfig()
        {
            FFmpegPath = @"C:\Flyleaf\FFmpeg",
            FFmpegDevices = false,    // Prevents loading avdevice/avfilter dll files. Enable it only if you plan to use dshow/gdigrab etc.

#if RELEASE
                FFmpegLogLevel      = Flyleaf.FFmpeg.LogLevel.Quiet,
                LogLevel            = LogLevel.Quiet,

#else
            FFmpegLogLevel = Flyleaf.FFmpeg.LogLevel.Warn,
            LogLevel = LogLevel.Debug,
            LogOutput = ":debug",
            //LogOutput         = ":console",
            //LogOutput         = @"C:\Flyleaf\Logs\flyleaf.log",                
#endif

            //PluginsPath       = @"C:\Flyleaf\Plugins",

            UIRefresh = false,    // Required for Activity, BufferedDuration, Stats in combination with Config.Player.Stats = true
            UIRefreshInterval = 250,      // How often (in ms) to notify the UI
            UICurTimePerSecond = true,     // Whether to notify UI for CurTime only when it's second changed or by UIRefreshInterval
        });
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }
}

//TODO: 
// 1. Опція точного пошуку (вище recognition threshold + перевірка по всім фото)
// 2. Список щойно знайдених людей на головній сторінці
// 3. Список ВСІХ людей на окремій сторінці
// 4. МОЖЛИВО Історія останніх медіа
// 5. Опція обведення обличчя