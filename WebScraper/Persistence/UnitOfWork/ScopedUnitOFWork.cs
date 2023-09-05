using Microsoft.Extensions.DependencyInjection;

namespace WebScraper.Persistence.UnitOfWork
{
    /// <summary>
    /// Service scope wrapper implementation of <see cref="IScopedUnitOfWork"/>.
    /// </summary>
    internal class ScopedUnitOFWork : IScopedUnitOfWork
    {
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedUnitOFWork"/> class.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork created in <paramref name="serviceScope"/>.</param>
        /// <param name="serviceScope">A service scope.</param>
        public ScopedUnitOFWork(IUnitOfWork unitOfWork, IServiceScope serviceScope)
            => (this.serviceScope, this.UnitOfWork) = (serviceScope, unitOfWork);

        /// <inheritdoc/>
        public IUnitOfWork UnitOfWork { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.serviceScope.Dispose();
        }
    }
}
