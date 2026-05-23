using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class OrderDetailPage : ContentPage
{
    public OrderDetailPage(OrderDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}