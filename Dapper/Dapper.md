# Dapper

---

## 概要

- C#でDBアクセスするためのライブラリ  
  - IDbConnection というインターフェースを拡張するライブラリ  
- .NET 環境で使えるシンプルなORマッパー  
  - SQLを実行して取得した結果をオブジェクトに対していい感じにマッピングしてくれる。  

- ORマッパーと言えばEntity Frameworkが思いつきますが、速度面ではDapperの方が圧倒的に優れている  
- Entity Frameworkほど高性能なことはできない  

[Dapperを使ったデータマッピングのはじめ方](https://webbibouroku.com/Blog/Article/dapper)  
[【C#】Dapperの使い方](https://pg-life.net/csharp/dapper/)  
[Dapper本家](https://dapper-tutorial.net/dapper)  

---

## Entity Framework との違い

- .NET で使える ORM の一つ。  
- データベースの情報とオブジェクトを直接マッピングすることで、データベースを意識することなく開発が行えるようになる。  
- 機能的には非常に高性能だが、いろいろ面倒。  

---

## Dapper できること

- データのマッピング  
- DB操作のラッピング  

---

## Dapper できないこと

- クエリ(SQL)の自動生成  
- マッピングするクラスの自動生成  

Dapperはあくまでオブジェクトへのデータマッピングが主な機能なので、SQLを自動で生成することはできない。  
SQLは自分で書く必要があります。  
同様にマッピングするクラスを自動的に生成することもできません。  

---

## Dapper のインストール

Nugetで提供されているので、Dapperで検索してインストールすればよろしい。  
因みにアイコンはDという文字を起点になんか色々線が入ってるやつ。  
