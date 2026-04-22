namespace BitcoinClub.Tests.Services.CalendarEvents.Handlers;

internal sealed class FailingHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => throw new HttpRequestException("Network error");
}
