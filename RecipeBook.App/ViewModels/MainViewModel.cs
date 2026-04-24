using RecipeBook.App.MVVM;
using RecipeBook.App.Services;
using RecipeBook.Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace RecipeBook.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly RecipeService _service;
        private ObservableCollection<Recipe> _recipes = new();
        private string _search = string.Empty;
        private int? _catId;
        private int? _timeFilter;
        private ObservableCollection<Category> _categories = new();
        private Recipe? _selected;

        public MainViewModel(RecipeService service)
        {
            _service = service;
            LoadCommand = new RelayCommand(_ => LoadAsync());
            AddCommand = new RelayCommand(_ => OpenEditorAsync(null));
            EditCommand = new RelayCommand(_ => OpenEditorAsync(Selected), _ => Selected != null);
            DeleteCommand = new RelayCommand(_ => DeleteAsync(), _ => Selected != null);
            ViewCommand = new RelayCommand(_ => OpenViewerAsync(Selected), _ => Selected != null);
            ExportJsonCommand = new RelayCommand(_ => ExportAsync());

            LoadCategoriesAsync();
            LoadAsync();
        }

        public ObservableCollection<Recipe> Recipes { get => _recipes; set => SetField(ref _recipes, value); }
        public string SearchText { get => _search; set { if (SetField(ref _search, value)) LoadAsync(); } }
        public int? SelectedCategoryId { get => _catId; set { if (SetField(ref _catId, value)) LoadAsync(); } }
        public int? SelectedTimeFilter { get => _timeFilter; set { if (SetField(ref _timeFilter, value)) LoadAsync(); } }
        public ObservableCollection<Category> Categories { get => _categories; set => SetField(ref _categories, value); }
        public Recipe? Selected { get => _selected; set => SetField(ref _selected, value); }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ViewCommand { get; }
        public RelayCommand ExportJsonCommand { get; }

        private async void LoadCategoriesAsync()
        {
            var list = await _service.GetCategoriesAsync();
            Categories.Clear();
            foreach (var c in list) Categories.Add(c);
        }

        private async void LoadAsync()
        {
            var recipes = await _service.GetRecipesAsync(SearchText, SelectedCategoryId, SelectedTimeFilter);
            Recipes = new ObservableCollection<Recipe>(recipes);
        }

        private async void OpenEditorAsync(Recipe? r)
        {
            var vm = new RecipeEditViewModel(_service, r);
            var win = new RecipeEditWindow { DataContext = vm, Owner = Application.Current.MainWindow };
            if (win.ShowDialog() == true)
                LoadAsync();
        }

        private async void OpenViewerAsync(Recipe? r)
        {
            if (r == null) return;
            var vm = new RecipeViewViewModel(r);
            var win = new RecipeViewWindow { DataContext = vm, Owner = Application.Current.MainWindow };
            win.ShowDialog();
        }

        private async void DeleteAsync()
        {
            if (Selected != null && MessageBox.Show("Удалить рецепт?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _service.DeleteRecipeAsync(Selected.Id);
                LoadAsync();
            }
        }

        private async void ExportAsync()
        {
            var opts = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            File.WriteAllText("recipes_export.json", JsonSerializer.Serialize(Recipes, opts));
            MessageBox.Show("Экспорт завершён: recipes_export.json");
        }
    }
}