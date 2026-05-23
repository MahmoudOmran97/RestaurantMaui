namespace RestaurantMaui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new NavigationPage(new Views.LoginPage()));

#if WINDOWS || MACCATALYST
        // Desktop: ابدأ بحجم مناسب
        window.Width = 1280;
        window.Height = 800;
        window.MinimumWidth = 900;
        window.MinimumHeight = 600;
#endif
        return window;
    }
}