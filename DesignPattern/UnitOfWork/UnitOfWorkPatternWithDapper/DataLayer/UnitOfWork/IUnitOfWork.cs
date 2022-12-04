using DataLayer.Repositories.BookAggregation;
using DataLayer.Repositories.CatalogueAggregation;

namespace DataLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IBookRepository BookRepository { get; }
    ICatalogueRepository CatalogueRepository { get; }

    void Commit();
}