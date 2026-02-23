using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class LandingPageCalendarHtmlValidationTests
    {
        [Fact]
        public async Task HomePage_IncludesResponsiveGoogleCalendarIframe()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();

            Assert.Contains("ratio ratio-16x9", html);
            Assert.Contains("<iframe", html);
            Assert.Contains("calendar.google.com/calendar/embed", html, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("loading=\"lazy\"", html);
        }
    }
}
