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
        _http = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    private void SetAuth()
    {
        if (!string.IsNullOrEmpty(AppSession.Token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppSession.Token);
    }

    // ── Categories ──────────────────────────────────────────────────────────────

    public async Task<List<CategorySimpleDto>?> GetCategoriesAsync(int restaurantId)
    {
        SetAuth();
        try
        {
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/categories/restaurant/{restaurantId}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<List<CategorySimpleDto>>(await res.Content.ReadAsStringAsync());
        }
        catch { return null; }
    }

    public async Task<ApiResult> CreateCategoryAsync(CreateCategoryRequest req)
    {
        SetAuth();
        try
        {
            var res = await _http.PostAsync($"{AppConfig.ApiBaseUrl}/categories", ToJson(req));
            return res.IsSuccessStatusCode
                ? new ApiResult { Ok = true }
                : new ApiResult { Ok = false, Error = "فشل إضافة القسم: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    public async Task<ApiResult> UpdateCategoryAsync(int id, UpdateCategoryRequest req)
    {
        SetAuth();
        try
        {
            var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/categories/{id}", ToJson(req));
            return res.IsSuccessStatusCode
                ? new ApiResult { Ok = true }
                : new ApiResult { Ok = false, Error = "فشل تعديل القسم: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    public async Task<ApiResult> DeleteCategoryAsync(int id)
    {
        SetAuth();
        try
        {
            var res = await _http.DeleteAsync($"{AppConfig.ApiBaseUrl}/categories/{id}");
            return res.IsSuccessStatusCode
                ? new ApiResult { Ok = true }
                : new ApiResult { Ok = false, Error = "فشل حذف القسم: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
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
            if (data is null)
                return new LoginResult { Ok = false, Error = "استجابة غير صحيحة من السيرفر" };

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
        try
        {
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants?page={page}&pageSize={pageSize}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<PagedResult<RestaurantDto>>(await res.Content.ReadAsStringAsync());
        }
        catch { return null; }
    }

    public async Task<RestaurantDto?> GetRestaurantAsync(int id)
    {
        SetAuth();
        try
        {
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<RestaurantDto>(await res.Content.ReadAsStringAsync());
        }
        catch { return null; }
    }

    public async Task<ApiResult> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto)
    {
        SetAuth();
        try
        {
            var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}/desktop-update", ToJson(dto));
            return res.IsSuccessStatusCode
                ? new ApiResult { Ok = true }
                : new ApiResult { Ok = false, Error = "فشل تحديث بيانات المطعم: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    public async Task<bool> ToggleRestaurantStatusAsync(int id)
    {
        SetAuth();
        try
        {
            var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}/toggle-status", null);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ── Menu ────────────────────────────────────────────────────────────────────

    public async Task<List<CategoryDto>?> GetMenuAsync(int restaurantId)
    {
        SetAuth();
        try
        {
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants/{restaurantId}/menu");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<List<CategoryDto>>(await res.Content.ReadAsStringAsync());
        }
        catch { return null; }
    }

    public async Task<ApiResult> CreateProductAsync(CreateProductRequest req)
    {
        SetAuth();
        try
        {
            var res = await _http.PostAsync($"{AppConfig.ApiBaseUrl}/products", ToJson(req));
            if (res.IsSuccessStatusCode) return new ApiResult { Ok = true };
            var err = await res.Content.ReadAsStringAsync();
            return new ApiResult { Ok = false, Error = "فشل إضافة المنتج: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    public async Task<ApiResult> UpdateProductAsync(int id, CreateProductRequest req)
    {
        SetAuth();
        try
        {
            var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/products/{id}", ToJson(req));
            if (res.IsSuccessStatusCode) return new ApiResult { Ok = true };
            return new ApiResult { Ok = false, Error = "فشل تعديل المنتج: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    public async Task<bool> ToggleProductAvailabilityAsync(int id)
    {
        SetAuth();
        try
        {
            var res = await _http.PutAsync($"{AppConfig.ApiBaseUrl}/products/{id}/toggle-availability", null);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        SetAuth();
        try
        {
            var res = await _http.DeleteAsync($"{AppConfig.ApiBaseUrl}/products/{id}");
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ── Orders ──────────────────────────────────────────────────────────────────

    public async Task<List<OrderDetail>> GetRestaurantOrdersAsync(
        int restaurantId, string? status = null, int page = 1, int pageSize = 100)
    {
        SetAuth();
        try
        {
            var url = $"{AppConfig.ApiBaseUrl}/orders/restaurant/{restaurantId}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += "&status=" + status;

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return [];

            var paged = Deserialize<PagedResult<OrderDetail>>(await res.Content.ReadAsStringAsync());
            return paged?.Data ?? [];
        }
        catch { return []; }
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
        // FIX: كانت ناقصة SetAuth() - بيتسبب في 401 Unauthorized
        SetAuth();
        try
        {
            var url = $"{AppConfig.ApiBaseUrl}/orders/{orderId}/restaurant-status";
            var res = await _http.PutAsync(url, ToJson(new UpdateStatusRequest { Status = newStatus }));
            return res.IsSuccessStatusCode
                ? new ApiResult { Ok = true }
                : new ApiResult { Ok = false, Error = "فشل تحديث الحالة: " + res.StatusCode };
        }
        catch (Exception ex)
        {
            return new ApiResult { Ok = false, Error = "خطأ في الاتصال: " + ex.Message };
        }
    }

    // ── Dashboard ────────────────────────────────────────────────────────────────

    public async Task<DashboardStats> GetDashboardStatsAsync(int restaurantId)
    {
        // FIX: استخدام صفحة واحدة بس للـ Dashboard بدل تحميل كل الأوردرات
        var orders = await GetRestaurantOrdersAsync(restaurantId, null, 1, 200);
        // FIX: مقارنة UTC مع UTC
        var today = DateTime.UtcNow.Date;
        var stats = new DashboardStats();

        foreach (var o in orders)
        {
            var orderDate = o.CreatedAt.Kind == DateTimeKind.Utc
                ? o.CreatedAt.Date
                : o.CreatedAt.ToUniversalTime().Date;

            if (orderDate == today)
            {
                stats.TodayOrders++;
                if (o.Status == "Delivered") stats.TodayRevenue += o.TotalAmount;
            }
            if (o.Status == "Pending") stats.PendingOrders++;
            if (o.Status is "Accepted" or "Preparing") stats.PreparingOrders++;
        }
        return stats;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

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
}
