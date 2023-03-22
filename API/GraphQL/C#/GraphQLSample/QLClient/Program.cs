using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;


var endpoint = @"https://localhost:7167/graphql"; // GraphQL サーバーのエンドポイントを設定

using var graphQLClient = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer());

// すべての本を取得するクエリを実行 (タイトルと著者だけをリクエスト)
var getAllBooksQuery = new GraphQLRequest
{
    Query = @"
    query GetBooks(){
        books  {
            id
            title
            author
            publisher
            publicationDate
        }
    }"
};

var allBooksResponse = await graphQLClient.SendQueryAsync<BooksResponse>(getAllBooksQuery);

Console.WriteLine("All books (Title and Author):");
foreach (var book in allBooksResponse.Data.Books ?? Enumerable.Empty<Book>())
{
    Console.WriteLine($"Title: {book.Title}, Author: {book.Author}");
}
Console.WriteLine();

// ID に基づいて特定の本を取得するクエリを実行 (すべてのフィールドをリクエスト)
var getBookByIdQuery = new GraphQLRequest
{
    Query = @"
    query GetBookById($id: String!) {
      bookById (id: $id) {
        id
        title
        author
        publisher
        publicationDate
      }
    }",
    Variables = new { id = "1" }
};

var bookByIdResponse = await graphQLClient.SendQueryAsync<BookByIdResponse>(getBookByIdQuery);

Console.WriteLine("Book by ID (All fields):");
Console.WriteLine($"" +
    $"ID: {bookByIdResponse.Data.BookById.Id}, " +
    $"Title: {bookByIdResponse.Data.BookById.Title}, " +
    $"Author: {bookByIdResponse.Data.BookById.Author}, " +
    $"Publisher: {bookByIdResponse.Data.BookById.Publisher}, " +
    $"PublicationDate: {bookByIdResponse.Data.BookById.PublicationDate.ToShortDateString()}"
);

Console.ReadLine();

public class BooksResponse
{
    public List<Book>? Books { get; set; }
}

public class BookByIdResponse
{
    public Book? BookById { get; set; }
}

public class Book
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public DateTime PublicationDate { get; set; }
}
