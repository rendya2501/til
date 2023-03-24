using QLServer.Repositories;

namespace QLServer.GraphQL.Book;

public class BookMutations
{
    private readonly BookRepository _bookRepository;

    public BookMutations(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public int CreateBook(Models.BookInput book) => _bookRepository.CreateBook(book);

    public Models.Book? UpdateBook(int id, string title) => _bookRepository.UpdateBook(id, title);

    public bool DeleteBook(int id) => _bookRepository.DeleteBook(id);
}