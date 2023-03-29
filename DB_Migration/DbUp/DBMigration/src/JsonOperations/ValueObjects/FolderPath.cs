namespace DBMigration.JsonOperations.ValueObjects;

/// <summary>
/// フォルダへのパスを表現した値オブジェクト
/// </summary>
internal class FolderPath
{
    /// <summary>
    /// フォルダパス
    /// </summary>
    internal string Value { get; }

    /// <summary>
    /// フォルダのデフォルト状態を表現。
    /// デフォルトはカレントディレクトリとする。
    /// </summary>
    internal static readonly FolderPath Default = new(Directory.GetCurrentDirectory());

    internal FolderPath(string? inputFolderPath = null)
    {
        // フォルダパスの指定がなければカレントディレクトリをデフォルトとする。
        var folderPath = inputFolderPath ?? Directory.GetCurrentDirectory();

        if (!Directory.Exists(folderPath))
            throw new ArgumentException($"{folderPath}は存在しません。");

        Value = folderPath;
    }
}
