using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface ICategoryRepository
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    //check methods
    bool IsExists(int id);
    bool IsExists(string name, int id);
    Task AddAsync(Category category);
    Task<CategoryDto> UpdateAsync(CategoryDto categoryDto, int id);
    Task StoreAsync(int id);
    Task DeleteAsync(int id);
}