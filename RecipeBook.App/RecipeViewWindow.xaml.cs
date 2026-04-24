using System.Windows;

namespace RecipeBook.App;

public partial class RecipeViewWindow : Window
{
    public RecipeViewWindow() => InitializeComponent();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}