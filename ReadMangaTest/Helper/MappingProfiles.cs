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
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
    }
}