using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .Build();
var section = configuration.GetSection("testsection");
var sectionExists = section.Exists();
var sectionExists2 = configuration.GetChildren().Any(item => item.Key == "testsection");



IConfigurationRoot configurationno = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings_test.json", true, true)
    .Build();
var section2 = configuration.GetSection("testsection");
var sectionExists5 = section.Exists();
var sectionExists3 = configuration.GetChildren().Any(item => item.Key == "testsection");



using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                var aa = hostContext.Configuration;
                var section = aa.GetSection("");
                section.Exists();
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings);
            });
    })
    .Build();

public class DatContext : DbContext
{
    public DatContext(DbContextOptions<DatContext> options) : base(options) { }
    public DbSet<Person> Person { get; set; }
}

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }
}