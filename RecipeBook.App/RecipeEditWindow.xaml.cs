using System.Windows;

namespace RecipeBook.App;

public partial class RecipeEditWindow : Window
{
    public RecipeEditWindow() => InitializeComponent();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}