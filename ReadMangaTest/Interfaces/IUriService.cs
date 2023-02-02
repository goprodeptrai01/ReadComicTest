using ReadMangaTest.Filters;

namespace ReadMangaTest.Interfaces;

public interface IUriService
{
    public Uri GetPageUri(PaginationFilter filter, string route);
}