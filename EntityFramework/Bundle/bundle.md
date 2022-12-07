# Bundle

## バンドル概要

バンドルは移行を実行するために必要な全ての情報を内包した実行ファイル。
CLI等のツールは依存関係（.NET SDK、プログラム、移行ファイル、ツール自体等）を整える必要があるが、バンドルはそれらを内包しているので、環境に左右されず、単体で移行を行うことができる。

- 移行バンドルは、移行を実行するために必要なすべてのものを含む自己充足的な実行ファイル(exe)  
- バンドルは実行環境に影響を受けずにマイグレーションを実行することができる。  
  - CLIはツールの依存関係（.NET SDK、モデルをコンパイルするためのソースコード、ツール自体）が本番サーバーにインストールされている必要がある。  

- 移行バンドルはコマンドラインインターフェースと同じアクションを実行する。  
- 実質的にdotnet ef database updateと同等の動作を提供する。  

- 主要なツール (Docker、SSH、PowerShell 等) で動作する。  
- EFCore6.0(.Net6)からの機能  
- 移行を実行するだけの機能なので、細かい制御はできない。  

>移行バンドルは、データベースに移行を適用するために使用できる単一ファイルの実行可能ファイルです。  
>これらは、次のような SQL スクリプトとコマンドライン ツールの欠点の一部に対処します。  
>
>- SQL スクリプトを実行するには、追加のツールが必要です。  
>- これらのツールによるトランザクション処理とエラー時の続行動作には一貫性がなく、予期できない場合もあります。  
>  そのため、移行の適用時にエラーが発生した場合に、データベースが未定義の状態になる可能性があります。  
>- バンドルは、CI プロセスの一部として生成することができ、後で配置プロセスの一部として簡単に実行することができます。  
>- バンドルは、.NET SDK または EF ツールを (または、自己完結型の場合は .NET ランタイムさえも) インストールせずに実行でき、プロジェクトのソース コードは必要ありません。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#bundles)  

<!--  -->
>生成される実行可能ファイルは、既定では efbundle という名前になります。  
>これは、データベースを最新の移行に更新するために使用できます。  
>これは、dotnet ef database update または Update-Database を実行するのと同等です。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#efbundle)

---

## バンドルの作成

コンソール(コマンドプロンプトやgitbash)で以下のBundle作成コマンドを実行する。  

`dotnet ef migrations bundle`  

デフォルトではwin-x64のexeが生成される。  

---

## バンドル発行バッチ

``` batch : BuildBundle.bat
@echo off
chcp 65001

@echo;
@echo --- AllStart ---
@echo;
@echo --- win-x64_Start ---
dotnet ef migrations bundle --self-contained -r win-x64 --output efbundle_win-x64.exe --force
@echo --- win-x64_Finished ---
@echo;
@echo --- win-x86_Start ---
dotnet ef migrations bundle --self-contained -r win-x86 --output efbundle_win-x86.exe --force
@echo --- win-x86_Finished ---
@echo;
@echo --- linux-x64_Start ---
dotnet ef migrations bundle --self-contained -r linux-x64 --output efbundle_linux-x64 --force
@echo --- linux-x64_Finished ---
@echo;
@echo --- AllFinished ---
@echo;

pause > nul
```

---

## バンドルの使い方

コンソールから接続情報を入力して実行  

`efbundle --connection "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>;`  

windowsの場合はexeなので、ダブルクリックで直接実行もできるが、即移行が実行され、結果を確認できないので、おすすめはしない。  
そもそも、ダブルクリックで必ずエラーとなるようにバンドルを発行する。  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

---

## バンドルでdownを実行

bundleにDownコマンドは存在しないが、Downは可能。  
マイグレーションファイルを指定することで、その時点そこまで戻すことができる。  

しかし、バンドルのコマンドで内包しているマイグレーションファイル一覧を出力する機能はないので、戻すファイルを指定する時は、データベースの適応履歴からファイル名を検索して戻すことになる。  

例:  
`efbandle 20221110062826_Second`

---

## 参考

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  
[EF Core 6.0: Introducing Migration Bundles](https://jaliyaudagedara.blogspot.com/2021/08/ef-core-60-introducing-migration-bundles.html?spref=tw)  
