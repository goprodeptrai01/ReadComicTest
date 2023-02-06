using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IArtistRepository
{
    //get api methods
    Task<List<ArtistDto>> GetAllAsync();
    Task<ArtistDto> GetByIdAsync(int id);
    Task<ArtistDto> GetByNameAsync(string name);
    //check methods
    bool IsExists(int id);
    bool IsExists(string name, int id);
    //post api method
    Task<ArtistDto> AddAsync(ArtistDto artist);
    //put api method
    Task<ArtistDto> UpdateAsync(ArtistDto artistDto, int id);
    //remove, store api methods
    Task StoreAsync(int id);
    Task DeleteAsync(int id);
}