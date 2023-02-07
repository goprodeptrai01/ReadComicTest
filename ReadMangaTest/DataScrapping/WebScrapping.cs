using HtmlAgilityPack;
using ReadMangaTest.Data;
using ReadMangaTest.Models;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace ReadMangaTest.DataScrapping;

public class WebScrapping
{
    private readonly DataContext _context;

    public WebScrapping(DataContext context)
    {
        Console.WriteLine("welcome to scraping");
        _context = context;
    }

    private HtmlNodeCollection GetHtmlNodes(string url, string xPath)
    {
        // Use HtmlWeb to download the HTML content of the web page
        var htmlWeb = new HtmlWeb();
        var htmlDocument = htmlWeb.Load(url);

        // Use BeautifulSoup to parse the HTML content
        var htmlSoup = new HtmlAgilityPack.HtmlDocument();
        htmlSoup.LoadHtml(htmlDocument.DocumentNode.OuterHtml);

        // Select the HTML nodes that contain the data you want to scrape
        return htmlSoup.DocumentNode.SelectNodes(xPath);
    }

    private HtmlNode GetHtmlNode(string url, string xPath)
    {
        // Use HtmlWeb to download the HTML content of the web page
        var htmlWeb = new HtmlWeb();
        var htmlDocument = htmlWeb.Load(url);

        // Use BeautifulSoup to parse the HTML content
        var htmlSoup = new HtmlAgilityPack.HtmlDocument();
        htmlSoup.LoadHtml(htmlDocument.DocumentNode.OuterHtml);

        // Select the HTML nodes that contain the data you want to scrape
        return htmlSoup.DocumentNode.SelectSingleNode(xPath);
    }

    // public List<Category> ScrapeCategory()
    public List<Category> ScrapeCategory()
    {
        // URL of the web page you want to scrape data from
        ScrapingBrowser browser = new ScrapingBrowser();
        var url = "https://kunmanga.com/manga-genre/action/";
        WebPage htmlPage = browser.NavigateToPage(new Uri(url));
        var categories = new List<Category>();

        var categoryNodes = htmlPage.Html.CssSelect(".genres_wrap").FirstOrDefault()
            .SelectNodes(".//li[@class='col-6 col-sm-4 col-md-2']");
        if (categoryNodes == null)
        {
            Console.WriteLine("No categories");
            return null;
        }

        foreach (var categoryNode in categoryNodes)
        {
            var category = new Category();
            var nameCategory = categoryNode.SelectSingleNode(".//a").InnerText.Trim().Split('\n').ToArray()[0];
            category.Name = nameCategory;
            var urlCategory = categoryNode.SelectSingleNode(".//a").Attributes["href"].Value;
            category.Url = urlCategory;
            category.Description = "";
            // Console.WriteLine(nameCategory);
            // Console.WriteLine(urlCategory);
            // Console.WriteLine(category.Description);
            categories.Add(category);
        }

        return categories;
    }

