using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using RestaurantMaui.Services;
using RestaurantMaui.ViewModels;
using RestaurantMaui.Views;

namespace RestaurantMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseFFImageLoading()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Services (Singletons) ────────────────────────────────────────────────
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<HubService>();

        // ── ViewModels ───────────────────────────────────────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<OrdersViewModel>();
        builder.Services.AddTransient<MenuViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<OrderDetailViewModel>();
        builder.Services.AddTransient<ProductFormViewModel>();
        // FIX: كان ناقص تسجيل CategoryFormViewModel
        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<CategoryFormViewModel>();

        // ── Pages ────────────────────────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<OrdersPage>();
        builder.Services.AddTransient<MenuPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<OrderDetailPage>();
        builder.Services.AddTransient<ProductFormPage>();
        // FIX: كان ناقص تسجيل CategoriesPage و CategoryFormPage
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<CategoryFormPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
