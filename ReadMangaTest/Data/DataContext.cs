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
    public DbSet<GroupTranslation> GroupTranslations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupTranslationComic>()
            .HasKey(gtc => new { gtc.GroupTranslationId, gtc.ComicId });
        modelBuilder.Entity<GroupTranslationComic>()
            .HasOne(gtc => gtc.GroupTranslation)
            .WithMany(gt => gt.GroupTranslationComics)
            .HasForeignKey(gtc => gtc.GroupTranslationId);
        modelBuilder.Entity<GroupTranslationComic>()
            .HasOne(gtc => gtc.Comic)
            .WithMany(c => c.GroupTranslationComics)
            .HasForeignKey(gtc => gtc.ComicId);
        
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