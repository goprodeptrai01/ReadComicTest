using HtmlAgilityPack;
using ReadMangaTest.Data;
using ReadMangaTest.Models;

namespace ReadMangaTest.DataScrapping;

public class WebScrapping
{
    private readonly DataContext _context;

    public WebScrapping(DataContext context)
    {
        _context = context;
    }

    public void ScrapeData()
    {
        Console.WriteLine("welcome to scraping");
        // URL of the web page you want to scrape data from
        var url = "https://www.nettruyenup.com";

        // Use HtmlWeb to download the HTML content of the web page
        var htmlWeb = new HtmlWeb();
        var htmlDocument = htmlWeb.Load(url);

        // Use BeautifulSoup to parse the HTML content
        var htmlSoup = new HtmlAgilityPack.HtmlDocument();
        htmlSoup.LoadHtml(htmlDocument.DocumentNode.OuterHtml);

        // Select the HTML nodes that contain the data you want to scrape
        var comicNodes = htmlSoup.DocumentNode.SelectNodes(
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
            var detailHtmlWeb = new HtmlWeb();
            var detailHtmlWebHtmlDocument = htmlWeb.Load(detailUrl);

            var detailHtmlSoup = new HtmlAgilityPack.HtmlDocument();
            detailHtmlSoup.LoadHtml(detailHtmlWebHtmlDocument.DocumentNode.OuterHtml);

            var detailComicNode = detailHtmlSoup.DocumentNode.SelectSingleNode(
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

        // Save the extracted data to the database
        _context.Artists.AddRange(artists);
        _context.Comics.AddRange(comics);
        _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");
    }
}