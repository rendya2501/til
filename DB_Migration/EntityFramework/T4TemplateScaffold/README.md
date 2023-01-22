# T4テンプレートによるスキャフォールドのカスタマイズ

## 概要

dotnet ef スキャフォールドコマンド  

``` txt
dotnet ef dbcontext scaffold 'Server=TestServer;Database=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -o Entity --context-dir Context --context DatContext --data-annotations --use-database-names --no-onconfiguring --force
```

スキャフォールドコマンドのオプションにおいて`--data-annotations`を指定しても論理名を`Comment`アノテーションとしてスキャフォールドしてくれない。  
これがないと、`Contextクラス`の`OnModelCreating`でFluentを記述しなければならない。  
エンティティだけで完結できない。  

エンティティ量が少なければ手動でいいが、多い場合はそうもいかない。  
`Comment`アノテーションを追加するだけの単純作業はしたくない。  

EFCore7からカスタムテンプレートを用いたスキャフォールド時のカスタマイズが可能になった模様。  
公式のサンプルを眺めつつ、T4テンプレートを編集することで、スキャフォールド時に`Comment`アノテーションを付けた状態でスキャフォールドすることに成功したのでまとめる。  

---

## 実行構築

- windows 10以降  
- DotNetSDK .Net6以上  
- VisualStudioCode or VisualStudio  
  - VSCodeを使う場合、C#関連の拡張がインストールされていること  
- EntityFrameworkCore 7  

---

## 準備

### プロジェクト作成

コンソールプロジェクトを作成する。  

`dotnet new console`  

### dotnet-ef(EFcoreツール)をインストールする  

環境の汚染を考慮してローカルインストールとする。  

``` .NET CLI
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

グローバルインストールの場合は以下のコマンドを実行。

``` .NET CLI
dotnet tool install --global dotnet-ef
```

### ライブラリのインストール

以下のライブラリをNuGetからインストールする。  

- `Microsoft.EntityFrameworkCore`  
- `Microsoft.EntityFrameworkCore.Design`  
- `Microsoft.EntityFrameworkCore.SqlServer`  
- `Microsoft.Extensions.Hosting`  

PowerShellやCommandPromptからCLIで実行する場合は下記コマンドを実行する。  
下記コマンドはNuGet公式サイトからライブラリを検索した時に提示されるコマンドとなる。  
[NuGet Gallery | Microsoft.EntityFrameworkCore 7.0.2](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)  

``` txt
dotnet add package Microsoft.EntityFrameworkCore --version 7.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.*
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7.*
dotnet add package Microsoft.Extensions.Hosting --version 7.*
```

※`7.*`の意味はメジャーバージョン7のうち最新のマイナーバージョンを取得するという意味  

### カスタム リバースエンジニアリング テンプレートのインストール

以下の手順は公式サイトの通りだが、同じようにまとめる。  

dotnet new の EF Core テンプレート パッケージをインストールする。  
コマンドプロンプトやgit bashで下記コマンドを実行する。  

``` txt
dotnet new install Microsoft.EntityFrameworkCore.Templates
```

これで既定のテンプレートをプロジェクトに追加できるようになった。  
次にプロジェクト ディレクトリから書きコマンドを実行する。  

``` txt
dotnet new ef-templates
```

このコマンドを実行すると、プロジェクトに次のファイルが追加される。  

- CodeTemplates/  
  - DbContext.t4  
  - EntityType.t4  

[カスタム リバース エンジニアリング テンプレート - EF Core | Microsoft Learn](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/scaffolding/templates?tabs=dotnet-core-cli)  

---

## テンプレートの編集

`DbContext.t4` テンプレートはデータベースの DbContext クラスをスキャフォールディングするために使用する。  
`EntityType.t4` テンプレートはデータベース内の各テーブルとビューのエンティティ型クラスをスキャフォールディングするために使用する。  
`Comment`アノテーションの対象となるのはEntityなので`EntityType.t4`テンプレートを編集する。  

`<#`と`#>`はディレクティブというらしい。要は始まりと終わりの定義だと思われるが、詳しく解説はしない。  
T4の解説は公式サイトを参照されたし。

