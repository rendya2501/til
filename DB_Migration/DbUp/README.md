# DbUp

.Netのマイグレーションツール「**DbUp**」の開発についてまとめる

---

## dbup-cli

cli操作を可能とするためのライブラリ

[GitHub - drwatson1/dbup-cli: Cross platform command line tool that helps you to deploy changes to SQL Server databases](https://github.com/drwatson1/dbup-cli)  
[NuGet Gallery | dbup-cli](https://www.nuget.org/packages/dbup-cli)  

---

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

---

## 本家

[Home - DbUp](https://dbup.readthedocs.io/en/latest/)  
[GitHub - DbUp/DbUp: DbUp is a .NET library that helps you to deploy changes to SQL Server databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.](https://github.com/DbUp/DbUp/)  
