namespace BitcoinClub.Services.CalendarEvents;

using BitcoinClub.ViewModels;

/// <summary>
/// Provides upcoming events sourced from the club's public Google Calendar iCal feed.
/// </summary>
public interface ICalendarEventsService
{
    /// <summary>
    /// Returns the next <paramref name="maxCount"/> upcoming events, ordered by start date ascending.
    /// Results are cached for 30 minutes. Returns an empty list if the feed is unreachable or unparseable.
    /// </summary>
    /// <param name="maxCount">Maximum number of events to return. Defaults to 6.</param>
    Task<IReadOnlyList<CalendarEventViewModel>> GetUpcomingAsync(int maxCount = 6);
}
