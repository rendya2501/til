var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL();

app.Run();



public class Query
{
    private readonly List<Book> _books = new()
    {
        new Book { Id = "1", Title = "GraphQLόε", Author = "Rc ΎY", Publisher = "ZpoΕ", PublicationDate = new DateTime(2021, 10, 1) },
        new Book { Id = "2", Title = "C#vO~O", Author = "ιΨ κY", Publisher = "ZpoΕ", PublicationDate = new DateTime(2020, 5, 15) },
        new Book { Id = "3", Title = ".NETJ­ΜΙΣ", Author = "²‘ Y", Publisher = "ZpoΕ", PublicationDate = new DateTime(2019, 11, 30) },
    };

    public List<Book> GetBooks() => _books;

    public Book GetBookById(string id) => _books.FirstOrDefault(x => x.Id == id);
}

public class Book
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public DateTime PublicationDate { get; set; }
}
