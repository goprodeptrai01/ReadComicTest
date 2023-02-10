using ReadMangaTest.Models;

namespace ReadMangaTest.DTO;

public class ArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public List<string> Comics { get; set; }
}

public class PostArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
