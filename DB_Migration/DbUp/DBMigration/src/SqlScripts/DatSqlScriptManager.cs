using DBMigration.Consts;
using DBMigration.SqlScripts.Base;

namespace DBMigration.SqlScripts;

/// <summary>
/// Dat用 SqlScript管理クラス
/// </summary>
internal class DatSqlScriptManager : SqlScriptManager
{
    protected override string ScriptPrefix => DatabaseConstants.DatPrefix;
}
