using QLServer.Models;

namespace QLServer.Repositories;

public class BookRepository
{
    private readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "GraphQL入門", Author = "山田 太郎", Publisher = "技術書出版", PublicationDate = new DateTime(2021, 10, 1) },
        new Book { Id = 2, Title = "C#プログラミング", Author = "鈴木 一郎", Publisher = "技術書出版", PublicationDate = new DateTime(2020, 5, 15) },
        new Book { Id = 3, Title = ".NET開発の極意", Author = "佐藤 次郎", Publisher = "技術書出版", PublicationDate = new DateTime(2019, 11, 30) },
    };

    public IEnumerable<Book> GetAllBooks() => _books;

    public Book GetBookById(int id) => _books.FirstOrDefault(book => book.Id == id);

    public int CreateBook(BookInput inputBook)
    {
        var newBook = new Book()
        {
            Id = _books.Max(book => book.Id) + 1,
            Title = inputBook.Title,
            Author = inputBook.Author,
            Publisher = inputBook.Publisher,
            PublicationDate = inputBook.PublicationDate,
        };
        _books.Add(newBook);
        return newBook.Id;
    }

    public Book? UpdateBook(int id, string title)
    {
        var book = GetBookById(id);
        if (book == null) return null;

        book.Title = title;
        return book;
    }

    public bool DeleteBook(int id)
    {
        var book = GetBookById(id);
        if (book == null) return false;

        _books.Remove(book);
        return true;
    }
}
