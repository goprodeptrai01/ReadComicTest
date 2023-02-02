using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ComicRepository : IComicRepository
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
                    .Select(cc => cc.Category.Name)
                    .ToList()
            }).ToListAsync();
    }

    public async Task<ComicDto> GetByIdAsync(int id)
    {
        var comic = await _context.Comics.Where(c => c.IsHidden == false && c.Id == id)
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
                    .Select(cc => cc.Category.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();
        if (comic == null)
        {
            return null;
        }   
        return _mapper.Map<ComicDto>(comic);
    }

    public async Task<ComicDto> GetByNameAsync(string name)
    {
        var comic = await _context.Comics.Where(c => c.IsHidden == false && c.Name == name)
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
                    .Select(cc => cc.Category.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();
        if (comic == null)
        {
            return null;
        }   
        return _mapper.Map<ComicDto>(comic);
    }

    public async Task<IEnumerable<ComicDto>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Comics
            .Where(c => c.IsHidden == false && c.ComicCategories.Any(cc => cc.CategoryId == categoryId))
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
                    .Select(cc => cc.Category.Name)
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ComicDto>> GetByArtistIdAsync(int artistId)
    {
        return await _context.Comics.Where(c => c.Artist.Id == artistId && c.IsHidden == false)
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
                    .Select(cc => cc.Category.Name)
                    .ToList()
            })
            .ToListAsync();
    }

    public bool IsExists(int id)
    {
        return _context.Comics.Any(x => x.IsHidden == false && x.Id == id);
    }

    public bool IsExists(string name)
    {
        return _context.Comics.Any(x => x.IsHidden == false && x.Name == name);
    }

    public async Task<ComicDto> AddAsync(PostComicDto comicDto, int[] categoryIds, int artistId)
    {
        try
        {
            var categories = _context.Categories.Count(c => categoryIds.Contains(c.Id) && !c.IsHidden);
            if (categories != categoryIds.Length)
            {
                throw new Exception("one or more categories Not found");
            }

            var artist = await _context.Artists.Where(a => a.Id == artistId && !a.IsHidden).FirstOrDefaultAsync();
            if (artist == null)
            {
                throw new Exception("Artist Not Found");
            }

            var comic = _mapper.Map<Comic>(comicDto);
            comic.Artist = artist;
            await _context.Comics.AddAsync(comic);
            await _context.SaveChangesAsync();


            var comicCategories = new List<ComicCategory>();
            foreach (var categoryId in categoryIds)
            {
                var comicCategory = new ComicCategory
                {
                    ComicId = comic.Id,
                    CategoryId = categoryId,
                };
                comicCategories.Add(comicCategory);
            }

            _context.ComicCategories.AddRange(comicCategories);
            await _context.SaveChangesAsync();
            return _mapper.Map<ComicDto>(comicDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ComicDto> UpdateAsync(PostComicDto comicDto, int comicId)
    {
        try
        {
            var comic = await _context.Comics.Where(c => c.Id == comicId && !c.IsHidden).FirstOrDefaultAsync();
            if (comic == null)
            {
                throw new Exception("Comic not found");
            }
            
            _mapper.Map(comicDto, comic);
            _context.Comics.Update(comic);
            await _context.SaveChangesAsync();
            return _mapper.Map<ComicDto>(comicDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateComicCategoryAsync(int id, int[] categoryIds)
    {
        try
        {
            var comic = await _context.Comics.Where(c => c.Id == id && !c.IsHidden).FirstOrDefaultAsync();

            if (comic == null)
                throw new Exception("Comic not found");

            var categories = _context.Categories.Count(c => categoryIds.Contains(c.Id) && !c.IsHidden);
            if (categories != categoryIds.Length)
            {
                throw new Exception("one or more categories Not found");
            }

            var idComicCategories = await _context.ComicCategories.Where(cc => cc.ComicId == id).ToListAsync();
            _context.ComicCategories.RemoveRange(idComicCategories);
            
            var comicCategories = new List<ComicCategory>();
            foreach (var categoryId in categoryIds)
            {
                var comicCategory = new ComicCategory
                {
                    CategoryId = categoryId,
                    ComicId = id
                };
                comicCategories.Add(comicCategory);
            }
            
            _context.ComicCategories.AddRange(comicCategories);
            await _context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateComicArtistAsync(int id, int artistId)
    {
        try
        {
            var comic = await _context.Comics.Where(c => c.Id == id && !c.IsHidden).FirstOrDefaultAsync();

            if (comic == null)
                throw new Exception("Comic not found");
            
            var artist = await _context.Artists.Where(a => a.Id == artistId && !a.IsHidden).FirstOrDefaultAsync();
            if (artist == null)
            {
                throw new Exception("Artist Not Found");
            }
            
            comic.Artist = artist;
            _context.Comics.Update(comic);
            await _context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task StoreAsync(int id)
    {
        try
        {
            var comic = await _context.Comics.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (comic == null)
                throw new Exception("Comic not found");
            
            comic.IsHidden = !comic.IsHidden;
            _context.Comics.Update(comic);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task MultiStoreAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Comics.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more comics Not Found");
            }
            
            var comics = await _context.Comics.Where(c => id.Contains(c.Id)).ToListAsync();
            foreach (var comic in comics)
            {
                comic.IsHidden =!comic.IsHidden;
            }
            _context.Comics.UpdateRange(comics);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var comic = await _context.Comics.FindAsync(id);
            if (comic == null)
                throw new Exception("Comic not found!");
            _context.Comics.Remove(comic);
            
            var comicCategories = await _context.ComicCategories.Where(cc => cc.ComicId == id).ToListAsync();
            if (comicCategories == null)
                throw new Exception("Comic category not found!");
            _context.ComicCategories.RemoveRange(comicCategories);

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);        }
    }

    public async Task MultiDeleteAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Comics.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more comics Not Found");
            }

            var comics = await _context.Comics.Where(c => id.Contains(c.Id)).ToListAsync();
            _context.Comics.RemoveRange(comics);
            
            var comicCategories = await _context.ComicCategories.Where(cc => id.Contains(cc.ComicId)).ToListAsync();
            if (comicCategories == null)
                throw new Exception("Comic category not found!");
            _context.ComicCategories.RemoveRange(comicCategories);
            
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}