namespace QLClient.GraphQL.Book;

public class BookMutations
{
    public int CreateBook { get; set; }
    public Models.Book? UpdateBook { get; set; }
    public bool DeleteBook { get; set; }
}