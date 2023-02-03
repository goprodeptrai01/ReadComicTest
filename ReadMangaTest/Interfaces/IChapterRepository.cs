using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IChapterRepository
{
    //get api methods
    Task<List<ChapterDto>> GetAllAsync();
    Task<ChapterDto> GetByIdAsync(int id);
    Task<List<ChapterDto>> GetChaptersByComicAsync(int comicId);
    //check methods
    bool IsExists(int id);
    bool IsAvailable(string name, int id);
    bool IsExists(string name, int id);
    //post api method
    Task<ChapterDto> AddAsync(ChapterDto chapterDto, int comicId);
    //put api methods
    Task<ChapterDto> UpdateAsync(ChapterDto chapterDto, int id);
    //remove, store api methods
    Task StoreAsync(int id);
    Task MultiStoreAsync(int[] id);
    Task DeleteAsync(int id);
    Task MultiDeleteAsync(int[] id);
}