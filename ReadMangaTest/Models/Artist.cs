using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Artist
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    [StringLength(500)]
    public string Description { get; set; }
    public ICollection<Comic> Comics { get; set; }
}