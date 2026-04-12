namespace BitcoinClub.ViewModels;

/// <summary>
/// Represents a single calendar event displayed on the landing page.
/// Populated by <see cref="BitcoinClub.Services.Events.IEventsService"/> from the club's iCal feed.
/// </summary>
public sealed class EventViewModel
{
    /// <summary>The event title as defined in Google Calendar.</summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// Local start date and time of the event.
    /// For recurring events this reflects the specific occurrence, not the series origin.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// <see langword="true"/> if the event spans a full day with no specific time (all-day event).
    /// When <see langword="true"/>, only the date part of <see cref="Start"/> is meaningful.
    /// </summary>
    public bool IsAllDay { get; set; }

    /// <summary>Location string from the calendar event, or <see langword="null"/> if not set.</summary>
    public string? Location { get; set; }
}
