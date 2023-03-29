using GrpcCRUDSample;
using static GrpcCRUDSample.Greeter;

namespace GrpcClient.Operations;

internal class GreeterOperation
{
    private readonly GreeterClient _client;

    public GreeterOperation(GreeterClient client)
    {
        _client = client;
    }

    public async Task GreetAsync()
    {
        Console.Write("Enter the Greeting: ");
        var name = Console.ReadLine();

        var request = new HelloRequest { Name = name };
        var reply = await _client.SayHelloAsync(request);
        Console.WriteLine("Greeting: " + reply.Message);
    }
}
