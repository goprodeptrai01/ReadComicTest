using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Models;

namespace ReadMangaTest.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Comic> Comics { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Artist> Artists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comic>()
            .HasOne(t => t.Category)
            .WithMany(tl => tl.Comics)
            .HasForeignKey(t => t.CategoryId);

        modelBuilder.Entity<Comic>()
            .HasOne(t => t.Artist)
            .WithMany(tg => tg.Comics)
            .HasForeignKey(t => t.ArtistId);

        modelBuilder.Entity<Chapter>()
            .HasOne(c => c.Comic)
            .WithMany(t => t.Chapters)
            .HasForeignKey(c => c.ComicId);
    }
}