    public void ScrapeComicAndArtist()
    {
        ScrapingBrowser browser = new ScrapingBrowser();
        var count = 0;
        var url = "https://kunmanga.com";
        var categoryList = ScrapeCategory();
        var comics = new List<Comic>();
        var artists = new List<Artist>();
        var nullArtist = new Artist
        {
            Name = "IsUpdating",
            Description = "Is Updating!",
            Url = "IsUpdating!"
        };
        artists.Add(nullArtist);
        var nullComic = new Comic()
        {
            Name = "null",
            Description = "null",
            Wallpaper = "null",
            Url = "null",
            Artist = nullArtist,
            IsHidden = true,
        };
        comics.Add(nullComic);
        var chapters = new List<Chapter>();
        var nullChapter = new Chapter()
        {
            Name = "null",
            Url = "null",
            IsHidden = true,
        };
        chapters.Add(nullChapter);
        var pages = new List<Page>();
        var nullPage = new Page()
        {
            Name = "null",
            Url = "null",
            IsHidden = true,
        };
        pages.Add(nullPage);
        var comicCategories = new List<ComicCategory>();
        try
        {
            for (var i = 1; i <= 1; i++)
            {
                var urlPage = url + "/page";
                if (i > 1)
                {
                    urlPage += $"/{i}";
                }

                WebPage htmlPage = browser.NavigateToPage(new Uri(url));
                var comicRowNodes = htmlPage.Html.CssSelect(".page-listing-item");
                foreach (var comicRowNode in comicRowNodes)
                {
                    var comicNodes = comicRowNode.SelectNodes("./div/div");
                    if (comicNodes != null)
                    {
                        foreach (var comicNode in comicNodes)
                        {
                            count++;
                            Console.WriteLine(count);
                            var comic = new Comic();
                            string comicUrl = comicNode.SelectSingleNode(".//a").Attributes["href"].Value;
                            string wallpaperComic = comicNode.SelectSingleNode(".//a//img")?.Attributes["src"].Value;
                            comic.Wallpaper = wallpaperComic;
                            comic.Url = comicUrl;
                            Console.WriteLine(comicUrl);
                            var detailComic = browser.NavigateToPage(new Uri(comicUrl)).Html;
                            var comicName = detailComic.CssSelect(".site-content").CssSelect(".post-title")
                                .FirstOrDefault()
                                ?.SelectSingleNode("//h1").InnerHtml.Trim();
                            comic.Name = comicName;
                            Console.WriteLine(comicName);
                            var infoNodes = detailComic.CssSelect(".site-content").CssSelect(".post-content_item")
                                .ToArray();
                            var infoArtistComic = infoNodes.CssSelect(".author-content")
                                .FirstOrDefault();
                            var artist = nullArtist;
                            if (infoArtistComic != null)
                            {
                                var artistName = infoArtistComic.SelectSingleNode(".//a").InnerHtml.Trim();
                                Console.WriteLine(artistName);
                                var artistUrl = infoArtistComic.SelectSingleNode(".//a").Attributes["href"].Value;
                                Console.WriteLine(artistUrl);
                                artist = new Artist()
                                {
                                    Name = artistName,
                                    Url = artistUrl,
                                    Description = "",
                                };
                            }

                            comic.Artist = artist;

                            var infoCategoryComics =
                                infoNodes.CssSelect(".genres-content").ToArray()[0].SelectNodes(".//a");
                            if (infoCategoryComics != null)
                            {
                                foreach (var infoCategoryComic in infoCategoryComics)
                                {
                                    var categoryName = infoCategoryComic.InnerHtml.Trim();
                                    Console.WriteLine(categoryName);
                                    foreach (var category in categoryList)
                                    {
                                        if (category.Name.Equals(categoryName))
                                        {
                                            var comicCategory = new ComicCategory()
                                            {
                                                Comic = comic,
                                                Category = category
                                            };
                                            comicCategories.Add(comicCategory);
                                        }
                                    }
                                    // var categoryUrl = infoCategoryComic.Attributes["href"].Value;
                                    // Console.WriteLine(categoryUrl);
                                }
                            }

                            var infoChapterComicNode = detailComic.CssSelect(".site-content").FirstOrDefault();
                            // Console.WriteLine("DEBUG: " + infoChapterComicNode.InnerHtml);
                            var comicDescriptionNode = infoChapterComicNode
                                ?.SelectSingleNode(".//div[@class='c-page-content style-1']")
                                .SelectSingleNode(".//div[@class='description-summary']");
                            var comicDescription = comicDescriptionNode != null
                                ? comicDescriptionNode.SelectSingleNode(".//div//div[@class='limit-html']") != null
                                    ? comicDescriptionNode.SelectSingleNode(".//div//div[@class='limit-html']")
                                        .InnerHtml
                                        .Trim()
                                    : comicDescriptionNode.SelectSingleNode(".//div//div[2]") != null
                                        ? comicDescriptionNode.SelectSingleNode(".//div//div[2]").InnerHtml.Trim()
                                        : comicDescriptionNode.SelectSingleNode(".//div//div") != null
                                            ? comicDescriptionNode.SelectSingleNode(".//div//div").InnerHtml.Trim()
                                            : comicDescriptionNode.SelectSingleNode(".//div//p[2]") != null
                                                ? comicDescriptionNode.SelectSingleNode(".//div//p[2]").InnerHtml.Trim()
                                                : comicDescriptionNode.SelectSingleNode(".//div//p").InnerHtml.Trim()
                                : "";
                            comic.Description = comicDescription;
                            Console.WriteLine(comicDescription);
                            var chapterComicNodes = infoChapterComicNode.CssSelect(".wp-manga-chapter    ").ToArray();
                            if (chapterComicNodes.Length > 0)
                            {
                                foreach (var chapterComicNode in chapterComicNodes)
                                {
                                    var chapterName = chapterComicNode.SelectSingleNode(".//a").InnerHtml;
                                    Console.WriteLine(chapterName);
                                    var chapterUrl = chapterComicNode.SelectSingleNode(".//a").Attributes["href"].Value;
                                    Console.WriteLine(chapterUrl);
                                    var chapter = new Chapter()
                                    {
                                        Name = chapterName,
                                        Url = chapterUrl,
                                        Comic = comic,
                                    };
                                    var chapterPage = browser.NavigateToPage(new Uri(chapterUrl));
                                    var pageNodes = chapterPage.Html.SelectNodes(".//div[@class='page-break no-gaps']");
                                    var countPage = 1;
                                    if (pageNodes != null)
                                    {
                                        foreach (var pageNode in pageNodes)
                                        {
                                            var pageName = countPage + "/" + pageNodes.Count();
                                            Console.WriteLine(pageName);
                                            countPage++;
                                            var pageUrl = pageNode.SelectSingleNode(".//img").Attributes["src"].Value
                                                .Trim();
                                            Console.WriteLine(pageUrl);
                                            var page = new Page
                                            {
                                                Name = pageName,
                                                Url = pageUrl,
                                                Chapter = chapter
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            _context.Artists.AddRange(artists);
            _context.Comics.AddRange(comics);
            _context.Chapters.AddRange(chapters);
            _context.Categories.AddRange(categoryList);
            _context.ComicCategories.AddRange(comicCategories);
            _context.SaveChanges();

            throw new Exception("Data has been saved to the database successfully but not enough! Error message: " + e.Message);
        }

        // Save the extracted data to the database
        _context.Artists.AddRange(artists);
        _context.Comics.AddRange(comics);
        _context.Chapters.AddRange(chapters);
        _context.Categories.AddRange(categoryList);
        _context.ComicCategories.AddRange(comicCategories);
        _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");

        // Console.WriteLine("\nManga: ");
        // foreach (var comic in comics)
        // {
        //     Console.WriteLine(comic.Id);
        //     Console.WriteLine(comic.Name);
        // }
        // Console.WriteLine("\nArtist: ");
        // foreach (var artist in artists)
        // {
        //     Console.WriteLine(artist.Id);
        //     Console.WriteLine(artist.Name);
        // }
        // Console.WriteLine("\nCategory: ");
        // foreach (var category in categoryList)
        // {
        //     Console.WriteLine(category.Id);
        //     Console.WriteLine(category.Name);
        // }
        // Console.WriteLine("\nComic category: ");
        // foreach (var comicCategory in comicCategories)
        // {
        //     Console.WriteLine("Comic: "+comicCategory.ComicId);
        //     Console.WriteLine("Category: "+comicCategory.CategoryId);
        // }
        // return;
    }
}