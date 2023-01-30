using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ComicRepository: IComicRepository
{
    private readonly DataContext _context;

    public ComicRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Comic>> GetAllAsync()
    {
        return await _context.Comics.ToListAsync();
    }

    public async Task<Comic> GetByIdAsync(int id)
    {
        return await _context.Comics.FindAsync(id);
    }

    public async Task AddAsync(Comic comic)
    {
        await _context.Comics.AddAsync(comic);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Comic comic)
    {
        _context.Comics.Update(comic);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var comic = await _context.Comics.FindAsync(id);
        _context.Comics.Remove(comic);
        await _context.SaveChangesAsync();
    }
}