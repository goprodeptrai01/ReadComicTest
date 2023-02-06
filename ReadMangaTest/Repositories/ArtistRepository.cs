using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;
using NotImplementedException = System.NotImplementedException;

namespace ReadMangaTest.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ArtistRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<List<ArtistDto>> GetAllAsync()
    {
        var data = await _context.Artists
            .Where(a => !a.IsHidden)
            .Include(a => a.Comics)
            .Select(a => new ArtistDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Comics = a.Comics
                    .Select(c => c.Name)
                    .ToList(),
            }).ToListAsync();
        return data;
    }

    public async Task<ArtistDto> GetByIdAsync(int id)
    {
        try
        {
            var artist = await _context.Artists
                .Where(a => a.Id == id && !a.IsHidden)
                .Include(a => a.Comics)
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Comics = a.Comics
                        .Select(c => c.Name)
                        .ToList(),
                }).FirstOrDefaultAsync();
            if (artist == null)
            {
                return null;
            }

            return _mapper.Map<ArtistDto>(artist);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<ArtistDto> GetByNameAsync(string name)
    {
        try
        {
            var artist = await _context.Artists
                .Where(a => a.Name == name && !a.IsHidden)
                .Include(a => a.Comics)
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Comics = a.Comics
                        .Select(c => c.Name)
                        .ToList(),
                }).FirstOrDefaultAsync();
            if (artist == null)
            {
                return null;
            }

            return _mapper.Map<ArtistDto>(artist);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public bool IsExists(int id)
    {
        return _context.Artists.Any(a => a.Id == id &&!a.IsHidden);
    }

    public bool IsExists(string name, int id)
    {
        return _context.Artists.Any(a => a.Name == name && a.Id != id);
    }

    public async Task<ArtistDto> AddAsync(ArtistDto artistDto)
    {
        try
        {
            var artist = _mapper.Map<Artist>(artistDto);
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
            return _mapper.Map<ArtistDto>(artist);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<ArtistDto> UpdateAsync(ArtistDto artistDto, int id)
    {
        try
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == id && !a.IsHidden);
            if (artist == null)
            {
                throw new Exception("Artist not found");
            }
            
            _mapper.Map(artistDto, artist);
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();
            return _mapper.Map<ArtistDto>(artist);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task StoreAsync(int id)
    {
        try
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
                throw new Exception("Artist not found");
            artist.IsHidden = !artist.IsHidden;
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
                throw new Exception("Artist not found");
            _context.Artists.Remove(artist);
            
            var comics = await _context.Comics.Where(c => c.Artist == artist).ToListAsync();
            if (comics!= null)
            {
                var artistNull = await _context.Artists.FindAsync(1);
                if (artistNull == null)
                {
                    throw new Exception("Artist null not found");
                }
                foreach (var comic in comics)
                {
                    comic.Artist = artistNull;
                    _context.Comics.Update(comic);
                }   
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message, e.InnerException);
        }
    }
}