using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class OrdersPage : ContentPage
{
    private readonly OrdersViewModel _vm;

    public OrdersPage(OrdersViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.RefreshAsync();
    }

    // فلتر الحالة من الـ Buttons
    private void OnStatusFilterClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _vm.SelectedStatus = btn.Text;
    }
}