using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Persistence.UnitOfWork;

/// <summary>
/// Represents a scope wrapper around instance of <see cref="IUnitOfWork"/>.
/// </summary>
public interface IScopedUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the unit of work.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
