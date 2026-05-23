using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Data;


namespace RestaurantMaui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApiService _api;

    // Connection
    [ObservableProperty] string apiUrl = AppConfig.BaseUrl;
    [ObservableProperty] string connectionStatus = string.Empty;

    // Restaurant Info
    [ObservableProperty] string restName = string.Empty;
    [ObservableProperty] string restPhone = string.Empty;
    [ObservableProperty] string restAddress = string.Empty;
    [ObservableProperty] string restDesc = string.Empty;
    [ObservableProperty] string deliveryFee = "0";
    [ObservableProperty] string minOrder = "0";
    [ObservableProperty] string estimatedTime = "30";
    [ObservableProperty] bool isOpen = true;
    [ObservableProperty] string imageUrl = string.Empty;
    [ObservableProperty] string coverUrl = string.Empty;
    [ObservableProperty] string infoStatus = string.Empty;
    [ObservableProperty] bool isBusy = false;

    // Session info (read-only display)
    public string UserName => AppSession.FullName;
    public string UserEmail => AppSession.Email;
    public string UserRole => AppSession.Role;

    public SettingsViewModel(ApiService api) => _api = api;

    [RelayCommand]
    async Task TestConnectionAsync()
    {
        if (!string.IsNullOrWhiteSpace(ApiUrl))
            AppConfig.BaseUrl = ApiUrl.TrimEnd('/');

        IsBusy = true;
        ConnectionStatus = "جاري الاختبار...";
        try
        {
            var rest = await _api.GetRestaurantAsync(AppSession.RestaurantId);
            ConnectionStatus = rest != null ? "✅ الاتصال ناجح" : "❌ فشل الاتصال";
        }
        catch { ConnectionStatus = "❌ فشل الاتصال"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task LoadInfoAsync()
    {
        IsBusy = true;
        try
        {
            var rest = await _api.GetRestaurantAsync(AppSession.RestaurantId);
            if (rest is null) { InfoStatus = "❌ فشل التحميل"; return; }

            RestName = rest.Name;
            RestPhone = rest.Phone ?? string.Empty;
            RestAddress = rest.Address;
            RestDesc = rest.Description ?? string.Empty;
            DeliveryFee = rest.DeliveryFee.ToString("F0");
            MinOrder = rest.MinOrderAmount.ToString("F0");
            EstimatedTime = rest.EstimatedTime.ToString();
            IsOpen = rest.IsOpen;
            ImageUrl = rest.ImageUrl ?? string.Empty;
            CoverUrl = rest.CoverImageUrl ?? string.Empty;
            InfoStatus = "✅ تم التحميل";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    async Task SaveInfoAsync()
    {
        IsBusy = true;
        InfoStatus = "جاري الحفظ...";
        try
        {
            var dto = new UpdateRestaurantDto
            {
                Name = RestName,
                Phone = RestPhone,
                Address = RestAddress,
                Description = RestDesc,
                DeliveryFee = decimal.TryParse(DeliveryFee, out var df) ? df : 0,
                MinOrderAmount = decimal.TryParse(MinOrder, out var mo) ? mo : 0,
                EstimatedTime = int.TryParse(EstimatedTime, out var et) ? et : 30,
                IsOpen = IsOpen,
                ImageUrl = ImageUrl,
                CoverImageUrl = CoverUrl
            };

            var result = await _api.UpdateRestaurantAsync(AppSession.RestaurantId, dto);
            InfoStatus = result.Ok ? "✅ تم الحفظ بنجاح" : $"❌ {result.Error}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    async Task ToggleOpenAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        InfoStatus = "جاري التحديث...";
        try
        {
            var ok = await _api.ToggleRestaurantStatusAsync(AppSession.RestaurantId);
            if (ok)
            {
                IsOpen = !IsOpen;
                InfoStatus = IsOpen ? "✅ المطعم مفتوح الآن" : "✅ المطعم مغلق الآن";
            }
            else
            {
                InfoStatus = "❌ فشل تحديث الحالة";
            }
        }
        catch
        {
            InfoStatus = "❌ خطأ في الاتصال";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    void Logout()
    {
        AppSession.Clear();
        Application.Current!.Windows[0].Page = new NavigationPage(new Views.LoginPage());
    }
}