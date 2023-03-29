using Grpc.Net.Client;
using GrpcClient.Operations;
using GrpcCRUDSample;


using var channel = GrpcChannel.ForAddress("http://localhost:5034");
var userOperation = new UserOperation(new User.UserClient(channel));
var greeterOperation = new GreeterOperation(new Greeter.GreeterClient(channel));


while (true)
{
    await userOperation.DisplayUsersAsync();

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
            await userOperation.ShowDetailAsync();
            break;
        case "2":
            await userOperation.InsertUserAsync();
            break;
        case "3":
            await userOperation.UpdateUserAsync();
            break;
        case "4":
            await userOperation.DeleteUserAsync();
            break;
        case "5":
            await greeterOperation.GreetAsync();
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
