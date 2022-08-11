# Laravel 環境構築まとめ

---

## 環境構築手順

1. composerインストール  
2. composerでLaravelプロジェクトを作成  
3. プロジェクト起動  

---

## プロジェクトの作成と起動

composerでの手順。  
laravelコマンドなるもので作成もできるらしい。  

`SampleProject`というプロジェクトを作成する。  

1. Laravelプロジェクトの作成  
`$composer create-project --prefer-dist laravel/laravel SampleProject`  

2. プロジェクトに移動  
`$cd SampleProject`  

3. 起動  
`$php artisan serve`  
Laravel development server started: <http://127.0.0.1:8000>

4. `http://localhost:8000/`にアクセス  
Laravelのロゴとページが表示されればOK  

2021/09/19現在でもこのやり方で問題なく作成出来た。  
2022/08/11でも問題なかった。  

[Laravelプロジェクトの作成と起動](https://qiita.com/rokumura7/items/ae9b89a6244d4b392bf9)  

---

## 停止コマンド

`Ctrl + C`  

[php artisan serveを停止させる方法](https://qiita.com/janet_parker/items/9bac1173b33175cc54df)  

---

## Laravelのバージョン確認

`php artisan --version` or `php artisan -V`  

因みに書かれている場所はここ  
`vendor/laravel/framework/src/Illuminate/Foundation/Application.php`  

`Ctrl + P`で飛べ  

[Laravelのバージョンを確認するコマンド](https://qiita.com/shosho/items/a7ea8198f8923b08e1dd)  

---

## --prefer-distとは？

「composer create-project」する際のオプションである、「--prefer-dist」とは何か疑問に思ったのでまとめ。  
「--prefer-dist」以外にも「--prefer-source」なるものがあるらしい。  

結論：**バージョン管理システムを含むか含まないか**  

``` txt : prefer-distとprefer-sourceの違い
--prefer-dist  ：バージョン管理を含めないでプロジェクト作成
--prefer-source：バージョン管理を含めてプロジェクト作成
```

`composer help install`コマンドで確認可能  

``` txt
> composer help install
Options:
  --prefer-source  Forces installation from package sources when possible, including VCS information.
  --prefer-dist    Forces installation from package dist even for dev versions.
```

--prefer-sourceの方には「including VCS information」と書いてある。  
--prefer-sourceの方はバージョン管理を含めてプロジェクト作成すると言うこと。  

[(composer) –prefer-distと–prefer-sourceの違い](https://hara-chan.com/it/programming/prefer-dist-prefer-source-difference/)  

---

## CreateProject時のエラー対応

>Alternatively, you can run Composer with \`--ignore-platform-req=ext-fileinfo\` to temporarily ignore these required extensions.

と言われた。  

>パッケージをインストールできませんでしたという内容です。  
「Problem 1」のところを読むと、Laravelは「league/flysystem」を必要とする。この「league/flysystem」が「ext-fileinfo」を必要とする。  
しかし「ext-fileinfo」が入っていない、あるいは許可されていないというエラーです。  

&emsp;

>対処法は簡単で、php.iniファイルに「extension=php_fileinfo.dll」を追記します。もし、既に記述がある場合はコメントアウトを外します。

`extension=php_fileinfo.dll`

[【Laravel】インストールエラー対処法を実例で解説：Your requirements could not be resolved to an installable set of packages.｜require league/flysystem ^1.1 -> satisfiable](https://prograshi.com/framework/laravel/installation-error-require-ext-fileinfo/)  
