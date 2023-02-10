using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Controllers;

[Route("v1/api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CategoryController(IComicRepository repository, IArtistRepository artistRepository, DataContext context,
        IMapper mapper, ICategoryRepository categoryRepository, IChapterRepository chapterRepository)
    {
        _comicRepository = repository;
        _artistRepository = artistRepository;
        _categoryRepository = categoryRepository;
        _chapterRepository = chapterRepository;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var categories = await _categoryRepository.GetAllAsync();

            return Ok(categories);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Category))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCategoryAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_categoryRepository.IsExists(id))
            return NotFound();
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            return Ok(category);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    
    /// <summary>
    /// Creates an Category.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/Employee
    ///     {        
    ///       "Name": "Action",
    ///       "Description": "Attack scenes"      
    ///     }
    /// </remarks>
    /// <param name="Category"></param>     
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Category))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostCategoryAsync([FromBody] CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (categoryDto == null) return BadRequest("Null");

        if (_categoryRepository.IsExists(categoryDto.Name, categoryDto.Id))
        {
            ModelState.AddModelError("Name", "Category already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            var category = _mapper.Map<Category>(categoryDto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(CategoryDto))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutCategoryAsync([FromRoute] int id, [FromBody] CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id == null)
        {
            return BadRequest("Null");
        }
        
        if (id != categoryDto.Id)
            return BadRequest("Invalid id");

        if (_categoryRepository.IsExists(categoryDto.Name, id))
        {
            ModelState.AddModelError("Name", "Category already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            await _categoryRepository.UpdateAsync(categoryDto, id);
            return Ok(categoryDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPut("{id}/store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _categoryRepository.StoreAsync(id);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }
    [HttpDelete("{id}/store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _categoryRepository.DeleteAsync(id);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }
}