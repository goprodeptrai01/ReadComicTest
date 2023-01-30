namespace ReadMangaTest.Models;

public class GroupTranslationComic
{
    public int GroupTranslationId { get; set; }
    public GroupTranslation GroupTranslation { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }
}