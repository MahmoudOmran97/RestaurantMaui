using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace RestaurantMaui.ViewModels;

[QueryProperty(nameof(IsEdit), "IsEdit")]
[QueryProperty(nameof(Product), "Product")]
[QueryProperty(nameof(Categories), "Categories")]
public partial class ProductFormViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] bool isEdit = false;
    [ObservableProperty] ProductDto? product;
    [ObservableProperty] List<CategoryDto>? categories;
    [ObservableProperty] bool isBusy = false;

    // Form Fields
    [ObservableProperty] string name = string.Empty;
    [ObservableProperty] string description = string.Empty;
    [ObservableProperty] string price = "0";
    [ObservableProperty] string discountedPrice = "0";
    [ObservableProperty] string imageUrl = string.Empty;
    [ObservableProperty] string preparationTime = "15";
    [ObservableProperty] string calories = "0";
    [ObservableProperty] CategoryDto? selectedCategory;

    public ObservableCollection<CategoryDto> CategoryItems { get; } = [];

    public string PageTitle => IsEdit ? "تعديل المنتج" : "إضافة منتج جديد";

    public ProductFormViewModel(ApiService api) => _api = api;

    partial void OnCategoriesChanged(List<CategoryDto>? value)
    {
        CategoryItems.Clear();
        if (value is null) return;
        foreach (var c in value) CategoryItems.Add(c);
    }

    partial void OnProductChanged(ProductDto? value)
    {
        if (value is null) return;
        Name = value.Name;
        Description = value.Description ?? string.Empty;
        Price = value.Price.ToString("F0");
        DiscountedPrice = value.DiscountedPrice.ToString("F0");
        ImageUrl = value.ImageUrl ?? string.Empty;
        PreparationTime = value.PreparationTime.ToString();
        Calories = value.Calories.ToString();
        SelectedCategory = CategoryItems.FirstOrDefault(c => c.Id == value.CategoryId);
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("خطأ", "برجاء إدخال اسم المنتج", "حسناً");
            return;
        }
        if (SelectedCategory is null)
        {
            await Shell.Current.DisplayAlert("خطأ", "برجاء اختيار الفئة", "حسناً");
            return;
        }

        var req = new CreateProductRequest
        {
            CategoryId = SelectedCategory.Id,
            Name = Name,
            Description = Description,
            Price = decimal.TryParse(Price, out var p) ? p : 0,
            DiscountedPrice = decimal.TryParse(DiscountedPrice, out var dp) ? dp : 0,
            ImageUrl = ImageUrl,
            PreparationTime = int.TryParse(PreparationTime, out var pt) ? pt : 15,
            Calories = int.TryParse(Calories, out var cal) ? cal : 0,
        };

        IsBusy = true;
        ApiResult result;
        if (IsEdit && Product is not null)
            result = await _api.UpdateProductAsync(Product.Id, req);
        else
            result = await _api.CreateProductAsync(req);
        IsBusy = false;

        if (result.Ok)
        {
            await Shell.Current.DisplayAlert("✅ نجاح", IsEdit ? "تم تعديل المنتج" : "تمت إضافة المنتج", "حسناً");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("❌ خطأ", result.Error, "حسناً");
        }
    }

    [RelayCommand]
    async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}