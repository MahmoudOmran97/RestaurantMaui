using RestaurantMaui.Views;

namespace RestaurantMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // تسجيل الصفحات التي تُفتح بالـ Navigation (مش Tabs)
        Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
        Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
    }
}