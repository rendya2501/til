using DBMigration.SqlScripts.Factories;
using DbUp.Engine;

namespace DBMigration.SqlScripts.Base;

/// <summary>
/// SqlScriptクラスを管理する基底クラス
/// </summary>
internal abstract class SqlScriptManager
{
    /// <summary>
    /// FirstMigrationスクリプト名の定義
    /// </summary>
    static readonly string FirstMigration = "FirstMigration.sql";

    /// <summary>
    /// Scriptsフォルダの接頭文字
    /// </summary>
    protected abstract string ScriptPrefix { get; }

    /// <summary>
    /// FirstMigrationスクリプトを取得します。
    /// </summary>
    /// <returns></returns>
    public SqlScript GetFirstMigrationScript()
    {
        return SqlScriptFactory.SqlScripts
            .First(w => w.Name.StartsWith(ScriptPrefix) && w.Name.EndsWith(FirstMigration));
    }

    /// <summary>
    /// スクリプトを取得します。
    /// </summary>
    /// <returns></returns>
    public List<SqlScript> GetSqlScripts()
    {
        return SqlScriptFactory.SqlScripts
            .Where(w => w.Name.StartsWith(ScriptPrefix))
            .ToList();
    }

    /// <summary>
    /// FirstMigrationスクリプトの内容を空にしたスクリプトを取得します。
    /// </summary>
    /// <returns></returns>
    public List<SqlScript> GetSqlScriptsIgnoreChange()
    {
        // FirstMigrationを飛ばして取得
        var sqlScripts = GetSqlScripts().Skip(1).ToList();

        var firstMigration = GetFirstMigrationScript();

        sqlScripts.Insert(0, new SqlScript(firstMigration.Name, ""));

        return sqlScripts;
    }
}
