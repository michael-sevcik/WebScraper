using Microsoft.Extensions.DependencyInjection;

namespace WebScraper.Persistence.UnitOfWork;

/// <summary>
/// Dependency injection implementation of <see cref="IUnitOfWorkProvider"/>.
/// </summary>
internal sealed class UnitOfWorkProvider : IUnitOfWorkProvider
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkProvider"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider that is supposed create the instance of <see cref="IUnitOfWork"/>.</param>
    public UnitOfWorkProvider(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public IScopedUnitOfWork CreateScopedUnitOfWork()
    {
        var scope = this.serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>()
            ?? throw new Exception($"{nameof(IUnitOfWork)} service is not available.");

        return new ScopedUnitOFWork(unitOfWork, scope);
    }
}
