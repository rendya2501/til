using Grpc.Net.Client;
using GrpcCRUDSample;


using var channel = GrpcChannel.ForAddress("http://localhost:5034");
var userClient = new User.UserClient(channel);
var greeterClient = new Greeter.GreeterClient(channel);


while (true)
{
    await DisplayUsers(userClient);

    Console.WriteLine("\nChoose an operation:");
    Console.WriteLine("1: Get");
    Console.WriteLine("2: Insert");
    Console.WriteLine("3: Update");
    Console.WriteLine("4: Delete");
    Console.WriteLine("5: Greet");
    Console.WriteLine("q: Exit");
    Console.Write("Enter the number or 'q': ");

    switch (Console.ReadLine() ?? "")
    {
        case "1":
            await ShowDetail(userClient);
            break;
        case "2":
            await InsertUser(userClient);
            break;
        case "3":
            await UpdateUser(userClient);
            break;
        case "4":
            await DeleteUser(userClient);
            break;
        case "5":
            await Greet(greeterClient);
            break;
        case "q":
            return;
        default:
            Console.WriteLine("Invalid operation. Please choose a number between 1 and 5, or 'q' to exit.");
            break;
    }

    Console.WriteLine();
    Console.WriteLine("---------------------------------------------------------------");
    Console.WriteLine();
}

static async Task DisplayUsers(User.UserClient client)
{
    Console.WriteLine("Current users in the database:");
    var response = await client.GetAllUsersAsync(new Empty { });

    foreach (var user in response.Users)
    {
        Console.WriteLine($"ID: {user.Id}, Name: {user.Name}");
    }
}

static async Task ShowDetail(User.UserClient client)
{
    Console.Write("Enter the user ID: ");
    _ = int.TryParse(Console.ReadLine(), out int id);

    var request = new GetUserRequest { Id = id };
    var response = await client.GetUserAsync(request);

    Console.WriteLine($"ID: {response.User.Id}, Name: {response.User.Name}, UniqueID: {response.User.UniqueId} ");
}

static async Task InsertUser(User.UserClient client)
{
    Console.Write("Enter the name: ");
    var name = Console.ReadLine();

    var request = new CreateUserRequest { Name = name };
    var response = await client.CreateUserAsync(request);

    Console.WriteLine($"User created with ID: {response.Id}");
}

static async Task UpdateUser(User.UserClient client)
{
    Console.Write("Enter the user ID: ");
    _ = int.TryParse(Console.ReadLine(), out int id);

    Console.Write("Enter the new name: ");
    var newName = Console.ReadLine();

    var request = new UpdateUserRequest { User = new UserModel { Id = id, Name = newName } };
    // var request = new UpdateUserRequest { User = { Id = id, Name = newName } }; // NG
    var response = await client.UpdateUserAsync(request);

    Console.WriteLine($"User with ID {id} {(response.Success ? "updated" : "not found")}.");
}

static async Task DeleteUser(User.UserClient client)
{
    Console.Write("Enter the user ID: ");
    _ = int.TryParse(Console.ReadLine(), out int id);

    var request = new DeleteUserRequest { Id = id };
    var response = await client.DeleteUserAsync(request);

    Console.WriteLine($"User with ID {id} {(response.Success ? "deleted" : "not found")}.");
}

static async Task Greet(Greeter.GreeterClient client)
{
    Console.Write("Enter the Greeting: ");
    var name = Console.ReadLine();

    var request = new HelloRequest { Name = name };
    var reply = await client.SayHelloAsync(request);
    Console.WriteLine("Greeting: " + reply.Message);
}

// while (true)
// {
//     Console.Write("Enter user ID: ");
//     int userId = int.Parse(Console.ReadLine());
//     if (userId == -1) break;

//     try
//     {
//         var response = await client.GetUserAsync(new GetUserRequest { Id = userId });
//         Console.WriteLine($"User ID: {response.Id}, Name: {response.Name}");
//     }
//     catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
//     {
//         Console.WriteLine("User not found.");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Error: {ex.Message}");
//     }
// }
