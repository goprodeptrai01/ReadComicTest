using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ChapterRepository : IChapterRepository
{
    private readonly DataContext _context;

    public ChapterRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Chapter>> GetAllAsync()
    {
        return await _context.Chapters.ToListAsync();
    }

    public async Task<Chapter> GetByIdAsync(int id)
    {
        return await _context.Chapters.FindAsync(id);
    }

    public async Task AddAsync(Chapter chapter)
    {
        await _context.Chapters.AddAsync(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Chapter chapter)
    {
        _context.Chapters.Update(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var chapter = await _context.Chapters.FindAsync(id);
        _context.Chapters.Remove(chapter);
        await _context.SaveChangesAsync();
    }
}