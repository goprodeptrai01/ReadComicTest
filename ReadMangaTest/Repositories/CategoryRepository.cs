using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;
using Exception = System.Exception;

namespace ReadMangaTest.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CategoryRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .Where(c => !c.IsHidden)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Where(c => !c.IsHidden)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync();
    }

    public bool IsExists(int id)
    {
        return _context.Categories.Any(x => x.Id == id && x.IsHidden == false);
    }

    public bool IsExists(string name, int id)
    {
        return _context.Categories.Any(x => x.Name == name && x.Id != id);
    }

    public async Task AddAsync(Category category)
    {
        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error adding category! "+e.Message);
        }
    }

    public async Task<CategoryDto> UpdateAsync(CategoryDto categoryDto, int id)
    {
        try
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id && !c.IsHidden);
            if (category == null)
            {
                throw new Exception("Category not found!");
            }
            
            _mapper.Map(categoryDto, category);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error updating category! "+e.Message);
        }
    }

    public async Task StoreAsync(int id)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found!");
            }
            
            category.IsHidden = !category.IsHidden;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error storing category! "+e.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new Exception("Category not found!");
            }
            _context.Categories.Remove(category);
            var comicCategories = await _context.ComicCategories.Where(cc => cc.CategoryId == id).ToListAsync();
            if (comicCategories == null)
                throw new Exception("Comic Category not found!");
            _context.ComicCategories.RemoveRange(comicCategories);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error deleting category! "+e.Message);
        }
    }
}