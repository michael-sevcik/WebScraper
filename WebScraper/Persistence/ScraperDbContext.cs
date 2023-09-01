using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Persistence;

public sealed class ScraperDbContext : DbContext
{
    public ScraperDbContext()
    {
    }

    public ScraperDbContext(DbContextOptions<ScraperDbContext> options)
    : base(options)
    {
    }

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



    }
}
