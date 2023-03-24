using QLServer.Repositories;

namespace QLServer.GraphQL.Book;

public class BookQueries
{
    private readonly BookRepository _bookRepository;

    public BookQueries(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public IEnumerable<Models.Book> GetBooks() => _bookRepository.GetAllBooks();

    public Models.Book GetBook(int id) => _bookRepository.GetBookById(id);
}