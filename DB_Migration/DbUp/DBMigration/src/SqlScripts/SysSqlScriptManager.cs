using DBMigration.Consts;
using DBMigration.SqlScripts.Base;

namespace DBMigration.SqlScripts;

/// <summary>
/// Sys用 SqlScript管理クラス
/// </summary>
internal class SysSqlScriptManager : SqlScriptManager
{
    protected override string ScriptPrefix => DatabaseConstants.SysPrefix;
}
