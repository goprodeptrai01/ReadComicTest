using ReadMangaTest.Models;

namespace ReadMangaTest.DTO;

public class ArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ArtistGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<String> ComicName { get; set; }
}