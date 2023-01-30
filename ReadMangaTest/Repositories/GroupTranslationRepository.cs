using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class GroupTranslationRepository: IGroupTranslationRepository
{
    private readonly DataContext _context;

    public GroupTranslationRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<List<GroupTranslation>> GetAllAsync()
    {
        return await _context.GroupTranslations.ToListAsync();
    }

    public async Task<GroupTranslation> GetByIdAsync(int id)
    {
        return await _context.GroupTranslations.FindAsync(id);
    }

    public async Task AddAsync(GroupTranslation groupTranslation)
    {
        await _context.GroupTranslations.AddAsync(groupTranslation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GroupTranslation groupTranslation)
    {
        _context.GroupTranslations.Update(groupTranslation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var groupTranslation = await _context.GroupTranslations.FindAsync(id);
        _context.GroupTranslations.Remove(groupTranslation);
        await _context.SaveChangesAsync();
    }
}