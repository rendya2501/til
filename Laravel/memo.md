# Laravel関係の色々

---

## プロジェクトの作成と起動

[Laravelプロジェクトの作成と起動](https://qiita.com/rokumura7/items/ae9b89a6244d4b392bf9)  
3年前でVersion5.6の話だが、2021/09/19現在でもこのやり方で問題なく起動出来た。  

Laravelプロジェクトの作成  
`$ composer create-project --prefer-dist laravel/laravel SampleProject`  

プロジェクトに移動  
`$cd SampleProject`  

起動  
`$php artisan serve`  
Laravel development server started: <http://127.0.0.1:8000>

### php artisan serveを停止させる方法

[php artisan serveを停止させる方法](https://qiita.com/janet_parker/items/9bac1173b33175cc54df)  
起動があるならもちろん停止も知っておかないとね。  

`Control + C`  

## バージョン確認

[Laravelのバージョンを確認するコマンド](https://qiita.com/shosho/items/a7ea8198f8923b08e1dd)  

`php artisan --version`  
もしくは  
`php artisan -V`  

因みに書かれている場所はここ  
`vendor/laravel/framework/src/Illuminate/Foundation/Application.php`  
`Ctrl + P`で飛べ  

---

## Controller→WorkerServiceのサンプル

[Laravel でサービス(Service)クラスの作り方](https://qiita.com/ntm718/items/14751e6d52b4bebde810)  
[Laravel ルーティング[web.php][Route] 8.x](https://noumenon-th.net/programming/2019/09/25/route/)  
https://daiki-sekiguchi.com/2018/08/31/laravel-how-to-make-service-class/
https://himakuro.com/laravel-service-class-guide

---

## LaravelのDB接続情報

.envファイル

---

## ファサード

<https://qiita.com/minato-naka/items/095f2a1beec1d09f423e>  

staticメソッドのようにメソッドを実行できる仕組み。  
実際にはstaticじゃなくて、ちゃんとnewして実行している。  
詳しくは解説を読むべし。  
読んで思ったのは、うまいこと出来てるんだなって事。  

---

## LaravelのCollection

[【5.5対応】Laravel の Collection を使い倒してみたくなった 〜 サンプルコード 115 連発 1/3](https://qiita.com/nunulk/items/9e0c4a371e94d763aed6)  

Collection : 配列のラッパークラスらしい。  

Laravel 標準の ORM である Eloquent で複数レコードを取得する際に、この Collection のインスタンス (正確には Collection を継承したクラスのインスタンス) で返ってきたりしますが、もちろん、アプリケーション内で自由に使うことができます。  
→  
だからそのままLinq的記述が出来たわけね。  

どうもCollectionはLaravelに含まれている機能というだけで、Collection自体がパッケージとして提供されているわけではなさそうだ。  
なのでCollectionを使うならLaravelを使わないとダメみたい。  
というわけで、次は適当にLaravelプロジェクトを作成して、Collectionを使ってみるだけのサンプルをやってみる事かな。  

---

## IISを使ってLaravelのプロジェクトをwebとして公開するまで

1. IISマネージャーを立ち上げる。  
   フォルダを右クリック、アプリケーションへの変換を選択。  

2. プログラムと機能のWindowsの機能の有効化または無効化より、Webに関係しそうな機能を選択してダウンロード  

3. 権限を設定。アプリケーション側の権限とphpフォルダの権限。  
   おもにIIS系とゲスト系。  
   ③、④はここを参照：<http://create-something.hatenadiary.jp/entry/2014/06/11/194808>  

4. php.iniの設定。ネットを参考に2箇所のステータスを開放。  

5. URL書き換えツールをインストール  
   <https://www.microsoft.com/ja-jp/download/details.aspx?id=7435>  

---

・Laravelプロジェクトを作って、collection,Eloquentのテスト環境を作る

・cronとLaravel
app/Console/Commands
app/Console/Kernel.php

Kernel.phpのCommand配列とCommandメソッドに色々登録すると、
各種コマンドクラスのhandleメソッドが実行される。
これが最低限のcronとLaravelの連携。

