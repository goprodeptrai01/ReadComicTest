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
    }
}