using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DataLayer.Entities;

namespace DataLayer.Repositories.BookAggregation;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;
    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> AddAsync(Book entity)
    {
        var sql = @"
            INSERT INTO Book (Title,Author,Publisher,Genre,Price) 
            VALUES (@Title,@Author,@Publisher,@Genre,@Price)";
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM Book WHERE Id = @Id";
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        return await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        var sql = "SELECT * FROM Book";
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        var result = (await connection.QueryAsync<Book>(sql)).ToList();
        return result;
    }

    public async Task<Book> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Book WHERE Id = @Id";
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        var result = await connection.QuerySingleOrDefaultAsync<Book>(sql, new { id });
        return result;
    }

    public async Task<int> UpdateAsync(Book entity)
    {
        var sql = @"
            UPDATE Book 
            SET Title  = @Title , Author  = @Author , Publisher  = @Publisher , Genre  = @Genre , Price  = @Price
            WHERE Id = @Id";
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        var result = await connection.ExecuteAsync(sql, entity);
        return result;
    }
}