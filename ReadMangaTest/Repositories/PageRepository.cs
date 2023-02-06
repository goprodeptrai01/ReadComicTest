using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Repositories;

public class PageRepository : IPageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public PageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PageDto>> GetAllAsync()
    {
        var data = await _context.Pages
            .Where(p => !p.IsHidden)
            .Include(p => p.Chapter)
            .Select(p => new PageDto
            {
                Id = p.Id,
                Name = p.Name,
                Url = p.Url,
                Chapter = p.Chapter.Name,
            }).ToListAsync();
        return data;
    }

    public async Task<PageDto> GetByIdAsync(int id)
    {
        var data = await _context.Pages
            .Where(p => p.Id == id && !p.IsHidden)
            .Include(p => p.Chapter)
            .Select(p => new PageDto
            {
                Id = p.Id,
                Name = p.Name,
                Url = p.Url,
                Chapter = p.Chapter.Name,
            }).FirstOrDefaultAsync();
        if (data == null)
            return null;
        return _mapper.Map<PageDto>(data);
    }

    public async Task<List<PageDto>> GetPagesbyChapterAsync(int chapterId)
    {
        return await _context.Pages
            .Where(p => !p.IsHidden && p.Chapter.Id == chapterId)
            .Include(p => p.Chapter)
            .Select(p => new PageDto()
            {
                Id = p.Id,
                Name = p.Name,
                Url = p.Url,
                Chapter = p.Chapter.Name,
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

    public async Task<PageDto> AddAsync(PageDto pageDto, int chapterId)
    {
        try
        {

            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
            if (chapter == null)
            {
                throw new Exception("Chapter not found!");
            }
        
            var page = _mapper.Map<Page>(pageDto);
            page.Chapter = chapter;
            await _context.Pages.AddAsync(page);
            await _context.SaveChangesAsync();
            return _mapper.Map<PageDto>(page);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to add page " + e);
        }
    }

    public async Task<PageDto> UpdateAsync(PageDto pageDto, int id)
    {
        try
        {
            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Id == id && !p.IsHidden);
            if (page == null)
            {
                throw new Exception("Page not found");
            }
            
            _mapper.Map(pageDto, page);
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();
            return pageDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to update page " + e);
        }
    }

    public async Task StoreAsync(int id)
    {
        try
        {
            var page = await _context.Pages.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (page == null)
                throw new Exception("Page not found");
            
            page.IsHidden = !page.IsHidden;
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to store page " + e);
        }
    }

    public async Task MultiStoreAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Pages.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more page Not Found");
            }
            
            var pages = await _context.Pages.Where(c => id.Contains(c.Id)).ToListAsync();
            foreach (var page in pages)
            {
                page.IsHidden =!page.IsHidden;
            }
            _context.Pages.UpdateRange(pages);
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
            var page = await _context.Pages.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (page == null)
                throw new Exception("Pages not found");
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to store page " + e);
        }
    }

    public async Task MultiDeleteAsync(int[] id)
    {
        try
        {
            var checkValid = _context.Pages.Count(c => id.Contains(c.Id));
            if (checkValid != id.Length)
            {
                throw new Exception("One or more pages Not Found");
            }

            var pages = await _context.Pages.Where(c => id.Contains(c.Id)).ToListAsync();
            _context.Pages.RemoveRange(pages);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}