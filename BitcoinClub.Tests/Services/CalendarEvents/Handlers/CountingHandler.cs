namespace BitcoinClub.Tests.Services.CalendarEvents.Handlers;

using System.Net;
using System.Text;

internal sealed class CountingHandler(string ics) : HttpMessageHandler
{
    public int CallCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ics, Encoding.UTF8, "text/calendar")
        });
    }
}
