using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Parsing.ProductListProcessor;

namespace Application.Parsing;

/// <summary>
/// Represents type of next page link extraction.
/// </summary>
public enum NextPageSelectionType
{
    /// <summary>
    /// The link is extracted from a button for continuing to the next page.
    /// </summary>
    button,

    /// <summary>
    /// The link is extracted from the element next to the current page number.
    /// </summary>
    nextElementToCurrenctlySelected,
}

/// <summary>
/// Configuration record for <see cref="ProductListProcessor"/>.
/// </summary>
public record ProductListProcessorConfiguration(string ProductsCssSelector, string NextPageCSSSelector, NextPageSelectionType TypeOfNextPageSelection);
