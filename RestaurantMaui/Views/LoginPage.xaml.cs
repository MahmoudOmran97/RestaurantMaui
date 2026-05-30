using RestaurantMaui.ViewModels;

namespace RestaurantMaui.Views;

public partial class LoginPage : ContentPage
{
    // FIX: يقبل LoginViewModel من DI مباشرة
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
