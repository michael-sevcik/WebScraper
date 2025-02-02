﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Downloader;

namespace ProductListCrawler;

/// <summary>
/// Represents a helper class that provides a simple way to crawl through a list of products.
/// </summary>
public interface IProductListCrawler
{
    /// <summary>
    /// Crawls through the list of products, which starts with <paramref name="productListStart"/> page, as an asynchronous operation using a <see cref="Task"/> object.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <param name="productListStart">The URI of the page to start crawling.</param>
    /// <param name="productPageTarget">The target block to which the product page URIs should be passed.</param>
    /// <param name="processor">The processor of a product list.</param>
    /// <returns>The task object representing the asynchronous crawling.</returns>
    /// <exception cref="NetworkException">Network error occurred.</exception>
    Task Crawl(Uri productListStart, IProductListProcessor processor, ITargetBlock<IReadOnlyCollection<Uri>> productPageTarget, CancellationToken token);
}
