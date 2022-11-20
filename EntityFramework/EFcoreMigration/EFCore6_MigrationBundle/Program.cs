using EFCore6_MigrationBundle.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
            });
    })
    .Build();

await host.RunAsync();
