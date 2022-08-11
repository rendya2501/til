# Laravel 環境構築まとめ

---

## 環境構築手順

1. Composerインストール  
2. ComposerでLaravelプロジェクトを作成  
3. プロジェクト起動  

---

## プロジェクトの作成と起動

3年前でVersion5.6の話だが、2021/09/19現在でもこのやり方で問題なく起動出来た。  

1. Laravelプロジェクトの作成  
`$composer create-project --prefer-dist laravel/laravel SampleProject`  

2. プロジェクトに移動  
`$cd SampleProject`  

3. 起動  
`$php artisan serve`  
Laravel development server started: <http://127.0.0.1:8000>

4. `http://localhost:8000/`にアクセス  
Laravelのロゴとページが表示されればOK

[Laravelプロジェクトの作成と起動](https://qiita.com/rokumura7/items/ae9b89a6244d4b392bf9)  

---

## php artisan serveを停止させるコマンド

`Ctrl + C`  

[php artisan serveを停止させる方法](https://qiita.com/janet_parker/items/9bac1173b33175cc54df)  

---

## Laravelのバージョン確認

[Laravelのバージョンを確認するコマンド](https://qiita.com/shosho/items/a7ea8198f8923b08e1dd)  

`php artisan --version`  
もしくは  
`php artisan -V`  

因みに書かれている場所はここ  
`vendor/laravel/framework/src/Illuminate/Foundation/Application.php`  
`Ctrl + P`で飛べ  

---

## Composerインストールで躓いた

ここと全く同じ。  
[ComposerのWindowsインストーラー実行時に発生したエラーへの対処](https://qiita.com/ienoue/items/d8f7c40fcd3528f38717)  
>Xdebugの設定をxdebug.start_with_request = yesにしてあるのが原因。
必要がないときもXdebugが呼び出されてしまう。  

php.iniに以下の行を追加することで解決した。  
`xdebug.log_level=0`  

インストール出来たらシェルでコマンドを実行。  
`composer -v`  

応答が帰ってきたらインストールは成功  
