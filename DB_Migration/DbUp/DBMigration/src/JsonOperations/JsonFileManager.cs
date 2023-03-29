using DBMigration.JsonOperations.ValueObjects;

namespace DBMigration.JsonOperations;

/// <summary>
/// Jsonファイルを管理するクラス
/// </summary>
internal class JsonFileManager
{
    /// <summary>
    /// 生成したjsonパス情報一覧
    /// </summary>
    internal IReadOnlyList<JsonFile> JsonFilePathList { get; }

    internal JsonFileManager(string inputPath)
    {
        // パスの指定がない場合、カレントディレクトリのappsettings.jsonをデフォルトとする。
        // パスの指定がある場合、相対パスを絶対パスに変換する。
        var path = string.IsNullOrEmpty(inputPath)
            ? Path.Combine(FolderPath.Default.Value, FileName.Default.Value)
            : Path.GetFullPath(inputPath);

        // 指定されたパスがディレクトリであれば、そのディレクトリのjsonファイルをすべて対象とする。
        // jsonファイルまで指定されていたら、そのファイルを対象とする。
        // 存在しないディレクトリやファイルを指定した場合、Fileクラスがスローする。
        JsonFilePathList = File.GetAttributes(path).HasFlag(FileAttributes.Directory)
            ? Directory.GetFiles(path, FileName.Unspecified.Value).Select(s => new JsonFile(s)).ToList()
            : new List<JsonFile>() { new JsonFile(path) };
    }
}
