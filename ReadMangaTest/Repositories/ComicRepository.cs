using System.Diagnostics;
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

    public async Task<IEnumerable<Comic>> GetAllAsync()
    {
        return await _context.Comics.ToListAsync();
    }

    public async Task<Comic> GetByIdAsync(int id)
    {
        return await _context.Comics.FindAsync(id);
    }

    public async Task<IEnumerable<Comic>> GetByArtistIdAsync(int artistId)
    {
        return await _context.Comics.Where(x => x.Artist.Id == artistId).ToListAsync(); 
    }

    public async Task AddAsync(Comic comic)
    {
        
        await _context.Comics.AddAsync(comic);
        await _context.SaveChangesAsync();
    }

    public async Task AddArtistToComic(int artistId, Comic comic)
    {
        
        var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
        if (artist == null)
        {
            return;
        }

        var newArtist = new Comic()
        {
            Artist = artist,
        };
        _context.Add(newArtist);
        _context.Add(comic);
        // await _context.SaveChangesAsync();
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