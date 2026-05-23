namespace RestaurantMaui;

public static class AppSession
{
    public static string Token { get; set; } = string.Empty;
    public static int UserId { get; set; }
    public static string FullName { get; set; } = string.Empty;
    public static string Email { get; set; } = string.Empty;
    public static string Role { get; set; } = string.Empty;
    public static int RestaurantId { get; set; }

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public static void Clear()
    {
        Token = string.Empty;
        UserId = 0;
        FullName = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        RestaurantId = 0;
    }
}