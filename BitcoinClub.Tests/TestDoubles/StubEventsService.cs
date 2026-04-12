using BitcoinClub.Services.Events;
using BitcoinClub.ViewModels;

namespace BitcoinClub.Tests.TestDoubles;

/// <summary>
/// A no-op stub of <see cref="IEventsService"/> that always returns an empty event list.
/// Used in unit tests that exercise controller or landing-page logic unrelated to events.
/// </summary>
internal sealed class StubEventsService : IEventsService
{
    public Task<IReadOnlyList<EventViewModel>> GetUpcomingAsync(int maxCount = 6)
        => Task.FromResult<IReadOnlyList<EventViewModel>>(Array.Empty<EventViewModel>());
}
