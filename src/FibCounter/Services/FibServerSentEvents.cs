using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace FibCounter.Services;

public interface IFibServerSentEvents : IServerSentEventsService
{
}

public class FibServerSentEvents:  ServerSentEventsService, IFibServerSentEvents
{
    public FibServerSentEvents(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options) : base(options)
    {
    }
}
