using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;

namespace ReadMangaTest.Controllers;

[Route("v1/api/page")]
[ApiController]
public class PageController :  ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IPageRepository _pageRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IUriService _uriService;

    public PageController(IArtistRepository artistRepository, IComicRepository comicRepository, ICategoryRepository categoryRepository, IChapterRepository chapterRepository, DataContext context, IMapper mapper, IUriService uriService)
    {
        _artistRepository = artistRepository;
        _comicRepository = comicRepository;
        _categoryRepository = categoryRepository;
        _chapterRepository = chapterRepository;
        _context = context;
        _mapper = mapper;
        _uriService = uriService;
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPagesAsync()
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var pages = await _pageRepository.GetAllAsync();

            return Ok(pages);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }

    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPageAsync(int id)
    {
        if (!_pageRepository.IsExists(id))
            return NotFound();

        var pageDto = await _pageRepository.GetByIdAsync(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pageDto);
    }
    
    [HttpGet("{chapterId}/chapter")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPagesByChapterAsync(int chapterId)
    {
        if (!_pageRepository.IsExists(chapterId))
            return NotFound();

        var pageDtos = await _pageRepository.GetPagesbyChapterAsync(chapterId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pageDtos);
    }

    [HttpPost("{chapterId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComicAsync([FromBody] PageDto pageDto, [FromRoute] int chapterId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (pageDto == null)
            return BadRequest("Null");

        if (_pageRepository.IsExists(pageDto.Name, pageDto.Id))
        {
            ModelState.AddModelError("Name", "Page already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            await _pageRepository.AddAsync(pageDto, chapterId);
            return Ok(pageDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComicAsync([FromRoute] int id, [FromBody] PageDto pageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (pageDto == null)
        {
            return BadRequest("Null");
        }
        
        if (id!= pageDto.Id)
            return BadRequest("Invalid page id");

        if (_pageRepository.IsAvailable(pageDto.Name, id))
        {
            ModelState.AddModelError("Name", "Page already exists");
            return StatusCode(422, ModelState);
        }
        
        try
        {
            await _pageRepository.UpdateAsync(pageDto, id);
            return Ok(pageDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPut("{id}/store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _pageRepository.StoreAsync(id);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpPut("{id}/multipal-store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiStoreAsync([FromRoute] string ids)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idArray = ids.Split(',').Select(int.Parse).ToArray();
            
            await _pageRepository.MultiStoreAsync(idArray);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _pageRepository.DeleteAsync(id);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{id}/multipal-delete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiDeleteAsync([FromRoute] string ids)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idArray = ids.Split(',').Select(int.Parse).ToArray();
            
            await _pageRepository.MultiDeleteAsync(idArray);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}