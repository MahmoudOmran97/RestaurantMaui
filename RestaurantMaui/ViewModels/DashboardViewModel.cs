using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.ObjectModel;

namespace RestaurantMaui.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly HubService _hub;

    [ObservableProperty] bool isBusy = false;
    [ObservableProperty] int todayOrders = 0;
    [ObservableProperty] string todayRevenue = "0 EGP";
    [ObservableProperty] int pendingOrders = 0;
    [ObservableProperty] int preparingOrders = 0;
    [ObservableProperty] string restaurantName = string.Empty;
    [ObservableProperty] string errorMessage = string.Empty;

    public ObservableCollection<OrderDetail> RecentOrders { get; } = [];

    public DashboardViewModel(ApiService api, HubService hub)
    {
        _api = api;
        _hub = hub;

        _hub.OnNewOrder += _ => MainThread.BeginInvokeOnMainThread(async () => await LoadAsync());
        _hub.OnOrderStatusChanged += (_, _) => MainThread.BeginInvokeOnMainThread(async () => await LoadAsync());
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var statsTask = _api.GetDashboardStatsAsync(AppSession.RestaurantId);
            var ordersTask = _api.GetRestaurantOrdersAsync(AppSession.RestaurantId, null, 1, 20);
            var restTask = _api.GetRestaurantAsync(AppSession.RestaurantId);

            // FIX: تشغيل الـ 3 requests بالتوازي بدل واحدة واحدة
            await Task.WhenAll(statsTask, ordersTask, restTask);

            var stats = await statsTask;
            var orders = await ordersTask;
            var rest = await restTask;

            TodayOrders = stats.TodayOrders;
            TodayRevenue = $"{stats.TodayRevenue:F0} EGP";
            PendingOrders = stats.PendingOrders;
            PreparingOrders = stats.PreparingOrders;
            RestaurantName = rest?.Name ?? string.Empty;

            RecentOrders.Clear();
            foreach (var o in orders) RecentOrders.Add(o);
        }
        catch (Exception ex)
        {
            ErrorMessage = "خطأ في تحميل البيانات: " + ex.Message;
        }
        finally { IsBusy = false; }
    }

    public async Task ConnectHubAsync() => await _hub.StartAsync();
}
