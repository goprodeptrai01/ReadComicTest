namespace ReadMangaTest.Models;

public class GroupTranslation
{
    public int GroupTranslationId { get; set; }
    public string GroupName { get; set; }

    public ICollection<GroupTranslationComic> GroupTranslationComics { get; set; }

}