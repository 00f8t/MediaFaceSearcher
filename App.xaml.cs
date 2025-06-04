using MediaFaceSearcher.Views;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using MediaFaceSearcher.DAL;

namespace MediaFaceSearcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IPersonDao, PersonDao>();
        containerRegistry.Register<IDialogCoordinator, DialogCoordinator>();

        containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
    }
}

//TODO:
//1. TOOLTIPS
//2. SETTINGS
//3. MULTIPLE PHOTOS