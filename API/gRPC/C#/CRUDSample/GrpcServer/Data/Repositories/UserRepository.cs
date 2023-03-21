using Dapper;
using GrpcServer.Models;
using System.Data;

namespace GrpcServer.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _connection.QueryAsync<User>("SELECT * FROM Users");
    }

    public async Task<int> CreateUserAsync(User user)
    {
        var sql = "INSERT INTO Users (Name) VALUES (@Name); SELECT last_insert_rowid();";
        return await _connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var sql = "UPDATE Users SET Name = @Name WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(sql, user);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var sql = "DELETE FROM Users WHERE Id = @Id";
        var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
}