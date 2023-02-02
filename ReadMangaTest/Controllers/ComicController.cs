using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Controllers;

[Route("v1/api/comic")]
[ApiController]
public class ComicController : ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ComicController(IComicRepository repository, IArtistRepository artistRepository, DataContext context,
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
    [ProducesResponseType(200, Type = typeof(IEnumerable<Comic>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetComics()
    {
        try
        {
            var comics = await _comicRepository.GetAllAsync();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(comics);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }

    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetComicAsync(int id)
    {
        if (!_comicRepository.IsExists(id))
            return NotFound();

        var comic = await _comicRepository.GetByIdAsync(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(comic);
    }
    
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Comic>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetComicByCategoryAsync(int categoryId)
    {
        if (!_categoryRepository.IsExists(categoryId))
            return NotFound();

        var comics = await _comicRepository.GetByCategoryAsync(categoryId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(comics);
    }
    
    [HttpGet("artist/{artistId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Comic>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetComicByArtistAsync(int artistId)
    {
        if (!_categoryRepository.IsExists(artistId))
            return NotFound();

        var comics = await _comicRepository.GetByArtistIdAsync(artistId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(comics);
    }

    [HttpPost("{categoryIds}&{artistId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComic([FromBody] PostComicDto comicDto, [FromRoute] string categoryIds, [FromRoute] int artistId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (comicDto == null)
            return BadRequest("Null");

        if (_comicRepository.IsExists(comicDto.Name))
        {
            ModelState.AddModelError("Name", "Comic already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            var categoryIdArray = categoryIds.Split(',').Select(int.Parse).ToArray();
            await _comicRepository.AddAsync(comicDto, categoryIdArray, artistId);
            return Ok(comicDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPut("{comicId}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComic([FromRoute] int comicId, [FromBody] PostComicDto comicDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (comicId == null)
        {
            return BadRequest("Null");
        }
        
        if (comicId!= comicDto.Id)
            return BadRequest("Invalid comic id");

        if (_comicRepository.IsExists(comicDto.Name))
        {
            ModelState.AddModelError("Name", "Comic already exists");
            return StatusCode(422, ModelState);
        }
        
        try
        {
            await _comicRepository.UpdateAsync(comicDto, comicId);
            return Ok(comicDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPut("{comicId}&{categoryIds}/category")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComicCategory([FromRoute] int comicId, [FromRoute] string categoryIds)
    {
        if (!_comicRepository.IsExists(comicId))
            return NotFound("comic is not exists");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var categoryIdArray = categoryIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            await _comicRepository.UpdateComicCategoryAsync(comicId, categoryIdArray);
            return Ok("Updated!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteComic(int id)
    {
        var comic = await _comicRepository.GetByIdAsync(id);
        if (comic == null)
        {
            return NotFound();
        }

        await _comicRepository.DeleteAsync(id);
        return NoContent();
    }
}