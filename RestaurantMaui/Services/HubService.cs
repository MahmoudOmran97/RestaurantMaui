using Microsoft.AspNetCore.SignalR.Client;

namespace RestaurantMaui.Services;

public class HubService : IAsyncDisposable
{
    private HubConnection? _connection;

    public event Action<int, string>? OnOrderStatusChanged;
    public event Action<int>? OnNewOrder;

    public bool IsConnected =>
        _connection?.State == HubConnectionState.Connected;

    public async Task StartAsync()
    {
        if (_connection != null) await StopAsync();

        _connection = new HubConnectionBuilder()
            .WithUrl(AppConfig.HubUrl, opts =>
            {
                opts.AccessTokenProvider = () => Task.FromResult<string?>(AppSession.Token);
                // تجاهل SSL في التطوير
                opts.HttpMessageHandlerFactory = _ =>
                    new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<int, string>("OrderStatusChanged",
            (id, st) => OnOrderStatusChanged?.Invoke(id, st));

        _connection.On<int>("NewOrder",
            id => OnNewOrder?.Invoke(id));

        try { await _connection.StartAsync(); }
        catch { /* السيرفر ممكن يكون مش شغال */ }
    }

    public async Task StopAsync()
    {
        if (_connection is null) return;
        await _connection.StopAsync();
        await _connection.DisposeAsync();
        _connection = null;
    }

    public async ValueTask DisposeAsync() => await StopAsync();
}