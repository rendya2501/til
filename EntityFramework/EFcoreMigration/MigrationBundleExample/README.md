# MigrationsBundleConsoleAppExample

ConsoleアプリにおけるEFCoreMigrationBundle生成検証プロジェクト  

- Contextクラスに接続情報をDIする方法の検証  
  →公式サンプルの通りにやればよろしい。  
   DIのやり方にも形式があるようで、それから外れた場合はエラーとなり、bundleを生成できない。  
- 接続情報を埋め込んだ状態でbundleを生成した場合、appsetting.jsonのrequire警告が出るか検証  
  →出なかった。  
- 埋め込んだ接続情報はbundle後も有効か検証  
  →有効であった。  

1,2,3,4の移行があり、3で意図的にエラーとさせた場合、4は実行されるのか？  
→されない。ただし1,2は実行されるので、1つでも失敗した場合、全てロールバックする設定があるのか調査する必要がある。  
因みに3での意図的なエラーは`CREATE VIEW`を`CREATE VUEW`としてエラーにしている。  

以下、1,2,3,4の実行ログ  

``` txt
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 6.0.11 initialized 'DatContext' using provider 'Microsoft.EntityFrameworkCore.SqlServer:6.0.11' with options: None
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (13ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT 1
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (11ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT OBJECT_ID(N'[__EFMigrationsHistory]');
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT 1
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (3ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE [__EFMigrationsHistory] (
          [MigrationId] nvarchar(150) NOT NULL,
          [ProductVersion] nvarchar(32) NOT NULL,
          CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
      );
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT 1
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT OBJECT_ID(N'[__EFMigrationsHistory]');
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [MigrationId], [ProductVersion]
      FROM [__EFMigrationsHistory]
      ORDER BY [MigrationId];
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20221108061420_First'.
Applying migration '20221108061420_First'.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE [Person] (
          [Id] int NOT NULL IDENTITY,
          [Name] nvarchar(max) NOT NULL,
          CONSTRAINT [PK_Person] PRIMARY KEY ([Id])
      );
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
      VALUES (N'20221108061420_First', N'6.0.11');
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20221108061607_Second'.
Applying migration '20221108061607_Second'.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (17ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      DECLARE @var0 sysname;
      SELECT @var0 = [d].[name]
      FROM [sys].[default_constraints] [d]
      INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]   
      WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Person]') AND [c].[name] = N'Name');
      IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Person] DROP CONSTRAINT [' + @var0 + '];');
      ALTER TABLE [Person] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
      VALUES (N'20221108061607_Second', N'6.0.11');
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20221108071310_third'.
Applying migration '20221108071310_third'.
fail: Microsoft.EntityFrameworkCore.Database.Command[20102]
      Failed executing DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']

                      CREATE VUEW [dbo].[MyView]
                      AS
                      SELECT ID,Name FROM dbo.People
Failed executing DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']

                CREATE VUEW [dbo].[MyView]
   at Microsoft.EntityFrameworkCore.Migrations.Internal.Migrator.Migrate(String targetMigration)
   at Microsoft.EntityFrameworkCore.Design.Internal.MigrationsOperations.UpdateDatabase(String targetMigration, String connectionString, String contextType)   at Microsoft.EntityFrameworkCore.Migrations.Design.MigrationsBundle.ExecuteInternal(String[] args)
   at Microsoft.EntityFrameworkCore.Migrations.Design.MigrationsBundle.<>c__DisplayClass6_0.<Configure>b__0(String[] args)         at Microsoft.DotNet.Cli.CommandLine.CommandLineApplication.Execute(String[] args)
   at Microsoft.EntityFrameworkCore.Migrations.Design.MigrationsBundle.Execute(String context, Assembly assembly, Assembly startupAssembly, String[] args)ClientConnectionId:66310037-4d19-410d-9257-ff3c39a89628Error Number:343,State:1,Class:15
不明なオブジェクトの種類 'VUEW' が CREATE、DROP、または ALTER ステートメントで使用されています。
```

- 参考  
  - [Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
