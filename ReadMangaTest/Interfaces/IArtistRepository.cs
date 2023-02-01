using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IArtistRepository
{
    Task<IList<Artist>> GetAllAsync();
    Task<Artist> GetByIdAsync(int id);
    Task<Artist> GetByNameAsync(string name);
    Task AddAsync(Artist artist);
    Task UpdateAsync(Artist artist);
    Task DeleteAsync(int id);
    Task<bool> IsArtistExistsAsync(int id);
    Task<bool> IsArtistExistsAsync(string name);
}