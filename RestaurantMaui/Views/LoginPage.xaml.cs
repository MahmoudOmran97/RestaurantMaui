using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = App.Current!.Handler!.MauiContext!
            .Services.GetService<LoginViewModel>()
            ?? new LoginViewModel(new Services.ApiService());
    }
}