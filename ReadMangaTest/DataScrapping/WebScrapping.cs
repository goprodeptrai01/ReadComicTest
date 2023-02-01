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

    public void ScrapeCategory()
    {
        
        // URL of the web page you want to scrape data from
        var url = "https://www.nettruyenup.com";

        var categoryGrandNode = GetHtmlNode(url,"//body//nav[@id='mainNav']//li[@class='dropdown']//ul[@class='nav']//li");

        var categoryNodes = categoryGrandNode.SelectNodes(".//ul[@class='nav']//li");
        
        var categories = new List<Category>();
        
        foreach (var categoryNode in categoryNodes)
        {
            var nameCategory = "";
            if (categoryNode.SelectSingleNode(".//a//strong") is not null)
            {
                Console.WriteLine(categoryNode.SelectSingleNode(".//a//strong").InnerHtml.Trim());
                nameCategory = categoryNode.SelectSingleNode(".//a//strong").InnerText.Trim();
            }
            else
            {
                Console.WriteLine(categoryNode.SelectSingleNode(".//a").InnerHtml.Trim());
                nameCategory = categoryNode.SelectSingleNode(".//a").InnerText.Trim();
            }

            var category = new Category()
            {
                Name = nameCategory,
                Description = ""
            };
            Console.WriteLine(category.Name);
            categories.Add(category);
        }
        
        return;
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
        var comics = new List<Comic>();
        var artists = new List<Artist>();
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

            Artist artist;
            if (nameArtist == null)
            {
                artist = nullArtist;
            }
            else
            {
                Console.WriteLine(nameArtist.InnerHtml.Trim());

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
            Console.WriteLine(comic.Name);
            Console.WriteLine(comic.Artist.Name);
            artists.Add(artist);
            comics.Add(comic);
        }


        artists = artists.Distinct().ToList();
        return;
        // Save the extracted data to the database
        _context.Artists.AddRange(artists);
        _context.Comics.AddRange(comics);
        _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");
    }
}