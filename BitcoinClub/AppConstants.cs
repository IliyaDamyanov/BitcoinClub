namespace BitcoinClub;

/// <summary>
/// Application-wide constants shared across all layers of the project.
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Constants related to the Google Calendar integration.
    /// </summary>
    public static class Calendar
    {
        /// <summary>Public iCal URL for the Bitcoin Club Sofia Google Calendar.</summary>
        public const string ICalUrl =
            "https://calendar.google.com/calendar/ical/" +
            "ef1b943ddbe088897fb86e9ddac3a57f3bbc0f303c93f6608369ff1926fc97cc%40group.calendar.google.com" +
            "/public/basic.ics";

        /// <summary>Memory cache key for the parsed upcoming events list.</summary>
        public const string CacheKey = "events_ical";
    }
}
