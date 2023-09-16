using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Persistence.AuctionRecord;

/// <summary>
/// Encapsulation of an information and its name.
/// </summary>
public class AdditionalInfromationPair
{
    /// <summary>
    /// Gets the name of <see cref="Value"/>.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the value of <see cref="Name"/>.
    /// </summary>
    public string Value { get; init; } = string.Empty;
}
