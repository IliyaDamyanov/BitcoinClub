namespace BitcoinClub.Services.CalendarEvents;

using BitcoinClub.ViewModels;
using Ical.Net;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Fetches upcoming events from the Bitcoin Club Sofia public Google Calendar iCal feed,
/// expands recurring events into individual occurrences, and caches the result in memory.
/// </summary>
/// <remarks>
/// Recurring events (e.g. the monthly meetup) are resolved via <c>CalendarEvent.GetOccurrences</c>
/// from Ical.Net, which correctly expands RRULE entries that a plain <c>calendar.Events</c>
/// iteration would miss. Occurrences are searched up to 3 months ahead.
/// On any network or parse failure the service logs a warning and returns an empty list,
/// so the landing page degrades gracefully rather than throwing.
/// </remarks>
public sealed class CalendarEventsService : ICalendarEventsService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CalendarEventsService> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initialises the service. <paramref name="http"/> is injected as a typed
    /// <see cref="HttpClient"/> managed by <c>IHttpClientFactory</c> (registered via
    /// <c>AddHttpClient&lt;ICalendarEventsService, CalendarEventsService&gt;()</c> in Program.cs).
    /// </summary>
    public CalendarEventsService(HttpClient http, IMemoryCache cache, ILogger<CalendarEventsService> logger, TimeProvider timeProvider)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CalendarEventViewModel>> GetUpcomingAsync(int maxCount = 6)
    {
        if (_cache.TryGetValue(AppConstants.Calendar.CacheKey, out IReadOnlyList<CalendarEventViewModel>? cached) && cached != null)
            return cached;

        try
        {
            var icsContent = await _http.GetStringAsync(AppConstants.Calendar.ICalUrl);
            var calendar = Calendar.Load(icsContent);
            if (calendar is null) return Array.Empty<CalendarEventViewModel>();

            // GetOccurrences expands recurring events. Occurrences are in chronological
            // order, so TakeWhile stops enumeration once we pass the 3-month window.
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var from = new CalDateTime(now.AddDays(-1));
            var until = now.AddMonths(3);

            var events = calendar.Events
                .SelectMany(e => e.GetOccurrences(from)
                    .TakeWhile(o => o.Period.StartTime.Value <= until)
                    .Select(o => new CalendarEventViewModel
                    {
                        Title = e.Summary ?? "",
                        Start = o.Period.StartTime.Value,
                        IsAllDay = e.IsAllDay,
                        Location = e.Location
                    }))
                .OrderBy(e => e.Start)
                .Take(maxCount)
                .ToList();

            _cache.Set(AppConstants.Calendar.CacheKey, (IReadOnlyList<CalendarEventViewModel>)events, TimeSpan.FromMinutes(30));

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load events from iCal feed");

            return Array.Empty<CalendarEventViewModel>();
        }
    }
}
