using GraphQL.Client.Abstractions;
using GraphQL;
using QLClient.GraphQL.Book;
using QLServer.Models;

namespace QLClient.GraphQL.Operations;

public class BookOperations
{
    private readonly IGraphQLClient _client;

    public BookOperations(IGraphQLClient client)
    {
        _client = client;
    }


    public async Task ShowBooksAsync()
    {
        var request = new GraphQLRequest
        {
            Query = @"
query {
  books {
    id
    title
    author
  }
}"
        };
        var response = await _client.SendQueryAsync<BookQueries>(request);
        var books = response.Data.Books;

        Console.WriteLine("ID\tTitle\t\tAuthor");
        Console.WriteLine("------------------------------------");
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Id}\t{book.Title}\t{book.Author}");
        }
    }


    public async Task GetBookByIdAsync()
    {
        Console.Write("Enter book ID: ");
        int id = int.Parse(Console.ReadLine());

        var request = new GraphQLRequest
        {
            Query = @"
query ($id: Int!) {
  book(id: $id) {
    id
    title
    author
    publisher
    publicationDate
  }
}",
            Variables = new { id }
        };
        var response = await _client.SendQueryAsync<BookQueries>(request);
        var book = response.Data.Book;

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }
        Console.WriteLine($"ID: {book.Id}");
        Console.WriteLine($"Title: {book.Title}");
        Console.WriteLine($"Author: {book.Author}");
        Console.WriteLine($"Publisher: {book.Publisher}");
        Console.WriteLine($"Publication Date: {book.PublicationDate}");
    }


    public async Task CreateBookAsync()
    {
        Console.Write("Title: ");
        string title = Console.ReadLine();
        Console.Write("Author: ");
        string author = Console.ReadLine();
        Console.Write("Publisher: ");
        string publisher = Console.ReadLine();
        Console.Write("Publication Date (yyyy-MM-dd): ");
        DateTime publicationDate = DateTime.Parse(Console.ReadLine());

        var request = new GraphQLRequest
        {
            Query = @"
mutation ($book: BookInput!) {
  createBook(book: $book) 
}",
            Variables = new
            {
                book = new BookInput()
                {
                    Title = title,
                    Author = author,
                    Publisher = publisher,
                    PublicationDate = publicationDate
                }
            }
        };
        var response = await _client.SendMutationAsync<BookMutations>(request);
        var succeedId = response.Data.CreateBook;
        Console.WriteLine($"Book created with ID: {succeedId}");
    }


    public async Task UpdateBookAsync()
    {
        Console.Write("Enter book ID: ");
        int updateId = int.Parse(Console.ReadLine());
        Console.Write("New title: ");
        string newTitle = Console.ReadLine();

        var request = new GraphQLRequest
        {
            Query = @"
mutation ($id: Int!, $title: String!) {
  updateBook(id: $id, title: $title) {
    id
    title
    author
    publisher
    publicationDate
  }
}",
            Variables = new { id = updateId, title = newTitle }
        };
        var response = await _client.SendMutationAsync<BookMutations>(request);
        var updateBook = response.Data.UpdateBook;

        if (updateBook == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }
        Console.WriteLine("Book updated:");
        Console.WriteLine($"ID: {updateBook.Id}");
        Console.WriteLine($"Title: {updateBook.Title}");
        Console.WriteLine($"Author: {updateBook.Author}");
        Console.WriteLine($"Publisher: {updateBook.Publisher}");
        Console.WriteLine($"Publication Date: {updateBook.PublicationDate}");
    }


    public async Task DeleteBookAsync()
    {
        Console.Write("Enter book ID: ");
        int deleteId = int.Parse(Console.ReadLine());

        var request = new GraphQLRequest
        {
            Query = @"
mutation ($id: Int!) {
  deleteBook(id: $id)
}",
            Variables = new { id = deleteId }
        };
        var response = await _client.SendMutationAsync<BookMutations>(request);
        var success = response.Data.DeleteBook;

        Console.WriteLine($"Book with ID {deleteId} {(success ? "deleted" : "not found")}.");
    }
}
