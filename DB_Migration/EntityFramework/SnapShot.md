# スナップショット

現在の状態を定義しているファイル  

`dotnet ef migration add`コマンドでマイグレーションファイルを生成する時に、エンティティの状態とスナップショットとの差分を計算し、必要なUp,Downを生成する。  
そのために必要な現在の状態を定義しているファイルがスナップショット。  

---

## 解説サイト全文

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

---

## スナップショット検証

### First

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

### Second

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

### Third

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

### Fourth

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
