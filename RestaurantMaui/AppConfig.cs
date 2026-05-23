namespace RestaurantMaui;

public static class AppConfig
{
    private const string DefaultBaseUrl = "https://deliveryappapi.runasp.net";

    public static string BaseUrl
    {
        get => Preferences.Get("ApiBaseUrl", DefaultBaseUrl);
        set => Preferences.Set("ApiBaseUrl", value);
    }

    public static string ApiBaseUrl => $"{BaseUrl.TrimEnd('/')}/api";
    public static string HubUrl => $"{BaseUrl.TrimEnd('/')}/hubs/tracking";

    public static int RestaurantId
    {
        get => Preferences.Get("RestaurantId", 1);
        set => Preferences.Set("RestaurantId", value);
    }
}