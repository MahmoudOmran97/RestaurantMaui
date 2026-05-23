using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using RestaurantMaui.Models;

namespace RestaurantMaui.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _http = new HttpClient(handler);
    }

    private void SetAuth()
    {
        if (!string.IsNullOrEmpty(AppSession.Token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppSession.Token);
    }
    public async Task<List<CategorySimpleDto>?> GetCategoriesAsync(int restaurantId)
    {
        SetAuth();
        var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/categories/restaurant/{restaurantId}");
        if (!res.IsSuccessStatusCode) return null;
        return Deserialize<List<CategorySimpleDto>>(await res.Content.ReadAsStringAsync());
    }

    public async Task<ApiResult> CreateCategoryAsync(CreateCategoryRequest req)
    {
        SetAuth();
        var res = await _http.PostAsync($"{AppConfig.ApiBaseUrl}/categories", ToJson(req));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل إضافة القسم: " + res.StatusCode };
    }

    public async Task<ApiResult> UpdateCategoryAsync(int id, UpdateCategoryRequest req)
    {
        SetAuth();
        var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/categories/{id}", ToJson(req));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل تعديل القسم: " + res.StatusCode };
    }

    public async Task<ApiResult> DeleteCategoryAsync(int id)
    {
        SetAuth();
        var res = await _http.DeleteAsync($"{AppConfig.ApiBaseUrl}/categories/{id}");
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل حذف القسم: " + res.StatusCode };
    }
    private static StringContent ToJson(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    private static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return default;
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    // ── Auth ────────────────────────────────────────────────────────────────────

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var res = await _http.PostAsync(
                AppConfig.ApiBaseUrl + "/auth/login",
                ToJson(new LoginRequest { Email = email, Password = password }));

            var body = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
                return new LoginResult { Ok = false, Error = "خطأ في تسجيل الدخول" };

            var data = Deserialize<LoginResponse>(body);
            return new LoginResult { Ok = true, Data = data };
        }
        catch (Exception ex)
        {
            return new LoginResult { Ok = false, Error = "لا يمكن الاتصال بالسيرفر: " + ex.Message };
        }
    }

    // ── Restaurants ─────────────────────────────────────────────────────────────

    public async Task<PagedResult<RestaurantDto>?> GetRestaurantsAsync(int page = 1, int pageSize = 50)
    {
        SetAuth();
        var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants?page={page}&pageSize={pageSize}");
        if (!res.IsSuccessStatusCode) return null;
        return Deserialize<PagedResult<RestaurantDto>>(await res.Content.ReadAsStringAsync());
    }

    public async Task<RestaurantDto?> GetRestaurantAsync(int id)
    {
        SetAuth();
        var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return Deserialize<RestaurantDto>(await res.Content.ReadAsStringAsync());
    }

    public async Task<ApiResult> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto)
    {
        SetAuth();
        var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}/desktop-update", ToJson(dto));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل تحديث بيانات المطعم: " + res.StatusCode };
    }

    public async Task<bool> ToggleRestaurantStatusAsync(int id)
    {
        SetAuth();
        var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}/toggle-status", null);
        return res.IsSuccessStatusCode;
    }

    // ── Menu ────────────────────────────────────────────────────────────────────

    public async Task<List<CategoryDto>?> GetMenuAsync(int restaurantId)
    {
        SetAuth();
        var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants/{restaurantId}/menu");
        if (!res.IsSuccessStatusCode) return null;
        return Deserialize<List<CategoryDto>>(await res.Content.ReadAsStringAsync());
    }

    public async Task<ApiResult> CreateProductAsync(CreateProductRequest req)
    {
        SetAuth();
        var res = await _http.PostAsync($"{AppConfig.ApiBaseUrl}/products", ToJson(req));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل إضافة المنتج" };
    }

    public async Task<ApiResult> UpdateProductAsync(int id, CreateProductRequest req)
    {
        SetAuth();
        var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/products/{id}", ToJson(req));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل تعديل المنتج" };
    }

    public async Task<bool> ToggleProductAvailabilityAsync(int id)
    {
        SetAuth();
        var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/products/{id}/toggle-availability", null);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        SetAuth();
        var res = await _http.DeleteAsync($"{AppConfig.ApiBaseUrl}/products/{id}");
        return res.IsSuccessStatusCode;
    }

    // ── Orders ──────────────────────────────────────────────────────────────────

    public async Task<List<OrderDetail>> GetRestaurantOrdersAsync(
        int restaurantId, string? status = null, int page = 1, int pageSize = 100)
    {
        SetAuth();
        var url = $"{AppConfig.ApiBaseUrl}/orders/restaurant/{restaurantId}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(status)) url += "&status=" + status;

        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return [];

        var paged = Deserialize<PagedResult<OrderDetail>>(await res.Content.ReadAsStringAsync());
        return paged?.Data ?? [];
    }

    public async Task<List<OrderDetail>> GetAllRestaurantOrdersAsync(
        int restaurantId, string? status = null)
    {
        var all = new List<OrderDetail>();
        int page = 1;
        const int pageSize = 100;

        while (true)
        {
            var orders = await GetRestaurantOrdersAsync(restaurantId, status, page, pageSize);
            if (orders.Count == 0) break;
            all.AddRange(orders);
            if (orders.Count < pageSize) break;
            page++;
        }
        return all;
    }

    public async Task<ApiResult> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        var url = $"{AppConfig.ApiBaseUrl}/orders/{orderId}/restaurant-status";
        var res = await _http.PutAsync(url, ToJson(new UpdateStatusRequest { Status = newStatus }));
        return res.IsSuccessStatusCode
            ? new ApiResult { Ok = true }
            : new ApiResult { Ok = false, Error = "فشل تحديث الحالة: " + res.StatusCode };
    }

    // ── Dashboard ────────────────────────────────────────────────────────────────

    public async Task<DashboardStats> GetDashboardStatsAsync(int restaurantId)
    {
        var orders = await GetAllRestaurantOrdersAsync(restaurantId);
        var today = DateTime.Today;
        var stats = new DashboardStats();

        foreach (var o in orders)
        {
            if (o.CreatedAt.Date == today)
            {
                stats.TodayOrders++;
                if (o.Status == "Delivered") stats.TodayRevenue += o.TotalAmount;
            }
            if (o.Status == "Pending") stats.PendingOrders++;
            if (o.Status is "Accepted" or "Preparing") stats.PreparingOrders++;
        }
        return stats;
    }
}