using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IPageRepository
{
    Task<List<PageDto>> GetAllAsync();
    Task<PageDto> GetByIdAsync(int id);
    Task<List<PageDto>> GetPagesbyChapterAsync(int chapterId);
    //check methods
    bool IsExists(int id);
    bool IsAvailable(string name, int id);
    bool IsExists(string name, int id);
    //post api method
    Task<PageDto> AddAsync(PageDto pageDto, int chapterId);
    //put api methods
    Task<PageDto> UpdateAsync(PageDto pageDto, int chapterId);
    //remove, store api methods
    Task StoreAsync(int id);
    Task MultiStoreAsync(int[] id);
    Task DeleteAsync(int id);
    Task MultiDeleteAsync(int[] id);
}