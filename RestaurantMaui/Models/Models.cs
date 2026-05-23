using Newtonsoft.Json;

namespace RestaurantMaui.Models;

// ─── Auth ────────────────────────────────────────────────────────────────────

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// ─── Restaurant ───────────────────────────────────────────────────────────────

public class RestaurantDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("description")] public string? Description { get; set; }
    [JsonProperty("address")] public string Address { get; set; } = string.Empty;
    [JsonProperty("latitude")] public double Latitude { get; set; }
    [JsonProperty("longitude")] public double Longitude { get; set; }
    [JsonProperty("imageUrl")] public string? ImageUrl { get; set; }
    [JsonProperty("coverImageUrl")] public string? CoverImageUrl { get; set; }
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("rating")] public double Rating { get; set; }
    [JsonProperty("totalRatings")] public int TotalRatings { get; set; }
    [JsonProperty("deliveryFee")] public decimal DeliveryFee { get; set; }
    [JsonProperty("minOrderAmount")] public decimal MinOrderAmount { get; set; }
    [JsonProperty("estimatedTime")] public int EstimatedTime { get; set; }
    [JsonProperty("isOpen")] public bool IsOpen { get; set; }

    public string? FullImageUrl => BuildUrl(ImageUrl);
    public string? FullCoverImageUrl => BuildUrl(CoverImageUrl);

    private static string? BuildUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (url.StartsWith("http")) return url;
        return AppConfig.BaseUrl.TrimEnd('/') + "/" + url.TrimStart('/');
    }
}

public class PagedResult<T> where T : class
{
    [JsonProperty("total")] public int Total { get; set; }
    [JsonProperty("page")] public int Page { get; set; }
    [JsonProperty("pageSize")] public int PageSize { get; set; }
    [JsonProperty("totalPages")] public int TotalPages { get; set; }
    [JsonProperty("data")] public List<T> Data { get; set; } = [];
}

// ─── Category & Product ───────────────────────────────────────────────────────

public class CategoryDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("imageUrl")] public string? ImageUrl { get; set; }
    [JsonProperty("sortOrder")] public int SortOrder { get; set; }
    [JsonProperty("products")] public List<ProductDto> Products { get; set; } = [];
}

public class ProductDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("description")] public string? Description { get; set; }
    [JsonProperty("price")] public decimal Price { get; set; }
    [JsonProperty("discountedPrice")] public decimal DiscountedPrice { get; set; }
    [JsonProperty("imageUrl")] public string? ImageUrl { get; set; }
    [JsonProperty("preparationTime")] public int PreparationTime { get; set; }
    [JsonProperty("calories")] public int Calories { get; set; }
    [JsonProperty("isAvailable")] public bool IsAvailable { get; set; }
    [JsonProperty("categoryName")] public string? CategoryName { get; set; }
    [JsonProperty("categoryId")] public int CategoryId { get; set; }

    public decimal EffectivePrice =>
        DiscountedPrice > 0 && DiscountedPrice < Price ? DiscountedPrice : Price;

    public string AvailabilityText => IsAvailable ? "✅ متاح" : "❌ غير متاح";
    public Color AvailabilityColor => IsAvailable ? AppTheme.Success : AppTheme.Danger;
}

public class CreateProductRequest
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
    public int PreparationTime { get; set; } = 15;
    public int Calories { get; set; }
}

// ─── Orders ───────────────────────────────────────────────────────────────────

public class OrderDetail
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("status")] public string Status { get; set; } = string.Empty;
    [JsonProperty("subTotal")] public decimal SubTotal { get; set; }
    [JsonProperty("deliveryFee")] public decimal DeliveryFee { get; set; }
    [JsonProperty("discount")] public decimal Discount { get; set; }
    [JsonProperty("totalAmount")] public decimal TotalAmount { get; set; }
    [JsonProperty("paymentMethod")] public string PaymentMethod { get; set; } = string.Empty;
    [JsonProperty("paymentStatus")] public string PaymentStatus { get; set; } = string.Empty;
    [JsonProperty("deliveryAddress")] public string DeliveryAddress { get; set; } = string.Empty;
    [JsonProperty("deliveryNotes")] public string? DeliveryNotes { get; set; }
    [JsonProperty("estimatedDelivery")] public int EstimatedDelivery { get; set; }
    [JsonProperty("cancellationReason")] public string? CancellationReason { get; set; }
    [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
    [JsonProperty("acceptedAt")] public DateTime AcceptedAt { get; set; }
    [JsonProperty("pickedUpAt")] public DateTime PickedUpAt { get; set; }
    [JsonProperty("deliveredAt")] public DateTime DeliveredAt { get; set; }
    [JsonProperty("restaurant")] public OrderRestaurantInfo? Restaurant { get; set; }
    [JsonProperty("items")] public List<OrderItemDto> Items { get; set; } = [];
    [JsonProperty("customerName")] public string CustomerName { get; set; } = string.Empty;
    [JsonProperty("customerPhone")] public string CustomerPhone { get; set; } = string.Empty;

    // Helper properties for UI
    public string StatusArabic => AppTheme.StatusArabic(Status);
    public Color StatusColor => AppTheme.StatusColor(Status);
    public string TotalFormatted => $"{TotalAmount:F0} EGP";
    public string CreatedFormatted => CreatedAt.ToLocalTime().ToString("hh:mm tt  dd/MM/yyyy");
    public string[] NextStatuses => AppTheme.NextStatuses(Status);
    public bool CanChangeStatus => NextStatuses.Length > 0;
}

public class OrderRestaurantInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Phone { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public string PriceText => $"{UnitPrice:F0} × {Quantity} = {TotalPrice:F0} EGP";
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

// ─── Update Restaurant ────────────────────────────────────────────────────────

public class UpdateRestaurantDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public decimal MinOrderAmount { get; set; }
    public int EstimatedTime { get; set; }
    public bool IsOpen { get; set; }
    public string? ImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
}

// ─── Dashboard Stats ──────────────────────────────────────────────────────────

public class DashboardStats
{
    public int TodayOrders { get; set; }
    public decimal TodayRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int PreparingOrders { get; set; }
    public int TotalProducts { get; set; }
    public double AvgRating { get; set; }
}

// ─── Results ─────────────────────────────────────────────────────────────────

public class LoginResult
{
    public bool Ok { get; set; }
    public LoginResponse? Data { get; set; }
    public string Error { get; set; } = string.Empty;
}

public class ApiResult
{
    public bool Ok { get; set; }
    public string Error { get; set; } = string.Empty;
}
// قسم مبسّط للعرض في قائمة الأقسام
public class CategorySimpleDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("imageUrl")] public string? ImageUrl { get; set; }
    [JsonProperty("sortOrder")] public int SortOrder { get; set; }
    [JsonProperty("productCount")] public int ProductCount { get; set; }
}

// لإضافة قسم جديد
public class CreateCategoryRequest
{
    public int RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

// لتعديل قسم
public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}