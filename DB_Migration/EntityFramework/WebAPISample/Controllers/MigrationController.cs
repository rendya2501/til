using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPISample.Context;

namespace WebAPISample.Controllers;

[ApiController]
[Route("[controller]")]
public class MigrationController : Controller
{
    private readonly DatContext _datContext;

    public MigrationController(DatContext datContext)
    {
        _datContext = datContext;
    }

    /// <summary>
    /// 移行を実行します。
    /// </summary>
    /// <param name="migrationFileName"></param>
    /// <returns></returns>
    [HttpPost("ExecuteMigration")]
    public async Task<IActionResult> ExecuteMigration(string? migrationFileName)
    {
        try
        {
            await _datContext.Database.BeginTransactionAsync();
            await _datContext.Database.GetService<IMigrator>().MigrateAsync(migrationFileName);
            await _datContext.Database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _datContext.Database.RollbackTransactionAsync();
            throw;
        }

        return Ok();
    }

    /// <summary>
    /// 対象データベースに適応されているマイグレーションを表示します。
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAppliedMigrations")]
    public async Task<ActionResult<IEnumerable<string?>>> GetAppliedMigrations()
    {
        var appliedMigratiosn = await _datContext.Database.GetAppliedMigrationsAsync();
        return Ok(appliedMigratiosn);
    }

    /// <summary>
    /// 対象データベースに適応すべきマイグレーションファイルの一覧を表示します。
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetPendingMigration")]
    public async Task<ActionResult<IEnumerable<string?>>> GetPendingMigration()
    {
        var pendingMigrations = await _datContext.Database.GetPendingMigrationsAsync();
        return Ok(pendingMigrations);
    }

    /// <summary>
    /// 保持しているマイグレーションファイルの一覧を表示します。
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMigrationFileNames")]
    public ActionResult<IEnumerable<string?>> GetMigrationFileNames()
    {
        var appliedMigratiosn = Directory.EnumerateFiles(@$"{Directory.GetCurrentDirectory()}\Migrations", "*.cs")
            .Select(s => Path.GetFileNameWithoutExtension(s))
            .Where(w => !w.EndsWith("Designer") && !w.EndsWith("Snapshot"))
            .ToList();
        return Ok(appliedMigratiosn);
    }

    /// <summary>
    /// 指定されたマイグレーションファイルとの差分からSQLスクリプトを生成します。
    /// </summary>
    /// <param name="fromMigration"></param>
    /// <param name="toMigration"></param>
    /// <returns></returns>
    [HttpGet("GetScript")]
    public ActionResult<string?> GetScript(string? fromMigration, string? toMigration)
    {
        var script = _datContext.Database.GetService<IMigrator>().GenerateScript(fromMigration, toMigration);
        return Ok(script);
    }
}