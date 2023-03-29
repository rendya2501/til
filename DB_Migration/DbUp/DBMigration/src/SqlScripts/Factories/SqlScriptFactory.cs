using DbUp.Engine;
using System.Reflection;

namespace DBMigration.SqlScripts.Factories;

/// <summary>
/// DbUpのSqlScriptクラスを生成する
/// </summary>
internal class SqlScriptFactory
{
    /// <summary>
    /// 埋め込みリソースから取得したファイル名のうち、不要となる文字列
    /// </summary>
    private static readonly string RemoveString = "DBMigration.Scripts.";

    /// <summary>
    /// スクリプト一覧
    /// </summary>
    internal static List<SqlScript> SqlScripts => new List<Assembly>() { Assembly.GetExecutingAssembly() }
        .Select(assembly => new
        {
            Assembly = assembly,
            ResourceNames = assembly.GetManifestResourceNames().ToArray()
        })
        .SelectMany(x =>
            x.ResourceNames.Select(resourceName =>
                SqlScript.FromStream(
                    // DBMigration.Scripts.Dat.yyyymm.~~ を Dat.yyyymm.~~ にする
                    resourceName.Replace(RemoveString, ""),
                    x.Assembly.GetManifestResourceStream(resourceName)
                )
            )
        )
        .OrderBy(sqlScript => sqlScript.Name)
        .ToList();

    /// <summary>
    /// ダミー用のSqlScriptインスタンスを取得します。
    /// </summary>
    /// <returns></returns>
    internal static SqlScript GetDummySqlScripts() => new("dummy.sql", "");
}
