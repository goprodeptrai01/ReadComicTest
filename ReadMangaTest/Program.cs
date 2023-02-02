using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Data;
using ReadMangaTest.DataScrapping;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// builder.Services.AddTransient<Seed>();
builder.Services.AddTransient<WebScrapping>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IComicRepository, ComicRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#pragma warning disable [warning code]
var app = builder.Build();
#pragma warning disable [warning code]
// if (args.Length == 1 && args[0].ToLower() == "seeddata")
//     SeedData(app);
//
// void SeedData(IHost app)
// {
//     var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
//
//     if (scopedFactory != null)
//         using (var scope = scopedFactory.CreateScope())
//         {
//             var service = scope.ServiceProvider.GetService<Seed>();
//             Console.WriteLine("Seeding database..." + service);
//             if (service != null) service.SeedDataContext();
//         }
// }

if (args.Length == 1 && args[0].ToLower() == "scrapecomic")
    ScrapeComic(app);
void ScrapeComic(IHost app)
{
    Console.WriteLine("Starting scraping...");
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<WebScrapping>();
            if (service != null)
            {
                Console.WriteLine("Scraping...");
                service.ScrapeComicAndArtist();
            }
        } 
    }
}

if (args.Length == 1 && args[0].ToLower() == "scrapecategory")
    ScrapeCategory(app);
void ScrapeCategory(IHost app)
{
    Console.WriteLine("Starting scraping...");
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<WebScrapping>();
            if (service != null)
            {
                Console.WriteLine("Scraping...");
                service.ScrapeCategory();
            }
        } 
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();