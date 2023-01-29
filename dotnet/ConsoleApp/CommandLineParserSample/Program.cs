// See https://aka.ms/new-console-template for more information
using CommandLine;

Parser.Default
    .ParseArguments<Options, Status>(args)
    .MapResult(
        (Options options) => options.Execute(),
        (Status s) => s.Execute(),
        errors => 1
    );
// .WithParsed<Options>(o =>
// {
//     if (o.Verbose)
//     {
//         Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
//         Console.WriteLine("Quick Start Example! App is in Verbose mode!");
//     }
//     else
//     {
//         Console.WriteLine($"Current Arguments: -v {o.Verbose}");
//         Console.WriteLine("Quick Start Example!");
//     }
// }).WithParsed<Status>(o =>
// {
//     if (o.Verbose)
//     {
//         Console.WriteLine($"Status: -v {o.Verbose}");
//         Console.WriteLine("Status");
//     }
//     else
//     {
//         Console.WriteLine($"Current Arguments: -v {o.Verbose}");
//         Console.WriteLine("Quick Start Example!");
//     }
// });

[Verb("Sub", HelpText = "Add file contents to the index.")]
public class Options
{
    [Option('c', "connection", Required = true, HelpText = "接続文字列")]
    public string connection { get; set; } = default!;

    [Option('h', "hoge")]
    public string Hoge { get; set; } = default!;

    public int Execute()
    {
        Console.WriteLine($"接続文字列: {this.connection}");
        Console.WriteLine($"hoge: {this.Hoge}");
        return 1;
    }
}

[Verb("Status")]
public class Status
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    public int Execute()
    {
        Console.WriteLine($"Status Status Status. Run Run: -v {this.Verbose}");
        return 1;
    }
}

