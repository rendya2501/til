using GrpcCRUDSample;
using static GrpcCRUDSample.User;

namespace GrpcClient.Operations;

public class UserOperation
{
    private readonly UserClient _client;

    public UserOperation(UserClient client)
    {
        _client = client;
    }


    public async Task DisplayUsersAsync()
    {
        Console.WriteLine("Current users in the database:");

        var response = await _client.GetAllUsersAsync(new Empty { });

        foreach (var user in response.Users)
        {
            Console.WriteLine($"ID: {user.Id}, Name: {user.Name}");
        }
    }

    public async Task ShowDetailAsync()
    {
        Console.Write("Enter the user ID: ");
        _ = int.TryParse(Console.ReadLine(), out int id);

        var request = new GetUserRequest { Id = id };
        var response = await _client.GetUserAsync(request);

        Console.WriteLine($"ID: {response.User.Id}, Name: {response.User.Name}, UniqueID: {response.User.UniqueId} ");
    }

    public async Task InsertUserAsync()
    {
        Console.Write("Enter the name: ");
        var name = Console.ReadLine();

        var request = new CreateUserRequest { Name = name };
        var response = await _client.CreateUserAsync(request);

        Console.WriteLine($"User created with ID: {response.Id}");
    }

    public async Task UpdateUserAsync()
    {
        Console.Write("Enter the user ID: ");
        _ = int.TryParse(Console.ReadLine(), out int id);

        Console.Write("Enter the new name: ");
        var newName = Console.ReadLine();

        var request = new UpdateUserRequest { User = new UserModel { Id = id, Name = newName } };
        // var request = new UpdateUserRequest { User = { Id = id, Name = newName } }; // NG
        var response = await _client.UpdateUserAsync(request);

        Console.WriteLine($"User with ID {id} {(response.Success ? "updated" : "not found")}.");
    }

    public async Task DeleteUserAsync()
    {
        Console.Write("Enter the user ID: ");
        _ = int.TryParse(Console.ReadLine(), out int id);

        var request = new DeleteUserRequest { Id = id };
        var response = await _client.DeleteUserAsync(request);

        Console.WriteLine($"User with ID {id} {(response.Success ? "deleted" : "not found")}.");
    }

}
