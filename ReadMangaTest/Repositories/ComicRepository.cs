using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ComicRepository: IComicRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ComicRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ComicDto>> GetAllAsync()
    {
        return await _context.Comics.Where(c => c.IsHidden == false)
            .Include(c => c.ComicCategories)
            .ThenInclude(cc => cc.Category)
            .Include(c => c.Artist)
            .Select(c => new ComicDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Wallpaper = c.Wallpaper,
                Artist = c.Artist.Name,
                Categories = c.ComicCategories
                    .Where(cc => cc.IsHidden == false)
                    .Select(cc => cc.Category.Name)
                    .ToList()
            }).ToListAsync();
    }

    public async Task<Comic> GetByIdAsync(int id)
    {
        return await _context.Comics.Where(c => c.IsHidden == false && c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Comic> GetByNameAsync(string name)
    {
        return await _context.Comics.Where(c => c.IsHidden == false && c.Name == name).FirstOrDefaultAsync();

    }

    public async Task<IEnumerable<Comic>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Comics.Where(c => c.IsHidden == false && c.ComicCategories.Any(cc => cc.CategoryId == categoryId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Comic>> GetByArtistIdAsync(int artistId)
    {
        return await _context.Comics.Where(c => c.Artist.Id == artistId && c.IsHidden == false).ToListAsync(); 
    }

    public bool IsExists(int id)
    {
        return _context.Comics.Any(x => x.IsHidden == false && x.Id == id);
    }

    public bool IsExists(string name)
    {
        return _context.Comics.Any(x => x.IsHidden == false && x.Name == name);
    }

    public async Task<ComicDto> AddAsync(Comic comic)
    {
        
        await _context.Comics.AddAsync(comic);
        await _context.SaveChangesAsync();
        return _mapper.Map<ComicDto>(comic);
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

    public Task<bool> UpdateComicCategoryAsync(int id, int[] categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateComicArtistAsync(int id, int[] artistId)
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task MultiStoreAsync(int[] id)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        var comic = await _context.Comics.FindAsync(id);
        _context.Comics.Remove(comic);
        await _context.SaveChangesAsync();
    }

    public Task MultiDeleteAsync(int[] id)
    {
        throw new NotImplementedException();
    }
}