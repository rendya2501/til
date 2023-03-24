using QLServer.GraphQL.Book;
using QLServer.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<BookRepository>();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<BookQueries>()
    .AddMutationType<BookMutations>();

var app = builder.Build();

app.MapGraphQL();

app.Run();



//public class Query
//{
//    private readonly List<Book> _books = new()
//    {
//        new Book { Id = 1, Title = "GraphQL入門", Author = "山田 太郎", Publisher = "技術書出版", PublicationDate = new DateTime(2021, 10, 1) },
//        new Book { Id = 2, Title = "C#プログラミング", Author = "鈴木 一郎", Publisher = "技術書出版", PublicationDate = new DateTime(2020, 5, 15) },
//        new Book { Id = 3, Title = ".NET開発の極意", Author = "佐藤 次郎", Publisher = "技術書出版", PublicationDate = new DateTime(2019, 11, 30) },
//    };
//    private int _nextId = 1;
//    //public List<Book> GetBooks() => _books;

//    //public Book GetBookById(string id) => _books.FirstOrDefault(x => x.Id == id);

//    public IEnumerable<Book> GetAll() => _books;

//    public Book GetById(int id) => _books.FirstOrDefault(book => book.Id == id);


//    public Book Insert(Book book)
//    {
//        book.Id = _nextId++;
//        _books.Add(book);
//        return book;
//    }

//    public bool Update(int id, string title)
//    {
//        var book = GetById(id);
//        if (book == null) return false;

//        book.Title = title;
//        return true;
//    }

//    public bool Delete(int id)
//    {
//        var book = GetById(id);
//        if (book == null) return false;

//        _books.Remove(book);
//        return true;
//    }
//}
