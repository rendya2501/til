# リバースエンジニアリング(スキャホールディング)

スキャフォールドに関するあれこれをまとめる

---

## リバースエンジニアリングコマンド

TestServerのTestDatabaseの情報を逆移行(リバースエンジニアリング)するコマンド  

■ PMC

``` txt
Scaffold-DbContext 'Server=TestServer;Database=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext -DataAnnotations -UseDatabaseNames -Force
```

■ dotnet-ef

``` txt
dotnet ef dbcontext scaffold 'Server=TestServer;Database=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -o Entity --context-dir Context --context DatContext --data-annotations --use-database-names --no-onconfiguring --force
```

■ コマンドの意味

``` txt
サーバー             : TestServer
データベース         : TestDatabase
ユーザー             : sa
パスワード           : 123456789
モデルの出力先       : Model
コンテキストの出力先 : Context
コンテキスト名       : DatContext
プロパティにアノテーションをつける : -DataAnnotations  --data-annotations
データベースのテーブル名に準拠する : -UseDatabaseNames --use-database-names
リバース結果を上書きする           : -Force  --force
```

`Integrated Security`がTrueだとログインできないという警告が出る。  
Falseにすることで解決するがそれでよいかどうかは知らない。  
SecurityをFalseにするのであまりよい印象はない。  

`–UseDatabaseNames`オプションがないとテーブル名が完全一致しない。
`TMa_Master`テーブルのテーブル名が`TmaMaster`みたいになってしまう。  

■ コマンド実行後のフォルダ構成  

``` txt
Project
├─Context
├─Model
└─Program.cs
```

