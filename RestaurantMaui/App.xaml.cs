namespace RestaurantMaui;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var loginPage = _services.GetRequiredService<Views.LoginPage>();
        var window = new Window(new NavigationPage(loginPage));

#if WINDOWS || MACCATALYST
        window.Width = 1280;
        window.Height = 800;
        window.MinimumWidth = 900;
        window.MinimumHeight = 600;
#endif
        return window;
    }

    // FIX: helper للـ logout يستخدم DI صح
    public static void NavigateToLogin()
    {
        if (Current is App app)
        {
            var loginPage = app._services.GetRequiredService<Views.LoginPage>();
            Current!.Windows[0].Page = new NavigationPage(loginPage);
        }
    }
}
