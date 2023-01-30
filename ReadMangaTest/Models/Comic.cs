using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Comic
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public int ArtistId { get; set; }
    public int CategoryId { get; set; }
    public string Describtion { get; set; }
    public string Wallpaper { get; set; }
    public Artist Artist { get; set; }
    public Category Category { get; set; }
    public virtual ICollection<Chapter> Chapters { get; set; }
    public ICollection<GroupTranslationComic> GroupTranslationComics { get; set; }
}