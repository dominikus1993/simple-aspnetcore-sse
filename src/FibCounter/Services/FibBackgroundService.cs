using System.Text.Json;

namespace FibCounter.Services;

public readonly record struct FibResponse(long CurrentValue);
public class FibBackgroundService: BackgroundService
{
    private readonly IFibServerSentEvents _client;

    public FibBackgroundService(IFibServerSentEvents client)
    {
        _client = client;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long counter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var clients = _client.GetClients();
            if (clients.Count > 0)
            {
                var poolSerialized = JsonSerializer.Serialize(new FibResponse(counter));

                await _client.SendEventAsync(poolSerialized, stoppingToken);
                counter = counter + 1;
            }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
