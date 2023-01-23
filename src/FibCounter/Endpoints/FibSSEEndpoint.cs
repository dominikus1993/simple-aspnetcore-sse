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
        await SendEventStreamAsync<FibResponse>("my-event", GetDataStream(ct), ct);
    }

    private static async IAsyncEnumerable<FibResponse> GetDataStream(
        [EnumeratorCancellation] CancellationToken cancellation)
    {
        int counter = 0;
        while (!cancellation.IsCancellationRequested)
        {
            yield return new FibResponse(counter, Fib(counter));
            counter += 1;
            await Task.Delay(TimeSpan.FromSeconds(2), cancellation);
        }
    }

    private static long Fib(int n)
    {
        if (n < 2)
        {
            return n;
        }
        var a = 0;
        var b = 1;
        
        for (int i = 2; i <= n; i++)
        {
            var c = b + a;
            a = b;
            b = c;
        }

        return b;
    }
}

internal readonly record struct FibResponse(long N, long Value);
