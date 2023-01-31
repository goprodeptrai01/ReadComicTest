using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ArtistRepository: IArtistRepository
{
    private readonly DataContext _context;

    public ArtistRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IList<Artist>> GetAllAsync()
    {
        return await _context.Artists.ToListAsync();
    }

    public async Task<Artist> GetByIdAsync(int id)
    {
        return await _context.Artists.FindAsync(id);
    }

    public async Task AddAsync(Artist artist)
    {
        await _context.Artists.AddAsync(artist);
    }

    public async Task UpdateAsync(Artist artist)
    {
        _context.Artists.Update(artist);
    }

    public async Task DeleteAsync(int id)
    {
        var artist = await _context.Artists.FindAsync(id);
        _context.Artists.Remove(artist);
    }
}