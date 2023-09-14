using Microsoft.EntityFrameworkCore;
using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Persistence;

/// <summary>
/// Implementation of <see cref="DbContext"/> that enables storing and managing <see cref="BaseAuctionRecord"/> entities.
/// </summary>
public sealed class ScraperDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScraperDbContext"/> class.
    /// </summary>
    public ScraperDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScraperDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ScraperDbContext(DbContextOptions<ScraperDbContext> options)
    : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet of <see cref="BaseAuctionRecord"/> entities.
    /// </summary>
    public DbSet<BaseAuctionRecord> AuctionRecords { get; set; }

    /// <summary>
    /// On configuring method. Configures the database connection with the predefined settings.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer();

    /// <summary>
    /// On model creating method. Configures entity relationships, table names and initial data.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseAuctionRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsMany(e => e.AdditionalInfromation).ToJson();
        });
    }
}
