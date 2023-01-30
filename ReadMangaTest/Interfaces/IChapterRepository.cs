using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IChapterRepository
{
    Task<List<Chapter>> GetAllAsync();
    Task<Chapter> GetByIdAsync(int id);
    Task AddAsync(Chapter chapter);
    Task UpdateAsync(Chapter chapter);
    Task DeleteAsync(int id);
}