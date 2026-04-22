namespace BitcoinClub.Tests.Services.CalendarEvents.Helpers;

using BitcoinClub.Services.CalendarEvents;
using BitcoinClub.ViewModels;

/// <summary>
/// A no-op stub of <see cref="ICalendarEventsService"/> that always returns an empty event list.
/// Used in tests that exercise controller or landing-page logic unrelated to calendar events.
/// </summary>
internal sealed class StubCalendarEventsService : ICalendarEventsService
{
    public Task<IReadOnlyList<CalendarEventViewModel>> GetUpcomingAsync(int maxCount = 6)
        => Task.FromResult<IReadOnlyList<CalendarEventViewModel>>(Array.Empty<CalendarEventViewModel>());
}
