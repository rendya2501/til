# APIメモ

## コラム

[redhat_An architect's guide to APIs: SOAP, REST, GraphQL, and gRPC](https://www.redhat.com/architect/apis-soap-rest-graphql-grpc)  

---

## APIサービスサイト

[httpbin - HTTP通信のテストに便利なWebサービス＆ソフト](https://softantenna.com/blog/httpbin/)  
[APIbank an AOS company](https://www.apibank.jp/ApiBank/api?category_no=0)  

---

## API Tester

---

## オペレーション対応表

``` txt
OPERATION | SQL    | HTTP/REST
----------+--------+-----------
CREATE    | INSERT | POST
READ      | SELECT | GET
UPDATE    | UPDATE | PUT/PATCH
DELETE    | DELETE | DELETE
```

[Repository Pattern](https://www.youtube.com/watch?v=x6C20zhZHw8)  

---

## PUTとPATCHの違い

PUT : 全てのリソースを変更  
PATCH : 一部のリソース変更  

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
