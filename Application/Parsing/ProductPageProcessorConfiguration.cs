namespace Application.Parsing;

public record ProductPageProcessorConfiguration(
    string EndOfAuctionXPathSelector,
    string UniqueIdentificationXPathSelector,
    string PriceXPathSelector,
    string NameXPathSelector,
    IEnumerable<NameSelectorPair> AdditionalInfromation);
