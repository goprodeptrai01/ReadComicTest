using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IComicRepository
{
    //get api methods
    Task<IEnumerable<ComicDto>> GetAllAsync();
    Task<Comic> GetByIdAsync(int id);
    Task<Comic> GetByNameAsync(string name);
    Task<IEnumerable<Comic>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Comic>> GetByArtistIdAsync(int artistId);
    //check methods
    bool IsExists(int id);
    bool IsExists(string name);
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