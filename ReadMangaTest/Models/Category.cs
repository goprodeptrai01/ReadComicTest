using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<ComicCategory> comicCategories { get; set; }
}