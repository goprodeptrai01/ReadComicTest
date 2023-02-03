using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Filters;
using ReadMangaTest.Helper;
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
    private readonly IUriService _uriService;

    public ComicController(IComicRepository repository, IArtistRepository artistRepository, DataContext context,
        IMapper mapper, ICategoryRepository categoryRepository, IChapterRepository chapterRepository, IUriService uriService)
    {
        _comicRepository = repository;
        _artistRepository = artistRepository;
        _categoryRepository = categoryRepository;
        _chapterRepository = chapterRepository;
        _uriService = uriService;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetComics([FromQuery] PaginationFilter filter)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var route = Request.Path.Value;
            
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            
            var comics = await _comicRepository.GetAllAsync(filter);

            var totalRecords = await _context.Comics.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<ComicDto>(comics, validFilter, totalRecords, _uriService, route);
            return Ok(pagedReponse);
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
        if (!_artistRepository.IsExists(artistId))
            return NotFound();

        var comics = await _comicRepository.GetByArtistIdAsync(artistId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(comics);
    }

    [HttpPost("{categoryIds}&{artistId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComicAsync([FromBody] PostComicDto comicDto, [FromRoute] string categoryIds, [FromRoute] int artistId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (comicDto == null)
            return BadRequest("Null");

        if (_comicRepository.IsExists(comicDto.Name, comicDto.Id))
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
    [ProducesResponseType(200, Type = typeof(ComicDto))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComicAsync([FromRoute] int comicId, [FromBody] PostComicDto comicDto)
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

        if (_comicRepository.IsExists(comicDto.Name, comicId))
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
    public async Task<IActionResult> PutComicCategoryAsync([FromRoute] int comicId, [FromRoute] string categoryIds)
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
    [HttpPut("{comicId}&{artistId}/artist")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComicAsync([FromRoute] int comicId, [FromRoute] int artistId)
    {
        if (!_comicRepository.IsExists(comicId))
            return NotFound("comic is not exists");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            await _comicRepository.UpdateComicArtistAsync(comicId, artistId);
            return Ok("Updated!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPut("{comicId}/store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreAsync([FromRoute] int comicId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _comicRepository.StoreAsync(comicId);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpPut("{comicId}/multipal-store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiStoreAsync([FromRoute] string comicId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var comicIdArray = comicId.Split(',').Select(int.Parse).ToArray();
            
            await _comicRepository.MultiStoreAsync(comicIdArray);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{comicId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteAsync(int comicId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _comicRepository.DeleteAsync(comicId);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{comicId}/multipal-delete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiDeleteAsync([FromRoute] string comicId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var comicIdArray = comicId.Split(',').Select(int.Parse).ToArray();
            
            await _comicRepository.MultiDeleteAsync(comicIdArray);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    
}