namespace ReadMangaTest.DTO;

public class ComicDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Wallpaper { get; set; }
    public string Artist { get; set; }
    public string Url { get; set; }
    public List<string> Categories { get; set; }
}

public class PostComicDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Wallpaper { get; set; }
}