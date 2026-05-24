using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.ObjectModel;

namespace RestaurantMaui.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] bool isBusy = false;
    [ObservableProperty] string categoriesCount = "0 قسم";
    [ObservableProperty] CategorySimpleDto? selectedCategory;

    public ObservableCollection<CategorySimpleDto> Categories { get; } = [];

    public CategoriesViewModel(ApiService api) => _api = api;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var cats = await _api.GetCategoriesAsync(AppSession.RestaurantId);
            Categories.Clear();
            foreach (var c in cats ?? [])
                Categories.Add(c);

            CategoriesCount = $"{Categories.Count} قسم";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    void SelectCategory(CategorySimpleDto category)
    {
        SelectedCategory = SelectedCategory?.Id == category.Id ? null : category;
    }

    [RelayCommand]
    async Task AddCategoryAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.CategoryFormPage));
    }

    [RelayCommand]
    async Task EditCategoryAsync()
    {
        if (SelectedCategory is null) return;
        var param = new Dictionary<string, object>
        {
            ["EditCategory"] = SelectedCategory
        };
        await Shell.Current.GoToAsync(nameof(Views.CategoryFormPage), param);
    }

    [RelayCommand]
    async Task DeleteCategoryAsync()
    {
        if (SelectedCategory is null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "حذف القسم",
            $"هل تريد حذف قسم \"{SelectedCategory.Name}\"؟\nسيتم حذف جميع منتجاته أيضاً.",
            "نعم، احذف", "إلغاء");

        if (!confirm) return;

        IsBusy = true;
        var result = await _api.DeleteCategoryAsync(SelectedCategory.Id);
        IsBusy = false;

        if (result.Ok)
        {
            SelectedCategory = null;
            await LoadAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("خطأ", result.Error, "حسناً");
        }
    }
}