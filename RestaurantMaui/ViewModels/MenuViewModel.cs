using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.ObjectModel;

namespace RestaurantMaui.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly ApiService _api;
    private List<CategoryDto> _allCategories = [];

    [ObservableProperty] bool isBusy = false;
    [ObservableProperty] string selectedCategory = "الكل";
    [ObservableProperty] string productsCount = "0 منتج";
    [ObservableProperty] ProductDto? selectedProduct;

    public ObservableCollection<ProductDto> Products { get; } = [];
    public ObservableCollection<string> Categories { get; } = [];

    public MenuViewModel(ApiService api) => _api = api;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var cats = await _api.GetMenuAsync(AppSession.RestaurantId);
            _allCategories = cats ?? [];

            Categories.Clear();
            Categories.Add("الكل");
            foreach (var c in _allCategories) Categories.Add(c.Name);

            FilterProducts();
        }
        finally { IsBusy = false; }
    }

    partial void OnSelectedCategoryChanged(string value) => FilterProducts();

    private void FilterProducts()
    {
        Products.Clear();
        var cats = SelectedCategory == "الكل"
            ? _allCategories
            : _allCategories.Where(c => c.Name == SelectedCategory).ToList();

        foreach (var cat in cats)
            foreach (var p in cat.Products)
            {
                p.CategoryName = cat.Name;
                p.CategoryId = cat.Id;
                Products.Add(p);
            }

        ProductsCount = $"{Products.Count} منتج";
    }

    // ← command جديد للـ TapGestureRecognizer على أندرويد
    [RelayCommand]
    void SelectProduct(ProductDto product)
    {
        SelectedProduct = SelectedProduct?.Id == product.Id ? null : product;
    }

    [RelayCommand]
    async Task AddProductAsync()
    {
        var param = new Dictionary<string, object>
        {
            ["IsEdit"] = false,
            ["Categories"] = _allCategories
        };
        await Shell.Current.GoToAsync(nameof(Views.ProductFormPage), param);
    }

    [RelayCommand]
    async Task EditProductAsync()
    {
        if (SelectedProduct is null) return;
        var param = new Dictionary<string, object>
        {
            ["IsEdit"] = true,
            ["Product"] = SelectedProduct,
            ["Categories"] = _allCategories
        };
        await Shell.Current.GoToAsync(nameof(Views.ProductFormPage), param);
    }

    [RelayCommand]
    async Task ToggleAvailabilityAsync()
    {
        if (SelectedProduct is null) return;
        IsBusy = true;
        await _api.ToggleProductAvailabilityAsync(SelectedProduct.Id);
        IsBusy = false;
        await LoadAsync();
    }

    [RelayCommand]
    async Task DeleteProductAsync()
    {
        if (SelectedProduct is null) return;
        bool confirm = await Shell.Current.DisplayAlert(
            "حذف المنتج",
            $"هل تريد حذف \"{SelectedProduct.Name}\"؟",
            "نعم", "لا");
        if (!confirm) return;

        IsBusy = true;
        bool ok = await _api.DeleteProductAsync(SelectedProduct.Id);
        IsBusy = false;

        if (ok) await LoadAsync();
        else await Shell.Current.DisplayAlert("خطأ", "فشل حذف المنتج", "حسناً");
    }
}