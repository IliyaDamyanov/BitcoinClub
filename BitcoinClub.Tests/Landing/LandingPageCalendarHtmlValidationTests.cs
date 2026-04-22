using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class LandingPageCalendarHtmlValidationTests
    {
        [Fact]
        public async Task HomePage_IncludesEventsSection()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();

            // The Google Calendar iframe has been replaced with a server-side iCal events section.
            Assert.Contains("bc-section--calendar", html);
            Assert.Contains("bc-calendar-link", html);
            Assert.Contains("calendar.google.com", html, StringComparison.OrdinalIgnoreCase);
        }
    }
}
