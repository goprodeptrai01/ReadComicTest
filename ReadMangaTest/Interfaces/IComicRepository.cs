using ReadMangaTest.DTO;
using ReadMangaTest.Filters;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IComicRepository
{
    //get api methods
    Task<List<ComicDto>> GetAllAsync(PaginationFilter filter);
    Task<ComicDto> GetByIdAsync(int id);
    Task<ComicDto> GetByNameAsync(string name);
    Task<IEnumerable<ComicDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ComicDto>> GetByArtistIdAsync(int artistId);
    //check methods
    bool IsExists(int id);
    bool IsExists(string name, int id);
    //post api method
    Task<ComicDto> AddAsync(PostComicDto comicDto, int[] categoryIds, int artistId);
    //put api methods
    Task<ComicDto> UpdateAsync(PostComicDto comicDto, int id);
    Task<bool> UpdateComicCategoryAsync(int id, int[] categoryId);
    Task<bool> UpdateComicArtistAsync(int id, int artistId);
    //remove, store api methods
    Task StoreAsync(int id);
    Task MultiStoreAsync(int[] id);
    Task DeleteAsync(int id);
    Task MultiDeleteAsync(int[] id);
}