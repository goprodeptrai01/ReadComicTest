using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Chapter
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    public string Content { get; set; }
    public bool IsHidden { get; set; } = false;
    public Comic Comic { get; set; }
}