using HtmlAgilityPack;
using ReadMangaTest.Data;
using ReadMangaTest.Models;

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

    public List<Category> ScrapeCategory()
    {
        // URL of the web page you want to scrape data from
        var url = "https://www.nettruyenup.com";

        var categoryGrandNode = GetHtmlNode(url, "//body//nav[@id='mainNav']//li[@class='dropdown']");

        var categoryNodes = categoryGrandNode.SelectNodes(".//ul[@class='nav']//li");
        // Console.WriteLine(categoryNodes);
        var categories = new List<Category>();
        if (categoryNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return null;
        }


        foreach (var categoryNode in categoryNodes)
        {
            var detailUrls = categoryNode.SelectSingleNode(".//a").Attributes["href"].Value;

            var descriptionCategory = GetHtmlNode(detailUrls,
                    "//main[@class='main']//div[@class='container']//div[@id='ctl00_mainContent_ctl00_divDescription']//div")
                .InnerHtml.Trim();
            var nameCategory = "";
            if (categoryNode.SelectSingleNode(".//a//strong") is not null)
            {
                nameCategory = categoryNode.SelectSingleNode(".//a//strong").InnerText.Trim();
            }
            else
            {
                nameCategory = categoryNode.SelectSingleNode(".//a").InnerText.Trim();
            }

            var category = new Category()
            {
                Name = nameCategory,
                Description = descriptionCategory
            };
            // Console.WriteLine(category.Name);
            // Console.WriteLine(category.Description);
            categories.Add(category);
        }

        return categories;
        // return;
        _context.Categories.AddRange(categories);
        _context.SaveChanges();
        Console.WriteLine("Data has been saved to the database successfully");
    }

    public void ScrapeComicAndArtist()
    {
        // URL of the web page you want to scrape data from
        var url = "https://www.nettruyenup.com";

        var comicNodes = GetHtmlNodes(url,
            "//main[@class='main']//div[@class='container']//div[@class='row']//div[@class='row']//div[@class='item']");
        Console.WriteLine(1);
        if (comicNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return;
        }

        Console.WriteLine(2);
        Console.WriteLine($"Found {comicNodes.Count} items on the web page");

        // Loop through the selected HTML nodes and extract the data
        Console.WriteLine(3);
        var categoryList = ScrapeCategory();


        var comics = new List<Comic>();
        var artists = new List<Artist>();
        var chapters = new List<Chapter>();
        var comicCategories = new List<ComicCategory>();
        var nullArtist = new Artist()
        {
            Name = "IsUpdating",
            Description = "Is Updating!"
        };
        artists.Add(nullArtist);
        int count = 4;
        foreach (var comicNode in comicNodes)
        {
            Console.WriteLine(count);
            count++;
            string detailUrl = comicNode.SelectSingleNode(".//div//a").Attributes["href"].Value;

            var detailComicNode = GetHtmlNode(detailUrl,
                "//main[@class='main']//div[@class='container']//div[@class='row']//div[@class='detail-info']//ul[@class='list-info']");

            var nameArtist = detailComicNode.SelectSingleNode(".//li[@class='author row']//p[@class='col-xs-8']//a");

            var nameCategories = detailComicNode.SelectNodes(".//li[@class='kind row']//p[@class='col-xs-8']//a");

            var nodeChapters = GetHtmlNode(detailUrl,
                "//main[@class='main']//div[@class='container']//div[@class='row']//div[@class='list-chapter']");

            List<string> chapterUrls = new List<string>();
            // Console.WriteLine(urlChapters.InnerHtml.Trim());
            if (nodeChapters.SelectNodes("//li[@class='row']//a") != null)
                foreach (var urlChapter in nodeChapters.SelectNodes("//li[@class='row']//a"))
                {
                    var chapterUrl = urlChapter.Attributes["href"].Value;
                    chapterUrls.Add(chapterUrl);
                }

            if (nodeChapters.SelectNodes("//li[@class='row less']//a") != null)
                foreach (var urlChapter in nodeChapters.SelectNodes("//li[@class='row less']//a"))
                {
                    var chapterUrl = urlChapter.Attributes["href"].Value;
                    chapterUrls.Add(chapterUrl);
                }

            if (chapterUrls == null)
            {
                Console.WriteLine("url null " + count);
            }


            // return;
            Artist artist;
            if (nameArtist == null)
            {
                artist = nullArtist;
            }
            else
            {
                // Console.WriteLine(nameArtist.InnerHtml.Trim());

                artist = new Artist()
                {
                    Name = nameArtist.InnerHtml.Trim(),
                    Description = "",
                };
            }

            var comic = new Comic()
            {
                Name = comicNode.SelectSingleNode(".//h3//a").InnerHtml.Trim(),
                Description = comicNode.SelectSingleNode(".//div[@class='box_text']").InnerHtml.Trim(),
                Wallpaper = comicNode.SelectSingleNode(".//a//img").Attributes["data-original"].Value,
                Artist = artist
            };

            foreach (var nameCategory in nameCategories)
            {
                var categoryName = nameCategory.InnerHtml.Trim();
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
            }

            var chapterList = new List<Chapter>();
            foreach (var chapterUrl in chapterUrls)
            {
                var chapterNode = GetHtmlNode(chapterUrl,
                    "//main[@class='main']//div[@class='container']//div[@class='reading']");
                var nameChapter = chapterNode.SelectSingleNode(".//span[@itemprop='name']").InnerText.Trim();
                var contentNodes = chapterNode.SelectNodes(".//div[@class='page-chapter']");
                string contents = "";
                foreach (var contentNode in contentNodes)
                {
                    contents += ","+ contentNode.SelectSingleNode(".//img").Attributes["src"].Value;
                }
                
                var chapter = new Chapter()
                {
                    Name = nameChapter,
                    Content = contents,
                    Comic = comic,
                };
                chapterList.Add(chapter);
            }

            // Console.WriteLine(comic.Name);
            // Console.WriteLine(comic.Artist.Name);
            chapters.AddRange(chapterList);
            artists.Add(artist);
            comics.Add(comic);
        }

        artists = artists.Distinct().ToList();
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
        // Save the extracted data to the database
        _context.Artists.AddRange(artists);
        _context.Comics.AddRange(comics);
        _context.Chapters.AddRange(chapters);
        _context.Categories.AddRange(categoryList);
        _context.ComicCategories.AddRange(comicCategories);
        _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");
    }
}