using System.Windows;
using RecipeBook.App.ViewModels;

namespace RecipeBook.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var service = AppBootstrapper.GetService();
        var main = new MainWindow { DataContext = new MainViewModel(service) };
        main.Show();
    }
}