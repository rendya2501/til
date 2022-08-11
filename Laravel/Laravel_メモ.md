# Laravelメモ

---

## LaravelのVender機能だけをダウンロードするやり方

[【php】Laravelで/vendor以下のディレクトリが見つからない時](https://mokabuu.com/it/php/%E3%80%90php%E3%80%91laravel%E3%81%A7-vendor%E4%BB%A5%E4%B8%8B%E3%81%AE%E3%83%87%E3%82%A3%E3%83%AC%E3%82%AF%E3%83%88%E3%83%AA%E3%81%8C%E8%A6%8B%E3%81%A4%E3%81%8B%E3%82%89%E3%81%AA%E3%81%84%E6%99%82)  
[git pullコマンドを実行したらvendorのファイルが開けなくなってエラー（Failed opening required）](https://laraweb.net/practice/7129/)  

`composer update`か`composer install`をやってみればいいっぽい。  
実際に会社でやってみて、その結果を後で書く。
composer.jsonとかcomposer.lockとかあるので、ダウンロードすべき情報はそちらに乗っていて、それを基にcomposerが必要なダウンロードを行ってくれるっぽいので、
そうしたらvenderフォルダが出来上がるっぽいです。  
全部ぽいだ。  

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

・cronとLaravel
app/Console/Commands
app/Console/Kernel.php

Kernel.phpのCommand配列とCommandメソッドに色々登録すると、
各種コマンドクラスのhandleメソッドが実行される。
これが最低限のcronとLaravelの連携。

---

## ルーティングサンプル

``` PHP
    Route::group(['prefix' => 'layer1'], function () {
        Route::group(['prefix' => 'layer2'], function () {
            Route::group(['prefix' => 'hogehoge'], function () {
                Route::get('/list/{code}', 'Api\layer1\layer2@gethogehogeList');
                Route::put('/', 'Api\layer1\layer2@hogehoge');
            });
        });
    });
```

---

## Laravel 405

最後にスラッシュを入れるとリダイレクトになるらしいが、なぜ？  
開発環境ではならないのに、61や本番で発生したのはなぜ？  
WEBサーバーの違いによるものなのか？  
→  
聞いてみたけど、わからないらしい。  
3年くらい前にもマスターでそういうことが起こったことがあるってだけで、根本的な原因はわからないってKn君が言ってた。  

ちなみに405は許可されていないメソッド（GETとかPOSTとか）でアクセスしようとした時に発生するエラーらしい。  
[405エラー](https://wa3.i-3-i.info/word15669.html)  
Mehotd Not Allowed  
Allowed : 許可された  

[Laravelで405や419エラーに遭遇した場合の対処法](https://qiita.com/aminevsky/items/04cdf17686e28c9847c4)  
結構記事はあるけども、どれもルーティングに関する簡単なミスばかり。  

---

## ログ出力

ファサードで整えられたログ出力ではなく、プレーンな状態でのログ出力の方法が分からなかったのでメモ。  

①use宣言する  

``` php
use Illuminate\Support\Facades\Log;
```

②Log::ファシリティ(メッセージ)でログ出力できる。  

``` php
Log::emergency($message);
Log::alert($message);
Log::critical($message);
Log::error($message);
Log::warning($message);
Log::notice($message);
Log::info($message);
Log::debug($message);
```

どのファシリティでも出力されるけど、とりあえずdebug使っておけばいいんじゃないかな。  
因みに出力先は、デフォルトの設定の場合、`storage/logs/laravel.log` に出力される。  

[Laravelでログを出力する](https://uedive.net/2019/3625/laravellog/)  

---

## json->getbody()->rewind()

[Guzzle getContents（）-> getBody（）-2回目の呼び出しで空の文字列が返されます](https://stackoverflow.com/questions/55120359/guzzle-getcontents-getbody-second-calls-return-empty-string)  
>応答本体はストリームであるため、これは予想される動作です（PSR-7仕様で詳細を参照してください）。  
>本文を再度読み取ることができるようにするには->getBody()->rewind()、ストリームを最初に巻き戻すように呼び出す必要があります。  
>すべてのストリームタイプが巻き戻し操作をサポートしているわけではないため、まれに例外が発生する可能性があることに注意してください。  

---

## Symfony\Bridge\PsrHttpMessage\Factory\HttpFoundationFactory' not found

プロキシでレスポンスをそのまま返そうとした時にエラーが発生。  
結果的に`symfony/psr-http-message-bridge`がないことが原因だった。  
composerでインストールしようとしたら嵌ったのでまとめる。  

主にphpのメモリのエラーとローカルでインストールしようとした時のcomposerとphpのバージョンの違いによるエラーでにっちもさっちも行かなくなった。  
バージョンを細かく指定して、メモリーの設定を一時的に外したらうまくいった。  

メモリーの制限を外しつつ、バージョンを指定したインストールのコマンド。  
`COMPOSER_MEMORY_LIMIT=-1 composer require symfony/psr-http-message-bridge:"1.3.0"`  

[Laravel Guzzle GET request](https://stackoverflow.com/questions/51331903/laravel-guzzle-get-request/55671570)  
[composerでgitの公開レポジトリからcloneする際のエラーの回避方法](https://qiita.com/pikanji/items/8997db5a773372393b02)  
[Composerのバージョン指定方法でのチルダ（~）とキャレット（^）の違い](http://blog.a-way-out.net/blog/2015/06/19/composer-version-tilde-and-caret/)  
[composer requireで"Allowed memory size of 1610612736 bytes exhausted"エラーが出た場合の対処法](https://qiita.com/96kuroguro/items/51a7be874e624227a3bb)  
[よく使うcomposerコマンドとバージョン指定方法の備忘録](https://tanden.dev/%E3%82%88%E3%81%8F%E4%BD%BF%E3%81%86composer%E3%82%B3%E3%83%9E%E3%83%B3%E3%83%89%E3%81%A8%E3%83%90%E3%83%BC%E3%82%B8%E3%83%A7%E3%83%B3%E6%8C%87%E5%AE%9A%E3%81%AE%E5%82%99%E5%BF%98%E9%8C%B2/)  
[laravel api return]  
[symfony/psr-http-message-bridge](https://packagist.org/packages/symfony/psr-http-message-bridge)  

``` txt
        // try {
        //     Log::debug('1');
        //     Log::debug($response->getStatusCode()); // HTTP status code;
        //     Log::debug($response->getReasonPhrase()); // Response message;
        //     // Log::debug(json_decode('   ',true)); // Body as the decoded JSON;
        //     // Log::debug(\GuzzleHttp\json_decode('   ', true)); 
        //     Log::debug((string) $response->getBody()); // Body, normally it is JSON;
        //     // Log::debug($response->getHeaders()); // Headers array;
        //     // Log::debug($response->hasHeader('Content-Type')); // Is the header presented?
        //     // Log::debug($response->getHeader('Content-Type')[0]); // Concrete header value;
        //     return json_decode($response->getBody(), true);
        // } catch (Exception $e) {
        //     Log::debug('2');
        // }
```