---

## 実装

クラス側のCommentアノテーションとして `[Comment("<#= EntityType.GetComment().Replace("\r\n", "\\r\\n") #>")]` ディレクティブを追加する。  

プロパティ側のCommentアノテーションとして `[Comment("<#= property.GetComment().Replace("\r\n", "\\r\\n") #>")]` ディレクティブを追加する。  

`GetComment`メソッドを呼び出すインスタンスが違うことに注意すれば、それ以外はどちらも同じである。  

### クラス側のCommentアノテーションの設定

デフォルトであれば50行目あたりから始まる箇所のコード  

``` t4
#>
/// <summary>
/// <#= code.XmlComment(EntityType.GetComment()) #>
/// </summary>
<#
    }

    if (Options.UseDataAnnotations)
    {
        foreach (var dataAnnotation in EntityType.GetDataAnnotations(annotationCodeGenerator))
        {
#>
<#= code.Fragment(dataAnnotation) #>
<#
        }
        if (!string.IsNullOrEmpty(EntityType.GetComment()))
        {
#>
[Comment("<#= EntityType.GetComment().Replace("\r\n", "\\r\\n") #>")]
<#
        }
    }
#>
```

### プロパティ側のCommentアノテーションの設定

デフォルトであれば80行目あたりから始まる箇所のコード  

``` t4
#>
    /// <summary>
    /// <#= code.XmlComment(property.GetComment(), indent: 1) #>
    /// </summary>
<#
        }

        if (Options.UseDataAnnotations)
        {
            var dataAnnotations = property.GetDataAnnotations(annotationCodeGenerator)
                .Where(a => !(a.Type == typeof(RequiredAttribute) && Options.UseNullableReferenceTypes && !property.ClrType.IsValueType));
            foreach (var dataAnnotation in dataAnnotations)
            {
#>
    <#= code.Fragment(dataAnnotation) #>
<#
            }
            if (!string.IsNullOrEmpty(property.GetComment()))
            {
#>
    [Comment("<#= property.GetComment().Replace("\r\n", "\\r\\n") #>")]
<#
            }
        }

        usings.AddRange(code.GetRequiredUsings(property.ClrType));
```

---

## 解説

`EntityType.t4`テンプレートにはスキャフォールドした後のエンティティと対応するであろう部分が見受けられる。  
一番の目安になるのはドキュメントコメントだろう。  
ドキュメントコメントの後にアノテーションが追加されるのでそこを見る。  
すると、そこには`foreach`でアノテーションに対応しているであろう部分が見受けられるので、そこに`[Comment("")]`を差し込めばよい。  

`Comment`アノテーションの内容としてはドキュメントコメントと同じ内容が入ればよいので、そのままドキュメントコメントで使っている式を差し込む。  
これがクラス側とプロパティ側で使用しているインスタンスが違うので注意する。  

`GetComment()`メソッドの後の`Replace`関数はドキュメントコメントに改行が含まれていた場合、スキャフォールド後、そのまま改行されてコンパイルエラーとなるため`\r\n`に置き換えるために必要である。  

`[Comment("")]`を差し込む前後に`if (!string.IsNullOrEmpty(○○.GetComment()))`のブロックが必要。  
これでisNull判定を行わないとスキャフォールド時にエラーが発生してスキャフォールドできなかったためである。  

プロパティ側だけインデントを4つ設けているが、これは必要。  
このインデントはそのままスキャフォールドした時に反映されるので、4つ開けておかないと、プロパティ側の`Commnet`アノテーションだけインデントの位置がずれてしまう。  
クラス側は必要ないが、もちろん4つ設ければそのまま反映される。  
