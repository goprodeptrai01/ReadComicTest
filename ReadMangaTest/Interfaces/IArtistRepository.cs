using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IArtistRepository
{
    Task<IList<Artist>> GetAll();
    Task<Artist> GetById(int id);
    Task Add(Artist artist);
    Task Update(Artist artist);
    Task Delete(int id);
}