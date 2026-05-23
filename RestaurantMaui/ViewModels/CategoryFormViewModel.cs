using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.ObjectModel;
using System.Xml.Linq;


namespace RestaurantMaui.ViewModels;

[QueryProperty(nameof(EditCategory), "EditCategory")]
public partial class CategoryFormViewModel : ObservableObject
{
    private readonly ApiService _api;

    // وضع التعديل — بيتعبّى لو جاي من زرار تعديل
    [ObservableProperty] CategorySimpleDto? editCategory;

    // حقول الفورم
    [ObservableProperty] string name = string.Empty;
    [ObservableProperty] string imageUrl = string.Empty;
    [ObservableProperty] string sortOrder = "1";
    [ObservableProperty] bool isBusy = false;
    [ObservableProperty] string errorMsg = string.Empty;

    public bool IsEdit => EditCategory is not null;
    public string PageTitle => IsEdit ? "تعديل القسم" : "إضافة قسم جديد";

    public CategoryFormViewModel(ApiService api) => _api = api;

    // لما بتيجي بيانات التعديل ملاها في الفورم
    partial void OnEditCategoryChanged(CategorySimpleDto? value)
    {
        if (value is null) return;
        Name = value.Name;
        ImageUrl = value.ImageUrl ?? string.Empty;
        SortOrder = value.SortOrder.ToString();
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        ErrorMsg = string.Empty;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMsg = "برجاء إدخال اسم القسم";
            return;
        }

        IsBusy = true;
        ApiResult result;

        if (IsEdit && EditCategory is not null)
        {
            var req = new UpdateCategoryRequest
            {
                Name = Name.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl.Trim(),
                SortOrder = int.TryParse(SortOrder, out var so) ? so : EditCategory.SortOrder
            };
            result = await _api.UpdateCategoryAsync(EditCategory.Id, req);
        }
        else
        {
            var req = new CreateCategoryRequest
            {
                RestaurantId = AppSession.RestaurantId,
                Name = Name.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl.Trim()
            };
            result = await _api.CreateCategoryAsync(req);
        }

        IsBusy = false;

        if (result.Ok)
        {
            await Shell.Current.DisplayAlert(
                "✅ نجاح",
                IsEdit ? "تم تعديل القسم بنجاح" : "تمت إضافة القسم بنجاح",
                "حسناً");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            ErrorMsg = result.Error;
        }
    }

    [RelayCommand]
    async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}