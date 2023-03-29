using Microsoft.Extensions.Configuration;

namespace DBMigration.JsonOperations;

/// <summary>
/// Configurationインスタンスを作成するファクトリクラスです。
/// </summary>
internal class ConfigurationBuilderFactory
{
    internal IConfiguration Configuration { get; }

    internal ConfigurationBuilderFactory(JsonFile path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        Configuration = new ConfigurationBuilder()
            .SetBasePath(path.FolderPath.Value)
            .AddJsonFile(path.FileName.Value)
            .Build();
    }
}
