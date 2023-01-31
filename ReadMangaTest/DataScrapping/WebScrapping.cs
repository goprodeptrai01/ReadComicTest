using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.Models;

namespace ReadMangaTest.DataScrapping;

public class WebScrapping
{
    public void ScrapeData()
    {
        // URL of the web page you want to scrape data from
        var url = "https://www.nettruyenup.com";

        // Use HtmlWeb to download the HTML content of the web page
        var htmlWeb = new HtmlWeb();
        var htmlDoc = htmlWeb.Load(url);

        // Use BeautifulSoup to parse the HTML content
        var htmlSoup = new HtmlAgilityPack.HtmlDocument();
        htmlSoup.LoadHtml(htmlDoc.DocumentNode.OuterHtml);

        // Select the HTML nodes that contain the data you want to scrape
        var comicNodes = htmlSoup.DocumentNode.SelectNodes("//div[@class='book-item']");
        if (comicNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return;
        }

        // Loop through the selected HTML nodes and extract the data
        var comics = new List<Comic>();
        foreach (var comicNode in comicNodes)
        {
            var comic = new Comic
            {
                Name = comicNode.SelectSingleNode(".//h3").InnerHtml.Trim(),
                Description = comicNode.SelectSingleNode(".//p[@class='book-desc']").InnerHtml.Trim(),
                Wallpaper = comicNode.SelectSingleNode(".//img").Attributes["src"].Value
            };

            comics.Add(comic);
        }

        // Save the extracted data to the database
        using (var context = new DataContext(new DbContextOptions<DataContext>()))
        {
            context.Comics.AddRange(comics);
            context.SaveChanges();
        }

        Console.WriteLine("Data has been saved to the database successfully");
    }
}