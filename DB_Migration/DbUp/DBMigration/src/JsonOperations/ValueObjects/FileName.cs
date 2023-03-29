namespace DBMigration.JsonOperations.ValueObjects;

/// <summary>
/// ファイル名を表現した値オブジェクト
/// </summary>
internal class FileName
{
    /// <summary>
    /// ファイル名
    /// </summary>
    internal string Value { get; }

    /// <summary>
    /// 無指定状態を表現。
    /// 指定jsonがないので、全てのjsonファイルを対象とする。
    /// </summary>
    internal static readonly FileName Unspecified = new("*.json");

    /// <summary>
    /// ファイル名のデフォルト状態を表現。
    /// デフォルトはappsettings.jsonとする。
    /// </summary>
    internal static readonly FileName Default = new("appsettings.json");

    internal FileName(string? inputFilename = null)
    {
        // ファイル名の指定がなければ`appsettings.json`をデフォルトとする。
        var fileName = inputFilename ?? "appsettings.json";

        if (!fileName.EndsWith(".json"))
            throw new ArgumentException($"jsonファイルを指定してください。{Environment.NewLine}入力ファイル名 : {fileName}");

        Value = fileName;
    }
}
