using DBMigration.JsonOperations.ValueObjects;

namespace DBMigration.JsonOperations;

/// <summary>
/// Jsonファイルを表現するクラス
/// </summary>
internal class JsonFile
{
    /// <summary>
    /// フォルダパス
    /// </summary>
    internal FolderPath FolderPath { get; }

    /// <summary>
    /// ファイルパス
    /// </summary>
    internal FileName FileName { get; }

    /// <summary>
    /// フルパス
    /// </summary>
    internal string FullPath { get; }

    internal JsonFile(string fullPath)
    {
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"ファイルが存在しません。:{fullPath}");

        FolderPath = new FolderPath(Path.GetDirectoryName(fullPath));
        FileName = new FileName(Path.GetFileName(fullPath));
        FullPath = fullPath;
    }
}
