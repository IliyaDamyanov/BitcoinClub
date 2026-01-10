using Microsoft.Extensions.Localization;

namespace BitcoinClub.Tests.TestDoubles;

internal sealed class StubStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public StubStringLocalizer(IReadOnlyDictionary<string, string> values)
    {
        _values = values;
    }

    public LocalizedString this[string name]
        => new(name, _values.TryGetValue(name, out var value) ? value : name, resourceNotFound: !_values.ContainsKey(name));

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(_values.TryGetValue(name, out var value) ? value : name, arguments), resourceNotFound: !_values.ContainsKey(name));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _values.Select(kvp => new LocalizedString(kvp.Key, kvp.Value, resourceNotFound: false));

    public IStringLocalizer WithCulture(System.Globalization.CultureInfo culture) => this;
}
