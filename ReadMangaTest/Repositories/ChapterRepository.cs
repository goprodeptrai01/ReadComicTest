using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class ChapterRepository : IChapterRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ChapterRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ChapterDto>> GetAllAsync()
    {
        var data = await _context.Chapters
            .Where(c => !c.IsHidden)
            .Include(c => c.Comic)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Content = c.Content,
                Comic = c.Comic.Name,
            }).ToListAsync();
        return data;
    }

    public async Task<ChapterDto> GetByIdAsync(int id)
    {
        var data = await _context.Chapters
            .Where(c => c.Id == id && !c.IsHidden)
            .Include(c => c.Comic)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Content = c.Content,
                Comic = c.Comic.Name,
            }).FirstOrDefaultAsync();
        if (data == null)
            return null;
        return _mapper.Map<ChapterDto>(data);
    }

    public async Task<List<ChapterDto>> GetChaptersByComicAsync(int comicId)
    {
        return await _context.Chapters
            .Where(c => !c.IsHidden && c.Comic.Id == comicId)
            .Include(c => c.Comic)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Content = c.Content,
                Comic = c.Comic.Name,
            }).ToListAsync();
    }

    public bool IsExists(int id)
    {
        return _context.Chapters.Any(e => e.Id == id &&!e.IsHidden);
    }

    public bool IsAvailable(string name, int id)
    {
        return _context.Chapters.Any(e => e.Name == name && e.Id != id);
    }
    public bool IsExists(string name, int id)
    {
        return _context.Chapters.Any(e => e.Name == name && e.Id == id);
    }

    public async Task<ChapterDto> AddAsync(ChapterDto chapterDto, int comicId)
    {
        try
        {

            var comic = await _context.Comics.FirstOrDefaultAsync(c => c.Id == comicId);
            if (comic == null)
            {
                throw new Exception("Comic not found");
            }
        
            var chapter = _mapper.Map<Chapter>(chapterDto);
            chapter.Comic = comic;
            await _context.Chapters.AddAsync(chapter);
            await _context.SaveChangesAsync();
            return _mapper.Map<ChapterDto>(chapter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to add chapter " + e);
        }
    }

    public async Task<ChapterDto> UpdateAsync(ChapterDto chapterDto, int id)
    {
        try
        {
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id && !c.IsHidden);
            if (chapter == null)
            {
                throw new Exception("Chapter not found");
            }
            
            _mapper.Map(chapterDto, chapter);
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
            return chapterDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to update chapter " + e);
        }
    }

    public async Task StoreAsync(int id)
    {
        try
        {
            var chapter = await _context.Chapters.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (chapter == null)
                throw new Exception("Chapter not found");
            
            chapter.IsHidden = !chapter.IsHidden;
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to store chapter " + e);
        }
    }

    public async Task MultiStoreAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Chapters.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more chapters Not Found");
            }
            
            var chapters = await _context.Chapters.Where(c => id.Contains(c.Id)).ToListAsync();
            foreach (var chapter in chapters)
            {
                chapter.IsHidden =!chapter.IsHidden;
            }
            _context.Chapters.UpdateRange(chapters);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error store");
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var chapter = await _context.Chapters.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (chapter == null)
                throw new Exception("Chapter not found");
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to store chapter " + e);
        }
    }

    public async Task MultiDeleteAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Chapters.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more chapters Not Found");
            }

            var chapters = await _context.Chapters.Where(c => id.Contains(c.Id)).ToListAsync();
            _context.Chapters.RemoveRange(chapters);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}