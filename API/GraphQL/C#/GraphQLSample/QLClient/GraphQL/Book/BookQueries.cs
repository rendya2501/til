namespace QLClient.GraphQL.Book;

public class BookQueries
{
    public IEnumerable<Models.Book>? Books { get; set; }
    public Models.Book? Book { get; set; }
}
