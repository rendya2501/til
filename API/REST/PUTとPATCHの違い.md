# PUTとPATCHの違い

PUT: 既存リソースの完全な更新（リソース全体の置き換え）  
PATCH: 既存リソースの部分的な更新  

---

以下のようなテーブルがあるとする。  

``` cs
class Hoge {
    public int Id {get;set;}
    public string Name {get;set;}
    public string Description {get;set;}
}
```

以下のようなリクエストを送ったとする。  

``` cs
{
    Name = "hoge"
}
```

Putは全て置き換えるので、IdとDescriptionはnullとなる。  
Patchであれば、IdとDescriptionはそのままで、Nameだけ更新する形となる。  

これが全てのリソースを変更するか一部のリソースを変更するかの違いとなる。  

[PUTとPATCHの違いは何？](https://techblg.app/articles/difference-between-put-and-patch/)  
