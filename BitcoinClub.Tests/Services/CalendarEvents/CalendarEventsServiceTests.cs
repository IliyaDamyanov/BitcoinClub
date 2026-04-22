namespace BitcoinClub.Tests.Services.CalendarEvents;

using System.Globalization;
using System.Text;

using BitcoinClub.Services.CalendarEvents;
using BitcoinClub.Tests.Services.CalendarEvents.Handlers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Xunit;

public sealed class CalendarEventsServiceTests
{
    [Fact]
    public async Task GetUpcomingAsync_FiltersOutEventsAfterThreeMonthWindow()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);
        var ics = BuildCalendar(
            ("inside-window", now.AddMonths(2)),
            ("outside-window", now.AddMonths(4)));

        using var httpClient = new HttpClient(new StaticIcsHandler(ics));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        var result = await sut.GetUpcomingAsync(maxCount: 10);

        // Assert
        var titles = result.Select(x => x.Title).ToList();
        Assert.Contains("inside-window", titles);
        Assert.DoesNotContain("outside-window", titles);
    }

    [Fact]
    public async Task GetUpcomingAsync_IncludesEventOnThreeMonthBoundary()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);
        var ics = BuildCalendar(("boundary", now.AddMonths(3)));

        using var httpClient = new HttpClient(new StaticIcsHandler(ics));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        var result = await sut.GetUpcomingAsync(maxCount: 10);

        // Assert
        Assert.Contains(result, x => x.Title == "boundary");
    }

    [Fact]
    public async Task GetUpcomingAsync_RespectsMaxCount()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);
        var ics = BuildCalendar(
            ("event-1", now.AddDays(1)),
            ("event-2", now.AddDays(2)),
            ("event-3", now.AddDays(3)),
            ("event-4", now.AddDays(4)),
            ("event-5", now.AddDays(5)));

        using var httpClient = new HttpClient(new StaticIcsHandler(ics));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        var result = await sut.GetUpcomingAsync(maxCount: 3);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetUpcomingAsync_ReturnsEventsInChronologicalOrder()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);

        // Intentionally out of order in the calendar
        var ics = BuildCalendar(
            ("later", now.AddDays(10)),
            ("earlier", now.AddDays(2)),
            ("middle", now.AddDays(5)));

        using var httpClient = new HttpClient(new StaticIcsHandler(ics));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        var result = await sut.GetUpcomingAsync(maxCount: 10);

        // Assert
        var titles = result.Select(x => x.Title).ToList();
        Assert.Equal(["earlier", "middle", "later"], titles);
    }

    [Fact]
    public async Task GetUpcomingAsync_ReturnsEmptyList_OnHttpFailure()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);

        using var httpClient = new HttpClient(new FailingHandler());
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        var result = await sut.GetUpcomingAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUpcomingAsync_SecondCall_HitsCache_NotHttp()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 04, 15, 12, 00, 00, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);
        var ics = BuildCalendar(("event", now.AddDays(1)));

        var handler = new CountingHandler(ics);
        using var httpClient = new HttpClient(handler);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new CalendarEventsService(httpClient, cache, NullLogger<CalendarEventsService>.Instance, timeProvider);

        // Act
        await sut.GetUpcomingAsync();
        await sut.GetUpcomingAsync();

        // Assert
        Assert.Equal(1, handler.CallCount);
    }

    private static string BuildCalendar(params (string title, DateTimeOffset start)[] events)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//BitcoinClub.Tests//EN");

        foreach (var (title, start) in events)
        {
            var dt = start.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{Guid.NewGuid()}");
            sb.AppendLine($"DTSTAMP:{dt}");
            sb.AppendLine($"DTSTART:{dt}");
            sb.AppendLine($"SUMMARY:{title}");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");
        return sb.ToString();
    }

}
