using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantMaui.Models;
using RestaurantMaui.Services;
using System.Collections.ObjectModel;

namespace RestaurantMaui.ViewModels;

public partial class OrdersViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly HubService _hub;

    [ObservableProperty] bool isBusy = false;
    [ObservableProperty] string selectedStatus = "الكل";
    [ObservableProperty] string ordersCount = "0 أوردر";
    [ObservableProperty] OrderDetail? selectedOrder;

    public ObservableCollection<OrderDetail> Orders { get; } = [];

    public List<string> StatusOptions { get; } =
    [
        "الكل", "Pending", "Accepted", "Preparing",
        "ReadyForPickup", "OnTheWay", "Delivered", "Cancelled", "Rejected"
    ];

    public OrdersViewModel(ApiService api, HubService hub)
    {
        _api = api;
        _hub = hub;
        _hub.OnNewOrder += _ => MainThread.BeginInvokeOnMainThread(async () => await RefreshAsync());
        _hub.OnOrderStatusChanged += (_, _) => MainThread.BeginInvokeOnMainThread(async () => await RefreshAsync());
    }

    partial void OnSelectedStatusChanged(string value) =>
        MainThread.BeginInvokeOnMainThread(async () => await RefreshAsync());

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var status = SelectedStatus == "الكل" ? null : SelectedStatus;
            var orders = await _api.GetAllRestaurantOrdersAsync(AppSession.RestaurantId, status);

            Orders.Clear();
            foreach (var o in orders) Orders.Add(o);
            OrdersCount = $"{orders.Count} أوردر";
        }
        finally { IsBusy = false; }
    }

    // ← command جديد للـ TapGestureRecognizer على أندرويد
    [RelayCommand]
    void SelectOrder(OrderDetail order)
    {
        SelectedOrder = SelectedOrder?.Id == order.Id ? null : order;
    }

    [RelayCommand]
    async Task ChangeStatusAsync(string newStatus)
    {
        if (SelectedOrder is null) return;
        var result = await _api.UpdateOrderStatusAsync(SelectedOrder.Id, newStatus);
        if (result.Ok)
            await RefreshAsync();
        else
            await Shell.Current.DisplayAlert("خطأ", result.Error, "حسناً");
    }

    [RelayCommand]
    async Task ViewDetailsAsync()
    {
        if (SelectedOrder is null) return;
        var param = new Dictionary<string, object> { ["Order"] = SelectedOrder };
        await Shell.Current.GoToAsync(nameof(Views.OrderDetailPage), param);
    }
}