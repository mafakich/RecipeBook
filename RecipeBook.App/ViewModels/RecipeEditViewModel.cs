using RecipeBook.App.MVVM;
using RecipeBook.App.Services;
using RecipeBook.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RecipeBook.App.ViewModels
{
    public class RecipeEditViewModel : ViewModelBase
    {
        private readonly RecipeService _service;
        private string _title = string.Empty;
        private string _desc = string.Empty;
        private int _catId;
        private int _cookTime;
        private Difficulty _diff;
        private ObservableCollection<Ingredient> _selectedIngredients = new();
        private ObservableCollection<RecipeStep> _steps = new();
        private ObservableCollection<Category> _categories = new();
        private ObservableCollection<Ingredient> _allIngredients = new();
        private Ingredient? _chosenIngredient;

        public RecipeEditViewModel(RecipeService service, Recipe? existing)
        {
            _service = service;
            SaveCommand = new RelayCommand(_ => SaveAsync(), _ => Validate());
            AddIngredientCommand = new RelayCommand(_ => AddIngredient(), _ => _chosenIngredient != null && !_selectedIngredients.Contains(_chosenIngredient!));
            RemoveIngredientCommand = new RelayCommand(_ => RemoveIngredient(), _ => _selectedIngredients.Count > 0);
            AddStepCommand = new RelayCommand(_ => AddStep());
            RemoveStepCommand = new RelayCommand(_ => RemoveStep(), _ => _steps.Count > 1);

            if (existing != null)
            {
                Title = existing.Title;
                Desc = existing.Description;
                _catId = existing.CategoryId;
                CookTime = existing.CookTimeMinutes;
                _diff = existing.Difficulty;
                foreach (var i in existing.Ingredients)
                    _selectedIngredients.Add(i);
                foreach (var s in existing.Steps.OrderBy(x => x.StepNumber))
                    _steps.Add(s);
            }
            else
            {
                _steps.Add(new RecipeStep { StepNumber = 1, Text = string.Empty });
            }
            LoadLookupsAsync();
        }

        public string Title { get => _title; set => SetField(ref _title, value); }
        public string Desc { get => _desc; set => SetField(ref _desc, value); }
        public int CategoryId { get => _catId; set => SetField(ref _catId, value); }
        public int CookTime { get => _cookTime; set => SetField(ref _cookTime, value); }
        public Difficulty Difficulty { get => _diff; set => SetField(ref _diff, value); }
        public ObservableCollection<Ingredient> SelectedIngredients { get => _selectedIngredients; }
        public ObservableCollection<RecipeStep> Steps { get => _steps; }
        public ObservableCollection<Category> Categories { get => _categories; }
        public ObservableCollection<Ingredient> AllIngredients { get => _allIngredients; }
        public Ingredient? ChosenIngredient { get => _chosenIngredient; set => SetField(ref _chosenIngredient, value); }

        public RelayCommand SaveCommand { get; }
        public RelayCommand AddIngredientCommand { get; }
        public RelayCommand RemoveIngredientCommand { get; }
        public RelayCommand AddStepCommand { get; }
        public RelayCommand RemoveStepCommand { get; }

        private async void LoadLookupsAsync()
        {
            var cats = await _service.GetCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats)
                Categories.Add(c);
            var ings = await _service.GetAllIngredientsAsync();
            AllIngredients.Clear();
            foreach (var i in ings)
                AllIngredients.Add(i);
        }

        private void AddIngredient()
        {
            if (ChosenIngredient != null)
                SelectedIngredients.Add(ChosenIngredient);
        }

        private void RemoveIngredient()
        {
            if (SelectedIngredients.Count > 0)
                SelectedIngredients.RemoveAt(SelectedIngredients.Count - 1);
        }

        private void AddStep()
        {
            Steps.Add(new RecipeStep { StepNumber = Steps.Count + 1, Text = string.Empty });
            RecalculateSteps();
        }

        private void RemoveStep()
        {
            if (Steps.Count > 1)
                Steps.RemoveAt(Steps.Count - 1);
            RecalculateSteps();
        }

        private void RecalculateSteps()
        {
            for (int i = 0; i < Steps.Count; i++)
                Steps[i].StepNumber = i + 1;
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Название обязательно");
                return false;
            }
            if (SelectedIngredients.Count == 0)
            {
                MessageBox.Show("Добавьте минимум 1 ингредиент");
                return false;
            }
            if (Steps.Count == 0 || Steps.Any(s => string.IsNullOrWhiteSpace(s.Text)))
            {
                MessageBox.Show("Заполните минимум 1 шаг приготовления");
                return false;
            }
            return true;
        }

        private async void SaveAsync()
        {
            var recipe = new Recipe
            {
                Id = (_steps.Count > 0 && _steps.First().RecipeId != 0) ? _steps.First().RecipeId : 0,
                Title = Title,
                Description = Desc,
                CategoryId = CategoryId,
                CookTimeMinutes = CookTime,
                Difficulty = Difficulty,
                CreatedAt = DateTime.UtcNow
            };
            foreach (var i in SelectedIngredients)
                recipe.Ingredients.Add(i);
            foreach (var s in Steps)
            {
                s.RecipeId = recipe.Id;
                recipe.Steps.Add(s);
            }
            await _service.SaveRecipeAsync(recipe);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive);
                if (window != null)
                    window.DialogResult = true;
            });
        }
    }
}