[ASP.NET Core - Scaffolding with Entity Framework Core (Database first approach)](https://www.youtube.com/watch?v=SnU4Ulee_NI)  

---

## 論理名はxmlコメントとしてスキャフォールドされる

■**A5M2定義**  

``` txt
テーブル論理名 : テストテーブル
テーブル物理名 : TestTable

No | 論理名 | 物理名 | データ型 or ドメイン | 必須 | 主キー |
---+--------+--------+----------------------+------+--------|
1  | 連番   | SeqNo  | bigint identity      | 〆   | 1      |
2  | コード | Code   | nvarchar(3)          | 〆   | 2      |
3  | 名称   | Name   | nvarchar(50)         |      |        |
4  | 区分   | Type   | tinyint              |      |        |
```

■**A5M2_出力されるDDL**  

論理名はコメントとして登録される。  
[SQLServerのテーブルやカラムにコメントをつける方法](https://lightgauge.net/database/sqlserver/3010/)  

``` sql
create table [TestTable] (
  [SeqNo] BIGINT IDENTITY not null
  , [Code] NVARCHAR(3) not null
  , [Name] NVARCHAR(50)
  , [Type] TINYINT
  , constraint [TestTable_PKC] primary key ([SeqNo],[Code])
) ;

EXECUTE sp_addextendedproperty N'MS_Description', N'テストテーブル', N'SCHEMA', N'dbo', N'TABLE', N'TestTable', NULL, NULL;
EXECUTE sp_addextendedproperty N'MS_Description', N'連番', N'SCHEMA', N'dbo', N'TABLE', N'TestTable', N'COLUMN', N'SeqNo';
EXECUTE sp_addextendedproperty N'MS_Description', N'コード', N'SCHEMA', N'dbo', N'TABLE', N'TestTable', N'COLUMN', N'Code';
EXECUTE sp_addextendedproperty N'MS_Description', N'名称', N'SCHEMA', N'dbo', N'TABLE', N'TestTable', N'COLUMN', N'Name';
EXECUTE sp_addextendedproperty N'MS_Description', N'区分', N'SCHEMA', N'dbo', N'TABLE', N'TestTable', N'COLUMN', N'Type';
```

■**EFCore7でスキャフォールドした結果**  

論理名はドキュメントコメントとして反映されていることが分かる。  

``` cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// テストテーブル
/// </summary>
[PrimaryKey("SeqNo", "Code")]
[Table("TestTable")]
public partial class TestTable
{
    /// <summary>
    /// 連番
    /// </summary>
    [Key]
    public long SeqNo { get; set; }

    /// <summary>
    /// コード
    /// </summary>
    [Key]
    [StringLength(3)]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(50)]
    public string? Name { get; set; }

    /// <summary>
    /// 区分
    /// </summary>
    public byte? Type { get; set; }
}
```

■**番外編1**  

因みにスキャフォールドした直後のDbContextクラスは以下の通り。  

``` cs
public partial class DatContext : DbContext
{
    public DatContext(DbContextOptions<DatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TestTable> TestTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestTable>(entity =>
        {
            entity.HasKey(e => new { e.SeqNo, e.Code }).HasName("TestTable_PKC");

            entity.ToTable("TestTable", tb => tb.HasComment("テストテーブル"));

            entity.Property(e => e.SeqNo)
                .ValueGeneratedOnAdd()
                .HasComment("連番");
            entity.Property(e => e.Code).HasComment("コード");
            entity.Property(e => e.Name).HasComment("名称");
            entity.Property(e => e.Type).HasComment("区分");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

因みにこの状態から`dotnet ef migrations add first`して、`dotnet ef migrations script`で出力したクエリは以下の通り。  
A5M2と結果は同じになる。  

``` sql
CREATE TABLE [TestTable] (
    [SeqNo] bigint NOT NULL IDENTITY,
    [Code] nvarchar(3) NOT NULL,
    [Name] nvarchar(50) NULL,
    [Type] tinyint NULL,
    CONSTRAINT [TestTable_PKC] PRIMARY KEY ([SeqNo], [Code])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'テストテーブル';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable';
SET @description = N'連番';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'SeqNo';
SET @description = N'コード';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Code';
SET @description = N'名称';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Name';
SET @description = N'区分';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Type';
```

■**番外編2**  

DbContextクラスのOnModelCreatingを削除する。  

``` cs
public partial class DatContext : DbContext
{
    public DatContext(DbContextOptions<DatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TestTable> TestTables { get; set; }
}
```

エンティティを以下のように書き換える。  

- Commentアノテーションを追加  
- 連番カラムに`[DatabaseGenerated( DatabaseGeneratedOption.Identity)]`を追加  

``` cs
/// <summary>
/// テストテーブル
/// </summary>
[PrimaryKey("SeqNo", "Code")]
[Table("TestTable")]
[Comment("テストテーブル")]
public partial class TestTable
{
    /// <summary>
    /// 連番
    /// </summary>
    [Key]
    [Comment("連番")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long SeqNo { get; set; }

    /// <summary>
    /// コード
    /// </summary>
    [Key]
    [StringLength(3)]
    [Comment("コード")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(50)]
    [Comment("名称")]
    public string? Name { get; set; }

    /// <summary>
    /// 区分
    /// </summary>
    [Comment("区分")]
    public byte? Type { get; set; }
}
```

`dotnet ef migrations add first`でマイグレーションファイルを作成。  
`dotnet ef database script`コマンドでクエリを出力する。  

``` sql
CREATE TABLE [TestTable] (
    [SeqNo] bigint NOT NULL IDENTITY,
    [Code] nvarchar(3) NOT NULL,
    [Name] nvarchar(50) NULL,
    [Type] tinyint NULL,
    CONSTRAINT [PK_TestTable] PRIMARY KEY ([SeqNo], [Code])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'テストテーブル';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable';
SET @description = N'連番';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'SeqNo';
SET @description = N'コード';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Code';
SET @description = N'名称';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Name';
SET @description = N'区分';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TestTable', 'COLUMN', N'Type';
```

OnModelCreatingは必要なく、アノテーションだけで同等のクエリを生成可能であることが分かる。  
唯一、主キー制約名が違うのでUpメソッドで直接書き換える必要はある。  

A5M2 : `TastTable_PKC`  
EFcore : `PK_TestTable`  

---

## スキャフォールドした時にOnModelCreatingが膨大になった場合の対処法

`IEntityTypeConfiguration<T>`を使用する。  
OnModelCreatingにべた書きするのではなく、クラス事にOnModelCreatingして、ContextクラスのOnModelCreatingはそれを呼び出すように修正する。  
T4によるカスタムテンプレートを作った上でのスキャフォールドで対処する内容となる。  

>大規模なモデルの場合、DbContext クラスの OnModelCreating メソッドが手に負えないほど大きくなる可能性があります。  
>これに対処する 1 つの方法は、IEntityTypeConfiguration\<T> クラスを使用することです。  
>これらのクラスの詳細については、「モデルの作成と構成」を参照してください。  
>[カスタム リバース エンジニアリング テンプレート - EF Core | Microsoft Learn](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/scaffolding/templates?tabs=dotnet-core-cli#entity-configuration-classes)  

<!--  -->
>OnModelCreating メソッドのサイズを小さくするため、エンティティ型のすべての構成を、IEntityTypeConfiguration\<TEntity> を実装する個別のクラスに抽出できます。  
>[モデルの作成と構成 - EF Core | Microsoft Learn](https://learn.microsoft.com/ja-jp/ef/core/modeling/#grouping-configuration)  

---

## Identityアノテーションがスキャフォールドされない

どうやらそういうものらしい。  

>DatabaseGenerated(DatabaseGeneratedOption.Identity)] データアノテーションを使用して、エンティティの追加時にデータベースが値を自分で生成することを知らせることは理論的には可能である。  
>
>しかし、[DatabaseGenerated]属性にはデフォルト値を指定するプロパティがないため、この場合、どのような値になるかの情報は保持されない。  
>データベースを雛形化した後にmigrationsを使いたい場合もあるので、デフォルト値のメタデータはかなり有用かもしれません。  
>
>ちなみに、BirthDateとGenderのプロパティにはデフォルト値の呼び出しが生成されません。  
>なぜなら、それらのデータベースで定義されたDEFAULT制約は、これらの型のCLRデフォルト値（DateTime?はnull、boolはfalse）と同じだからです。  
>
>[asp.net mvc - Scaffold-DbContext doese not add [DatabaseGenerated(DatabaseGeneratedOption.Computed)] on Generated class - Stack Overflow](https://stackoverflow.com/questions/63040061/scaffold-dbcontext-doese-not-add-databasegenerateddatabasegeneratedoption-comp)  
