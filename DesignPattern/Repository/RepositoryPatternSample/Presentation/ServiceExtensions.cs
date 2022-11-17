using DataLayer.Repositories.BookAggregation;
using DataLayer.Repositories.CatalogueAggregation;

namespace Presentation;

public static class ServiceExtensions
{
    public static void AddApplication(this IServiceCollection service)
    {
        service.AddTransient<IBookRepository, BookRepository>();
        service.AddTransient<ICatalogueRepository, CatalogueRepository>();
    }
}
