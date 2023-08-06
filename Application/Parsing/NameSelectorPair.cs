namespace Application.Parsing;

/// <summary>
/// Record for storing additional information selectors with the name of the desired information.
/// </summary>
/// <param name="Name">The name of the information selected by <paramref name="XPathSelector"/></param>
/// <param name="XPathSelector">The Selector of the desired information.</param>
public record NameSelectorPair(string Name, string XPathSelector);
