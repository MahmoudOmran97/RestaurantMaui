using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;

namespace RestaurantMaui.ViewModels;

[QueryProperty(nameof(Order), "Order")]
public partial class OrderDetailViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] OrderDetail? order;
    [ObservableProperty] bool isBusy = false;

    public OrderDetailViewModel(ApiService api) => _api = api;

    [RelayCommand]
    async Task ChangeStatusAsync(string newStatus)
    {
        if (Order is null) return;
        bool confirm = await Shell.Current.DisplayAlert(
            "تأكيد",
            $"تحويل الأوردر #{Order.Id} إلى {AppTheme.StatusArabic(newStatus)}؟",
            "نعم", "لا");
        if (!confirm) return;

        IsBusy = true;
        var result = await _api.UpdateOrderStatusAsync(Order.Id, newStatus);
        IsBusy = false;

        if (result.Ok)
        {
            await Shell.Current.DisplayAlert("✅ نجاح", "تم تحديث الحالة بنجاح", "حسناً");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("❌ خطأ", result.Error, "حسناً");
        }
    }

    [RelayCommand]
    async Task GoBackAsync() => await Shell.Current.GoToAsync("..");
}