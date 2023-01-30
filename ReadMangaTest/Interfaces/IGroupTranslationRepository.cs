using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IGroupTranslationRepository
{
    Task<List<GroupTranslation>> GetAllAsync();
    Task<GroupTranslation> GetByIdAsync(int id);
    Task AddAsync(GroupTranslation groupTranslation);
    Task UpdateAsync(GroupTranslation groupTranslation);
    Task DeleteAsync(int id);
}