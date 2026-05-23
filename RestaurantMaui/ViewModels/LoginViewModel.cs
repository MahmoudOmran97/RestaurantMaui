using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Services;

namespace RestaurantMaui.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] string email = string.Empty;
    [ObservableProperty] string password = string.Empty;
    [ObservableProperty] string apiUrl = AppConfig.BaseUrl;
    [ObservableProperty] string errorMessage = string.Empty;
    [ObservableProperty] bool isBusy = false;

    public LoginViewModel(ApiService api) => _api = api;

    [RelayCommand]
    async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "برجاء إدخال البريد الإلكتروني وكلمة المرور";
            return;
        }

        // تحديث رابط الـ API لو تغيّر
        if (!string.IsNullOrWhiteSpace(ApiUrl))
            AppConfig.BaseUrl = ApiUrl.TrimEnd('/');

        IsBusy = true;
        ErrorMessage = string.Empty;

        var result = await _api.LoginAsync(Email, Password);

        IsBusy = false;

        if (!result.Ok || result.Data is null)
        {
            ErrorMessage = result.Error;
            return;
        }

        // حفظ الجلسة
        AppSession.Token = result.Data.Token;
        AppSession.UserId = result.Data.Id;
        AppSession.FullName = result.Data.FullName;
        AppSession.Email = result.Data.Email;
        AppSession.Role = result.Data.Role;

        // جلب أول مطعم
        var restaurants = await _api.GetRestaurantsAsync(pageSize: 5);
        if (restaurants?.Data?.Count > 0)
            AppSession.RestaurantId = restaurants.Data[0].Id;

        // الانتقال للـ Shell الرئيسي
        Application.Current!.Windows[0].Page = new AppShell();
    }
}