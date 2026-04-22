namespace BitcoinClub.Tests.Services.CalendarEvents.Handlers;

using System.Net;
using System.Text;

internal sealed class StaticIcsHandler(string ics) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ics, Encoding.UTF8, "text/calendar")
        });
}
