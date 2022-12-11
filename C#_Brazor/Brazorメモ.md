# Blazorメモ

[BlazorSample](https://github.com/rendya2501/BlazorSample)  

---

## ファンクションのキーバインド

ファンクションのキーバインド設定可能。  
ダイアログ上でF5を掌握できることを確認した。  

``` razor
@if (ShowConfirmation)
{
    <div class="modal fade show" @onkeydown="KeyHandler" @onkeydown:preventDefault style="display:block" tabindex="-1">
    </div>
}

@code {
    private void KeyHandler(KeyboardEventArgs e)
    {
        if (e.Key == "F5")
        {
            _ = 1;
        }
    }
}
```

[ASP.NET Core Blazor のイベント処理](https://learn.microsoft.com/ja-jp/aspnet/core/blazor/components/event-handling?view=aspnetcore-6.0)  

---

## Styleタグ中のcharset

`@charset "utf-8";`はrazorファイルのStyleタグの中に記述できない。  
でも、全体のプロジェクトではutf8が適応されているっぽいので、別に書かなくても問題ない模様。  

---

## BrazorのCSSの分離

トップレベルにCSSを配置できることは分かったが、CSSってどうやって適応されていくのか？  
トップレベルに大本となるCSSを配置して、微妙に色などを変えたい場合はそれぞれのrazorファイルにCSSを書くのがお作法なのだろうか。  
bootstrapを使いつつ、共通的に使いたいcssコンポーネントを作った時、それはどこに配置すべきなのだろうか。  
そもそもCSSって後書きが強いのか？  
CSSの勉強しないとわからんな。  

[【Blazor Server】【ASP.NET Core】CSS isolation と MapControllers](https://mslgt.hatenablog.com/entry/2020/12/16/203458)  

---

## 静的ファイルを wwwroot 以外のフォルダに配置する

ASP.Netではwwwrootなるフォルダに静的コンテンツを配置する決まりになっている模様。  
各プロジェクトに静的コンテンツを配置したくばスタートアッププログラムで追加の設定が必要。  
その手順は全て参考サイトに乗っているので、そちらを参照されたし。  

[静的ファイルを wwwroot 以外のフォルダに配置する](https://sorceryforce.net/ja/tips/asp-net-core-content-static-file-another-folder)  

---

## blazor multi select bind

[.NET6 Blazor selectタグ multiple @bind](https://sumomo.ohwaki.jp/wordpress/?p=406)  

---

RazorでのDapperサンプル（dynamicで受け取る場合）
dapper-smple1.cshtml

``` cs
@using Dapper;

<!DOCTYPE html>
<html lang="en">
    <head >
        <meta charset="utf-8" />
        <title></ title>
    </head>
    <body>
        <ul>
        @foreach( var d in users){
            <li> @d.Name : @ d.Email : @d.Age</li>
        }
        </ul>
    </body>
</html>

@{
    dynamic users;
    using( var cn = new System.Data.SqlServerCe.SqlCeConnection("接続文字列"))
    {
       cn.open();
       users　= cn.Query("SELECT * FROM users");
    }
}
```

[kiyokura/dapper-smple1.cshtml](https://gist.github.com/kiyokura/7185300)  

---

## 一覧テンプレート

``` html
<table class="table table-striped align-middle table-bordered">
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var Item in List)
        {
            <tr>
                <td>@Item.ID</td>
                <td>@Item.Name</td>
                <td>
                    <a href='/api/edit/@Item.ID' class="btn btn-secondary" role="button">
                        Edit
                    </a>
                    <a href='/api/delete/@Item.ID' class="btn btn-danger" role="button">
                        Delete
                    </a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

## aa

引数がない場合はnew Actionする必要はない。  
どちらの書き方でもいいが、長く書く必要はないだろう。  

``` html
@onclick="() => Cancel()"
@onclick="Cancel"
```

引数がある場合はnew Actionしなければエラーとなる。  

``` html
@onclick="() => Cancel(user.UserId)"

void Cancel(int i)
{
    _ = i;
}
```

---

## 参考サイト

[.NET 6 と Entity Framework Core InMemory を使用した Blazor Server CRUD](https://www.youtube.com/watch?v=ii6QzWudZ6E)  
