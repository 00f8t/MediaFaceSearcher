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
    }
}

//TODO: 
// 1. Опція точного пошуку (вище recognition threshold + перевірка по всім фото)
// 2. Список щойно знайдених людей на головній сторінці
// 3. Список ВСІХ людей на окремій сторінці
// 4. МОЖЛИВО Історія останніх медіа
// 5. Опція обведення обличчя