namespace RestaurantMaui;

public static class AppTheme
{
    public static Color Primary => Color.FromArgb("#FF5722");
    public static Color PrimaryLight => Color.FromArgb("#FF8A65");
    public static Color PrimaryDark => Color.FromArgb("#C62828");
    public static Color Background => Color.FromArgb("#F5F5F5");
    public static Color Surface => Colors.White;
    public static Color TextPrimary => Color.FromArgb("#212121");
    public static Color TextSecondary => Color.FromArgb("#757575");
    public static Color Success => Color.FromArgb("#388E3C");
    public static Color Warning => Color.FromArgb("#F57C00");
    public static Color Danger => Color.FromArgb("#D32F2F");
    public static Color Info => Color.FromArgb("#0288D1");

    public static Color StatusColor(string? status) => status switch
    {
        "Pending" => Warning,
        "Accepted" => Info,
        "Preparing" => Color.FromArgb("#7B1FA2"),
        "ReadyForPickup" => Color.FromArgb("#00796B"),
        "OnTheWay" => Primary,
        "Delivered" => Success,
        "Cancelled" => Danger,
        "Rejected" => Danger,
        _ => TextSecondary,
    };

    public static string StatusArabic(string? status) => status switch
    {
        "Pending" => "⏳ انتظار",
        "Accepted" => "✅ مقبول",
        "Preparing" => "👨‍🍳 جاري التحضير",
        "ReadyForPickup" => "📦 جاهز للتسليم",
        "OnTheWay" => "🛵 في الطريق",
        "Delivered" => "✔️ تم التسليم",
        "Cancelled" => "❌ ملغي",
        "Rejected" => "🚫 مرفوض",
        _ => status ?? "",
    };

    public static string[] NextStatuses(string? current) => current switch
    {
        "Pending" => ["Accepted", "Rejected"],
        "Accepted" => ["Preparing"],
        "Preparing" => ["ReadyForPickup"],
        "ReadyForPickup" => ["OnTheWay"],
        "OnTheWay" => ["Delivered"],
        _ => [],
    };
}