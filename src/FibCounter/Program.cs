using System.Net.WebSockets;
using System.Text.Json;
using FibCounter.Services;
using Lib.AspNetCore.ServerSentEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<FibBackgroundService>();
builder.Services.AddServerSentEvents<IFibServerSentEvents, FibServerSentEvents>(options =>
{
    options.KeepaliveMode = ServerSentEventsKeepaliveMode.Always;
    options.KeepaliveInterval = 500;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(b =>
{
    b.AllowAnyHeader();
    b.AllowAnyMethod();
    b.AllowAnyOrigin();
});
app.UseAuthorization();
app.MapServerSentEvents<FibServerSentEvents>("/sse", new ServerSentEventsOptions
{
    OnPrepareAccept = response =>
    {
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("X-Accel-Buffering", "no");
    }
});
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

app.MapControllers();
app.MapGet("/ws", async (HttpContext context, CancellationToken ct) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await Echo(webSocket, ct);
        return Results.Ok("xD");
    }
    else
    {
        return Results.BadRequest("wypad");
    }
});
app.Run();


static async Task Echo(WebSocket webSocket, CancellationToken cancellationToken)
{
    long conuter = 1;
    while (webSocket.State == WebSocketState.Open)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(new FibResponse(conuter));
        await webSocket.SendAsync(json, WebSocketMessageType.Text, true, cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        conuter = conuter + 1;
    }

    await webSocket.CloseAsync(webSocket.CloseStatus.Value, "xD", cancellationToken);
}
