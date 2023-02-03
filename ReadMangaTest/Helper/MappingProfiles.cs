using AutoMapper;
using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Helper;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<Comic, ComicDto>();
        CreateMap<ComicDto, Comic>();
        CreateMap<ComicDto, PostComicDto>();
        CreateMap<Comic, PostComicDto>();
        CreateMap<PostComicDto, ComicDto>();
        CreateMap<PostComicDto, Comic>();
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
        CreateMap<Artist, ArtistDto>();
        CreateMap<Artist, PostArtistDto>();
        CreateMap<ArtistDto, Artist>();
        CreateMap<ArtistDto, PostArtistDto>();
        CreateMap<PostArtistDto, ArtistDto>();
        CreateMap<PostArtistDto, Artist>();
    }
}