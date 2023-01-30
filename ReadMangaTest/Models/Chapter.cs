﻿using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Chapter
{
    public int Id { get; set; }
    public int ComicId { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    public string Content { get; set; }

    public Comic Comic { get; set; }
}