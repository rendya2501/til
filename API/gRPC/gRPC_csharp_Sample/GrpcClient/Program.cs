using Grpc.Net.Client;
using GrpcSample;


// 開発環境用に証明書検証をスキップする設定
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


// 通信先のサーバーアドレスを指定
using var channel = GrpcChannel.ForAddress("http://localhost:5041");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "World" });

Console.WriteLine("Greeting: " + reply.Message);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();