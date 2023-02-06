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
        var url = "https://mangafreak.to/comics";

        var categoryGrandNode = GetHtmlNode(url, "/html/body/div/div/div/div[1]/div/div[1]/div/span");


        var categoryNodes = categoryGrandNode.SelectNodes(".//a");
        // Console.WriteLine(categoryNodes);
        var categories = new List<Category>();
        var categoryNull = new Category()
        {
            Name = "null",
            Description = "null",
            Url = "null",
            IsHidden = true
        };
        categories.Add(categoryNull);
        if (categoryNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return null;
        }

        foreach (var categoryNode in categoryNodes)
        {
            var categoryUrl = categoryNode.Attributes["href"].Value;
            var categoryName = categoryNode.InnerHtml.Trim();

            var category = new Category
            {
                Name = categoryName,
                Description = "Is Updating!",
                Url = "https://mangafreak.to" + categoryUrl
            };
            categories.Add(category);
        }

        return categories;

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
        var testUrl = "https://kunmanga.com/manga/the-lady-and-the-beast/chapter-106-5/";
        
        var test = GetHtmlNode(testUrl, "/html/body/div[1]/div/div[1]/div/div/div/div/div/div[2]/div[1]/div[2]/div/div/div[2]/div[2]/img").Attributes["src"].Value;
        

        Console.WriteLine(test);
        return;

        // URL of the web page you want to scrape data from
        var url = "https://mangafreak.to";
        int countItems = 0;
        var categoryList = ScrapeCategory();
        var comics = new List<Comic>();
        var artists = new List<Artist>();
        var nullArtist = new Artist
        {
            Name = "IsUpdating",
            Description = "Is Updating!",
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
        var comicCategories = new List<ComicCategory>();

        for (var i = 1; i <= 2; i++)
        {
            var urlPage = url + "/comics";
            if (i > 1)
            {
                urlPage += $"/?page={i}";
            }

            var comicNodes = GetHtmlNodes(urlPage,
                "/html/body/div/div/div/div[2]/div/div/div[1]/div[3]//div[@class= 'manga-item column']");

            // foreach (var comicNode in comicNodes)
            // {
            //     Console.WriteLine(comicNode.SelectSingleNode("./a[2]").InnerHtml.Trim());
            // }


            Console.WriteLine(1);
            if (comicNodes == null)
            {
                Console.WriteLine("No data found on the web page");
                return;
            }

            Console.WriteLine(2);
            // countItems += comicNodes.Count();

            Console.WriteLine(3);
            int count = 4;

            foreach (var comicNode in comicNodes)
            {
                Console.WriteLine(count);
                count++;
                string comicUrl = url + comicNode.SelectSingleNode("./a[1]").Attributes["href"].Value;
                Console.WriteLine(comicUrl);
                var detailComic = GetHtmlNode(comicUrl,
                    "/html/body/div/div/div/div[2]/div/div[2]/div[1]/div/div[2]");
                //html/body/div/div/div/div[2]/div/div[2]/div[1]/div/div[2]/div[2]/div[2]/h1
                var comicName = detailComic.SelectSingleNode("./div[2]/div[2]//h1").InnerHtml.Trim();
                Console.WriteLine(comicName);
                //html/body/div/div/div/div[2]/div/div[2]/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[2]/div/a[1]
                var artistName = detailComic.SelectSingleNode("./div[2]/div[2]/div[1]/div[2]/div/a[1]").InnerHtml
                    .Trim();
                Console.WriteLine(artistName);
                var categoryNames = detailComic.SelectNodes("./div[2]/div[2]/div[1]/div[5]/div/a");
                foreach (var categoryName in categoryNames)
                {
                    Console.WriteLine(categoryName.InnerHtml.Trim());
                }

                //html/body/div/div/div/div[2]/div/div[2]/div[1]/div/div[3]
                //*[@id="main-container"]/div/div[2]/div[1]/div/div[3]
                //*[@id="main-container"]/div/div[2]/div[1]/div/div[3]
                var lastChapterUrl = url + comicNode.SelectSingleNode("./a[3]").Attributes["href"].Value;
                //html/body/div/div/div/div[2]/div/div/div[3]/div
                var nodeChapter = GetHtmlNode(lastChapterUrl, "/html/body/div/div/div/div[2]/div/div");
                var infoChaperNodes = nodeChapter.SelectNodes("./div[3]/div/select/option");
                foreach (var infoChapter in infoChaperNodes)
                {
                    var chapterName = infoChapter.InnerHtml.Trim();
                    Console.WriteLine(chapterName);
                    var readUrl = comicUrl.Split('/').ToArray();
                    readUrl[3] = "reading";
                    var chapterUrl = string.Join("/", readUrl) + "/" + infoChapter.Attributes["value"].Value;
                    Console.WriteLine(chapterUrl);
                    var pageNode = GetHtmlNode(chapterUrl, "/html/body");
                    var pageNodes = pageNode.SelectNodes("//div//div//div[@class='default-container']");
                    foreach (var page in pageNodes)
                    {
                        Console.WriteLine(page.Attributes["id"].Value);
                    }
                }
            }

            return;
            foreach (var comicNode in comicNodes)
            {
                Console.WriteLine(count);
                count++;
                string detailUrl = comicNode.SelectSingleNode(".//div//a").Attributes["href"].Value;

                var detailComicNode = GetHtmlNode(detailUrl,
                    "//main[@class='main']//div[@class='container']//div[@class='row']//div[@class='detail-info']//ul[@class='list-info']");

                var nameArtist =
                    detailComicNode.SelectSingleNode(".//li[@class='author row']//p[@class='col-xs-8']//a");

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
                    string urls = "";
                    foreach (var contentNode in contentNodes)
                    {
                        urls += "," + contentNode.SelectSingleNode(".//img").Attributes["src"].Value;
                    }

                    var chapter = new Chapter()
                    {
                        Name = nameChapter,
                        Url = urls,
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
        }

        Console.WriteLine(countItems);
        return;

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