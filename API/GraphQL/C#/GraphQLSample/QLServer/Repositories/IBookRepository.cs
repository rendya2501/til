namespace QLServer.Repositories
{
    public interface IBookRepository
    {
        Task<Models.Book> GetBookByIdAsync(int id);
        Task<IEnumerable<Models.Book>> GetAllBooksAsync();
        Task<int> CreateBookAsync(Models.Book book);
        Task<bool> UpdateBookAsync(Models.Book book);
        Task<bool> DeleteBookAsync(int id);
    }
}
