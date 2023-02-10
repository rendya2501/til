# DbUp

.Netのマイグレーションツール「**DbUp**」の開発についてまとめる

---

## 本家

[Home - DbUp](https://dbup.readthedocs.io/en/latest/)  
[GitHub - DbUp/DbUp](https://github.com/DbUp/DbUp/)  

---

## dbup-cli

DbUpのcli操作を可能とするためのライブラリ  

[GitHub - drwatson1/dbup-cli](https://github.com/drwatson1/dbup-cli)  
[NuGet Gallery | dbup-cli](https://www.nuget.org/packages/dbup-cli)  

---

## DbUp.OnChange

DbUpの拡張ライブラリ。  
DbUpをベースに更なるオプションが追加されている。  

履歴テーブルに残して実行しない設定等、細かな設定が可能となっている。  
→研究の結果、本家のDbUpでも実現できることが分かったので、採用する理由はなくなった。  

しかし、SQLServer専用となってしまった。  

[Database migrations with DbUp - BitLoop Blog](https://blog.bitloop.tech/database-migrations-with-dbup/)  
[GitHub - BitLoopTech/DbUp.OnChange](https://github.com/BitLoopTech/DbUp.OnChange)  

---

## 時系列を採用すべき理由

>スクリプトは冪等であるべき(同じ操作を何度繰り返しても、同じ結果が得られる性質)  
>スクリプトは変更、削除してはいけない  
>膨大なスクリプトのリストが増えるのが心配なら、DbUpプロジェクトをフォルダ構造で開始することができます。  
>201902/20190220203000-AddCustomerTable.sql  
>201902/20190220203012-AddProductTitleTagIndex.sql  
>これで、各フォルダには、数ヶ月分の変更点があるだけです。  
>[5 DbUp tips from Paul Stovell | Steve Fenton](https://www.stevefenton.co.uk/blog/2019/02/5-dbup-tips-from-paul-stovell/)  

<!--  -->
>フォルダ内のファイル名は一意でなければならず、通常は現在の日付と時間、またはプロジェクトの発券システムを使用している場合はチケット番号を追加します  
>(例: Migration_20200430_1130_P0001_00001.sql)．  
>[Database migrations with DbUp - BitLoop Blog](https://blog.bitloop.tech/database-migrations-with-dbup/)  

---

フォルダの実行順を制御する方法  
[DbUp/run-group-order.md at master · DbUp/DbUp · GitHub](https://github.com/DbUp/DbUp/blob/master/docs/more-info/run-group-order.md)  

DbUpの公式の紹介を更にわかりやすく解説してくれているサイト。  
翻訳が面白かったので残す。  
>実稼働環境ではこのフォールバック値を使用しないことをお勧めします。移行が成功したと思っても、フォールバック値のせいで間違ったデータベースに移行されたのでは、たまったものではありません。  
[Database Migrations with DbUp – Eric L. Anderson](https://elanderson.net/2020/08/database-migrations-with-dbup/)  

毎回実行したいクエリのやり方の紹介。  
[Always Run Migrations with DbUp – Eric L. Anderson](https://elanderson.net/2020/08/always-run-migrations-with-dbup/)  

クエリでは無く、コードから移行スクリプトを生成する方法の紹介。  
[Code-based Database Migrations with DbUp – Eric L. Anderson](https://elanderson.net/2020/08/code-based-database-migrations-with-dbup/)  

Webプロジェクトとして含めた例の紹介記事  
[Database Migration with DbUp + Postgresql + Dapper in ASP.Net core | Medium](https://medium.com/@niteshsinghal85/dbup-postgresql-dapper-in-asp-net-core-c3be6c580c54)  

ジャーナルのカスタマイズについての記事  
[C#: Tutorial: Building Custom DbUp Journal | by Joseph Saravanan Ganesan | Medium](https://medium.com/@saravananganesan/c-tutorial-building-custom-dbup-journal-5dae1d77ecfd)  
[GitHub - vaananart/DbUp-CustomJournal](https://github.com/vaananart/DbUp-CustomJournal)  
