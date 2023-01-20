using System.Runtime.CompilerServices;
using FastEndpoints;

namespace FibCounter.Endpoints;

public sealed class FibSSEEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("event-stream");
        AllowAnonymous();
        Options(x => x.RequireCors(p => p.AllowAnyOrigin().AllowAnyMethod()));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        //simply provide any IAsyncEnumerable<T> as argument
        await SendEventStreamAsync("my-event", GetDataStream(ct), ct);
    }

    private static async IAsyncEnumerable<object> GetDataStream(
        [EnumeratorCancellation] CancellationToken cancellation)
    {
        long counter = 0;
        while (!cancellation.IsCancellationRequested)
        {
            yield return new FibResponse(counter);
            counter += 1;
            await Task.Delay(TimeSpan.FromSeconds(2), cancellation);
        }
    }
}

internal record FibResponse(long Counter);
