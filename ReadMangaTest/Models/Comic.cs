using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadMangaTest.Models;

public class Comic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    public string Wallpaper { get; set; }
    public Artist Artist { get; set; }
    public virtual ICollection<Chapter> Chapters { get; set; }
    
    public ICollection
        <ComicCategory> comicCategories { get; set; }
}