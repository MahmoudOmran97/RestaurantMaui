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
    [ObservableProperty] string restaurantIdText = AppConfig.RestaurantId.ToString();
    [ObservableProperty] string errorMessage = string.Empty;
    [ObservableProperty] bool isBusy = false;

    public LoginViewModel(ApiService api) => _api = api;

    partial void OnRestaurantIdTextChanged(string value)
    {
        if (int.TryParse(value, out var id) && id > 0)
            AppConfig.RestaurantId = id;
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "برجاء إدخال البريد الإلكتروني وكلمة المرور";
            return;
        }

        if (!int.TryParse(RestaurantIdText, out var restId) || restId <= 0)
        {
            ErrorMessage = "برجاء إدخال رقم المطعم بشكل صحيح";
            return;
        }

        // تحديث رابط الـ API لو تغيّر
        if (!string.IsNullOrWhiteSpace(ApiUrl))
            AppConfig.BaseUrl = ApiUrl.TrimEnd('/');

        AppConfig.RestaurantId = restId;

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
        AppSession.RestaurantId = restId;

        // الانتقال للـ Shell الرئيسي
        Application.Current!.Windows[0].Page = new AppShell();
    }
}