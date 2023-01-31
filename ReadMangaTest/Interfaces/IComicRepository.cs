using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IComicRepository
{
    Task<IEnumerable<Comic>> GetAllAsync();
    Task<Comic> GetByIdAsync(int id);
    Task<IEnumerable<Comic>> GetByArtistIdAsync(int artistId);
    Task AddAsync(Comic comic);
    Task AddArtistToComic(int artistId, Comic comic);
    Task UpdateAsync(Comic comic);
    Task DeleteAsync(int id);
}