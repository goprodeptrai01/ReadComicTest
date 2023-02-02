using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(int id);
    //check methods
    bool IsExists(int id);
    bool IsExists(string name);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}