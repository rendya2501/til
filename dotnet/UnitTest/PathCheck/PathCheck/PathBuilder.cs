using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PathCheckTest")]

namespace PathCheck;

internal class PathBuilder
{
    internal IReadOnlyList<string> Paths { get; init; } = new List<string>();

    internal PathBuilder(string inputPath)
    {
        var path = string.IsNullOrEmpty(inputPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), "*.json")
            : Path.GetFullPath(inputPath);

        this.Paths = File.GetAttributes(path).HasFlag(FileAttributes.Directory)
            ? Directory.GetFiles(path, "*.json")
            : new List<string>() { path };
    }
}
