using Faker;
using ReadMangaTest.Models;

namespace ReadMangaTest.Data;

public class Seed
{
    private readonly DataContext _context;

    public Seed(DataContext context)
    {
        _context = context;
    }

    public void SeedDataContext()
    {
        Console.WriteLine("Seeding");
        if (_context.ComicCategories.Any())
        {
            Console.WriteLine("Creating");
            var comicCategories = new List<ComicCategory>();
            for (var i = 0; i < 777777; i++)
            {
                Console.WriteLine("Numb of data: "+i);
                var comicCategory = new ComicCategory()
                {
                    Comic = new Comic()
                    {
                        Name = "Comic "+Name.FullName(),
                        Description = Lorem.Sentence(),
                        Wallpaper = Internet.DomainWord(),
                        Url = Internet.Url(),
                        Artist = new Artist()
                        {
                            Name = "Artist "+Name.FullName(),
                            Description = Lorem.Sentence(),
                            Url = Internet.Url(),
                        },
                        Chapters = new List<Chapter>()
                        {
                            new Chapter()
                            {
                                Name = "Chapter "+ Name.FullName(),
                                Url = Internet.Url(),
                                Pages = new List<Page>()
                                {
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    }
                                }
                            },
                            new Chapter()
                            {
                                Name = "Chapter "+Name.FullName(),
                                Url = Internet.Url(),
                                Pages = new List<Page>()
                                {
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    },
                                    new Page()
                                    {
                                        Name = "Page "+Name.FullName(),
                                        Url = Internet.Url(),
                                    }
                                }
                            }
                        }
                    },
                    Category = new Category()
                    {
                        Name = " Category " + Name.FullName(),
                        Description = "DesCategory " + Lorem.Sentence(3),
                        Url = Internet.Url(),
                    }
                };
                comicCategories.Add(comicCategory);
            };
            _context.ComicCategories.AddRange(comicCategories);
            _context.SaveChanges();
        }
        Console.WriteLine("Done!");
    }
}