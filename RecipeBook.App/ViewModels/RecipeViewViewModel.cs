using RecipeBook.App.MVVM;
using RecipeBook.Core;
using System.Collections.ObjectModel;

namespace RecipeBook.App.ViewModels
{
    public class RecipeViewViewModel : ViewModelBase
    {
        private readonly Recipe _recipe;
        private string _title = string.Empty;
        private string _categoryName = string.Empty;
        private string _detailsText = string.Empty;
        private ObservableCollection<string> _ingredientsList = new();
        private ObservableCollection<string> _stepsList = new();

        public RecipeViewViewModel(Recipe recipe)
        {
            _recipe = recipe;
            Title = recipe.Title;
            CategoryName = recipe.Category?.Name ?? "Без категории";
            DetailsText = $"Время: {recipe.CookTimeMinutes} мин | Сложность: {recipe.Difficulty}";

            IngredientsList.Clear();
            foreach (var ing in recipe.Ingredients)
                IngredientsList.Add(ing.Name);

            StepsList.Clear();
            foreach (var step in recipe.Steps.OrderBy(s => s.StepNumber))
                StepsList.Add($"{step.StepNumber}. {step.Text}");
        }

        public string Title { get => _title; set => SetField(ref _title, value); }
        public string CategoryName { get => _categoryName; set => SetField(ref _categoryName, value); }
        public string DetailsText { get => _detailsText; set => SetField(ref _detailsText, value); }
        public ObservableCollection<string> IngredientsList { get => _ingredientsList; set => SetField(ref _ingredientsList, value); }
        public ObservableCollection<string> StepsList { get => _stepsList; set => SetField(ref _stepsList, value); }
    }
}