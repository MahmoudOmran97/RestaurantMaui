using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.ConnectHubAsync();
        await _vm.LoadAsync();
    }
}