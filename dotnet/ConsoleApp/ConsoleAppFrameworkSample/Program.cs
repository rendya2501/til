
internal class Program
{
    private static void Main(string[] args)
    {
        var app = ConsoleApp.Create(args);
        // 基本
        app.AddCommand("hello", ([Option("m", "Message to display.")] string message) => Console.WriteLine($"Hello {message}"));

        // app.AddCommands<Hello2>();
        app.AddCommands<Foo>();
        // app.AddCommands<Bar>();
        app.Run();
    }

    // [Command("hello2")]
    // public class Hello2 : ConsoleAppBase
    // {
    //     public void degllee([Option("m", "Message to display.")] int id)
    //     {
    //         // select * from...
    //     }
    // }
    
    public class Foo : ConsoleAppBase
    {

        [Command("Echo")]
        public void Echo([Option("m", "Message to display.")] string msg)
        {
            Console.WriteLine(msg);
        }

        [Command("Sum")]
        public void Sum([Option(0)] int x, [Option(1)] int y)
        {
            Console.WriteLine((x + y).ToString());
        }
    }

    [Command("Bar")]
    public class Bar : ConsoleAppBase
    {
        [Command("Bar2")]
        public void Hello2()
        {
            Console.WriteLine("H E L L O");
        }
    }
}