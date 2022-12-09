# EntityFrameworkCore

MigrationにおけるEntityFrameworkCoreの仕様をまとめる  

---

## 列を途中に追加する場合、順序は維持されない

単純にAddColumnされるだけである。  
なので、エンティティのプロパティの順序とデータベースのカラムの順序がずれることになる。  

順番を維持したければ、自前でクエリを書く必要がある。  
(tempテーブルを用意しつつ、新しいテーブルを作って、INSERT SELECTしてTEMPをDROPするやつ)  

無印のEntityFrameworkでは出来た模様。  
EFCoreはテーブル生成の最初だけ有効だが、変更の段階では無効となる模様。  

[Entity Framework コードファーストで複合キーを使ってみる](https://blog.shibayan.jp/entry/20110217/1297872610)  

---

## 列の変更

列の変更は、Alterではない。
→
drop columnした後、add columnする。
Alter Columnではない。
`EXEC sp_rename 'テーブル名.現在の列名','変更する列名','COLUMN'`  

列定義の変更も同様
alter tableされず、
`ALTER TABLE テーブル名 ALTER COLUMN 変更する列名 データ型`

[EntityFramework Core のエンティティを名前変更したら、テーブル削除/新しい名前でテーブル新規作成のマイグレーションコードが生成されてしまった](https://devadjust.exblog.jp/28190433/)  

---

## 履歴テーブルのカスタマイズ

このサイトの通りにやれば行けるかもしれない。  

[Customize Entity Framework Core Migration History Table](https://www.codeproject.com/Articles/5338891/Customize-Entity-Framework-Core-Migration-History)  
[How to Customize Migration History Table with Entity Framework Core](https://stackoverflow.com/questions/55342435/how-to-customize-migration-history-table-with-entity-framework-core)  

検索文字列 : efcore history customize  

---

## 論理名はxmlコメントとしてスキャフォールドされる

---

## WebプロジェクトがMigrationプロジェクトと親和性が高い理由

コンソールアプリからbundleを作成しようとした時、うまくいかなかった。  
スタートアップの書き方はWebをHostするような形でないと駄目らしい。  

参考リンク先の文献をそのまま引用する。  

>どうやら、EF Core CLI は ASP.NET Core アプリケーションの Program クラスに定義されている (であろう) CreateWebHostBuilder メソッドを必要とする模様。  
[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

- コンソールアプリではHostをBuildする必要がある。  
  - 最初からHostingが前提のWebアプリのほうが手っ取り早い。  
- コンソールアプリでappsettings.jsonを使う場合、パッケージをインストールする必要がある。  
  - Webアプリでは最初からjsonがサポートされているので手っ取り早い。  
- コンソールアプリではユーザーシークレットの機能が使えない。(たぶん頑張ればいけるけど、調べた感じパッと出てこなかった)  
  - Webアプリは問題なく使える。  

というわけで、webプロジェクトとして構築したほうがいろいろ都合がよい。  

---

## 初期データの投入

マイグレーションファイルに直接記述することで投入可能。  

>Entity Framework Coreのシードデータ  
>ほとんどのプロジェクトでは、作成されたデータベースにいくつかの初期データを持ちたいと思います。  
>そこで、データベースを作成し構成するために移行ファイルを実行するとすぐに、いくつかの初期データを投入したいと思います。  
>このアクションはデータシーディングと呼ばれます。  
>[Migrations and Seed Data With Entity Framework Core](https://code-maze.com/migrations-and-seed-data-efcore/)  

---

## ストアド等のマイグレーションは可能か？

EntityFramework6ではストアドは行けるらしいが、Coreでは対応していない。  
※EFCore7である程度対応された模様。  

・ストアド、シノニムのリバースエンジニアリングはできないことを確認。  
・テーブル以外の情報(view,index,ストアド,シノニム)はマイグレーションファイルのUpメソッドに生クエリで直接記述する。  
・同時にDownメソッドに`DROP VIEW view_hoge"`等の命令を記述すること  

・マイグレーションの時に生クエリをMigrationクラスの中に記述すれば行ける模様。  
・view,indexの作成はコードファーストでは無理。マイグレーションファイルに直接記述する必要がある模様。  

>viewが作れない  
>indexが貼れない  
[[C#]EntityFramework(dotnet ef)マイグレーションまとめ](https://codelikes.com/csharp-dotnet-ef-migrations/)  
[[C#]EntityFrameworkでViewをMigration時に作る](https://codelikes.com/entityframework-view-migration/)  

<!--  -->
>戴いた記事のリンク読みましたが、code firstでのきめ細かい実装は無理と思わざるを得ませんでした。なかなかこれといった解決策はなさそうです。  
[Entity Frameworkは、ストアドプロシージャを自動生成できますか？](https://teratail.com/questions/250125)  
[EF6_Code First での挿入、更新、削除にストアド プロシージャを使用する](https://learn.microsoft.com/ja-jp/ef/ef6/modeling/code-first/fluent/cud-stored-procedures?redirectedfrom=MSDN)  

---

## Migrationプロジェクトをクラスライブラリとして使う方法

まだ検証段階だが、この通りにやれば行けるかも。  

``` txt
sln
├─Sample.Web  Webアプリケーション。Sample.Dbを参照
├─Sample.API  Web APIプロジェクト。Sample.Dbを参照
└─Sample.Db   DBのEntityクラスを定義したクラスライブラリ
```

`dotnet ef migrations add NewMigration --startup-project ..//Sample.API`  

[Entity Framework CoreのMigrationをクラスライブラリに対して実施したい](https://qiita.com/gushwell/items/4c1e54ab3281670db0b9)  
[別の移行プロジェクトを使用する](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli)  

---

## 複合主キーの設定

※2022/12/06 Tue  
EF7では複合主キーの問題に対応されていることが分かった。  
EF6以前まではアノテーションだけの解決は無理。  

複合主キーはEntityの`Key`アノテーションの指定だけでは無理。  
DbContextクラスのOnModelCreatingメソッドの中でHasKeyメソッドによる複合主キーの設定が必要。  
そうしなければ、マイグレーションファイル作成時にエラーとなり、移行処理を先に進めることができなくなる。  

■`Category`エンティティに`Key`アノテーションで複合主キーを設定する。  

``` cs : Category.cs
public class Category
{
    [Key]
    public int Id { get; set; }
    [Key]
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}
```

■マイグレーションファイル追加コマンド実行  

`dotnet ef migrations add <name>`  

すると、以下のエラーが発生する。  

``` txt
The entity type 'Category' has multiple properties with the [Key] attribute. Composite primary keys can only be set using 'HasKey' in 'OnModelCreating'.
```

DbContextクラスを継承したContextクラスでOnModelCreatingメソッドをoverrideし、Linq中でHasKeyで複合主キーを設定しなければならない。  

``` cs : DbContext
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Name })
                .HasName("Category_PKC");

            // 省略
        }
    }
```

■EF7  

EF7の新機能として、`PrimaryKey`アノテーションが追加された。  
OnModelCreatingメソッドのHasKeyメソッドによる複合主キーの設定は必要なくなった。  

しかし、注意点として、主キー名は設定できないので、Upメソッドで書き直す必要がある。  

``` cs
[PrimaryKey(nameof(PostId), nameof(CommentId))]
public class Comment
{
    public int PostId { get; set; }
    public int CommentId { get; set; }
    public string CommentText { get; set; } = null!;
}
```

[Microsoft_複合キーのマッピング属性](https://learn.microsoft.com/ja-jp/ef/core/what-is-new/ef-core-7.0/whatsnew#mapping-attribute-for-composite-keys)  

---

## スナップショット

現在の状態を定義しているファイル。  

`dotnet ef migration add`コマンドでマイグレーションファイルを生成する時に、エンティティの状態とスナップショットとの差分を計算し、必要なUp,Downを生成する。  
そのために必要な現在の状態を定義しているファイルがスナップショット。  

### 解説サイト全文

>モデルスナップショットは、\<YourContext>ModelSnapshot.cs という名前のクラスファイルに格納されたモデルの現在の状態です。  
>このファイルは、最初の移行が作成されたときに Migrations フォルダに追加され、その後の移行ごとに更新されます。  
>このファイルによって、Migrations フレームワークがデータベースをモデルに合わせて最新の状態にするために必要な変更を計算することができます。  
>
>以前のバージョンでは、この情報はデータベースに保存されていたため、新しい移行のためのコードをスキャフォールドする前に、データベースに問い合わせる必要がありました。  
>また、個々のマイグレーションごとにリソースファイル(.resx)として保存されていました。  
>このため、各開発者が独自の開発データベースを持ち、しばしば開発の異なる段階にあったり、共有の開発データベースに異なる移行を同時に適用しようとするようなチーム環境では、問題が生じていました。  
>
>スナップショットが各移行の一部として保存されている場合、移行を削除することが可能で、その際、スナップショットは削除され、移行とスナップショットは依然として互いに同期したままでした。  
>EF Coreでマイグレーションファイルを削除すると、スナップショットとマイグレーションがずれたままになってしまいます。  
>このため、マイグレーションを元に戻すにはremove-migrationコマンドだけを使うようにすることをお勧めします。  
>とはいえ、うっかりmigrationsフォルダからmigrationファイルを削除してしまっても、EF Coreはそれを認識してスナップショットを元に戻してくれます。  
>
>[The Model Snapshot In Entity Framework Core](https://www.learnentityframeworkcore.com/migrations/model-snapshot)  

### スナップショット検証

■**First**  

初期状態としてProductエンティティを定義する。  

``` cs
public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
}
```

Firstマイグレーションファイルを作成する。  
`dotnet ef migrations First`  

生成されたSnapshot情報。  
Productエンティティの情報と一致している。  

``` cs
[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "6.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

        modelBuilder.Entity("BundleCreateSequence.Models.Product", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                b.Property<int>("CategoryId")
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Product");
            });
#pragma warning restore 612, 618
    }
}
```

■**Second**  

Productエンティティに「Description」追加。  

``` cs
public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
}
```

Secondマイグレーションファイルを作成する。  
`dotnet ef migrations second`  

SnapShotにDescriptionが追加された。

``` cs
[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "6.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

        modelBuilder.Entity("BundleCreateSequence.Models.Product", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                b.Property<int>("CategoryId")
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Description") // ←追加
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Product");
            });
#pragma warning restore 612, 618
    }
}
```

■**Third**  

Secondマイグレーションファイルを直接削除。  
ProductエンティティにTestを追加。  

``` cs
public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public string Test { get; set; }
}
```

再びSecondでマイグレーションファイル作成。  
`dotnet ef migrations add Second`  

生成されたUp,DownメソッドはAddColumnではなくRenameColumnとなっている。  
これはSnapshotにある「Description」が「Test」に名称変更された、ということになる。  

``` cs
public partial class Second : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Description",
            table: "Product",
            newName: "Test");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Test",
            table: "Product",
            newName: "Description");
    }
}
```

■**Fourth**  

またSecondマイグレーションファイルを直接削除。  
今度はSnapshotの「Description」も手動で削除してSecondでマイグレーションファイルを発行。  
`dotnet ef migrations add Second`  

``` cs
public partial class Second : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Test",
            table: "Product",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Test",
            table: "Product");
    }
}
```

AddColumnとなった。  
SnapShotに「Description」の情報も「Test」の情報もないので、新規追加と判断されたことが分かる。  

以上を持って、SnapShotは現在の状態を保持し、マイグレーションファイルの生成はSnapshotを元に計算され、生成されることが分かった。  

---

## MigrationメソッドでDownを実行する

dotnet-efコマンドやDbContextクラス等でも、直接的なDownコマンドは存在しない。  
Downしたい場合は、マイグレーションファイル名を直接指定する必要がある。  
そうすることで指定したマイグレーションまで戻ることができ、その時Downメソッドが実行される。  
DbContextクラスのMigrationメソッドを普通に使う場合、引数を受け付けるようにはできていないので、ファイル名を指定したDownを実行することができないが、参考リンク先の方法を使えばそれができるようになる。  

>まず知っておくべき事として、実は、dbContext.Database オブジェクトは、`IInfrastructure<IServiceProvider>` インターフェースを実装している。  
>このインターフェース経由で dbContext.Database オブジェクトに問い合わせすると、dbContext.Database オブジェクトが隠し持っている (?) 各種サービスの参照を手に入れることができる。  
>そして、それらサービスのひとつとして、EFCore におけるマイグレーションの諸々を司る IMigrator インターフェースのサービスがある。  
>この IMigrator インターフェースには、指定の名前のマイグレーション定義にまでマイグレーションを進める、引数に対象マイグレーション定義名を持つ、`MigrateAsync(string)` 又はその同期バージョンである `Migrate(string)`メソッドが用意されているのだ。  
>
>``` cs
>var services = dbContext.Database as IInfrastructure<IServiceProvider>;
>var migrator = services.GetService<IMigrator>();
>await migrator.MigrateAsync("M2");
>```
>
>これで指定したマイグレーション定義、上記例だと "M2" までのマイグレーション適用が可能とな>る。
>[Entity Framework Core + Code Style で、指定名のマイグレーションまでに留めてマイグレーションする](https://devadjust.exblog.jp/28746582/)  

因みにリンク先の内容はM1,M2,M3という移行が必要でM2まで適応させたい場合の方法についての解説しているところである。

---

## IMigrator インターフェイス

>EF Core Migrations スクリプトを生成したり、データベースを直接移行したりするために使用されるメイン サービス。  
[IMigrator インターフェイス](https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.entityframeworkcore.migrations.imigrator?view=efcore-6.0)  

こいつを掌握できればコマンドからしかアクセスできない処理でも実行できるかもしれない。
