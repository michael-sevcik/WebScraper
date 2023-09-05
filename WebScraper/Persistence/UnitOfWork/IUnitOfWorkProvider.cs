namespace WebScraper.Persistence.UnitOfWork;

/// <summary>
/// Represents a provider of a scoped instance of <see cref="IScopedUnitOfWork"/>.
/// </summary>
public interface IUnitOfWorkProvider
{
    /// <summary>
    /// Creates a scoped unit of work.
    /// </summary>
    /// <returns>The scoped unit of work.</returns>
    IScopedUnitOfWork CreateUnitOfWork();
}
