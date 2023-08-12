namespace Application.Parsing;

/// <summary>
/// Encapsulation of date's format and its CSS selector
/// </summary>
/// <param name="Selector">The date </param>
/// <param name="format"></param>
public record DateWithExactFormatSelector(string Selector, string Format);

/// <summary>
/// Record of configuration for <see cref="ProductPageProcessor"/> encapsulates various selectors for parsing relevant data.
/// </summary>
public record ProductPageProcessorConfiguration(
    DateWithExactFormatSelector EndOfAuctionCssSelector,
    string UniqueIdentificationCssSelector,
    string PriceCssSelector,
    string NameCssSelector,
    IEnumerable<NameSelectorPair> AdditionalInfromation);
