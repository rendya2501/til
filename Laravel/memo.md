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

`Ctrl + C`  

## バージョン確認

[Laravelのバージョンを確認するコマンド](https://qiita.com/shosho/items/a7ea8198f8923b08e1dd)  

`php artisan --version`  
もしくは  
`php artisan -V`  

因みに書かれている場所はここ  
`vendor/laravel/framework/src/Illuminate/Foundation/Application.php`  
`Ctrl + P`で飛べ  

---

## Controller→WorkerServiceを実現する最小のサンプル

①WorkerServiceの作成  
\appにServicesフォルダを作成する。  
そのフォルダの中にTestService.phpを作成する。  

``` php : TestService.php
namespace App\Services;
// 書いた後整形しろ。短くしたいから1文で書いてる
class TestService{ public function Hoge() { echo 'hoge2'; } }
```

②Controllerの作成  
\app\Http\ControllersにTestController.phpを作成する  

``` php : TestController.php
namespace App\Http\Controllers;

use App\Services\TestService;

class TestController extends Controller
{
    private $test;
    public function __construct(TestService $test_service)
    {
        $this->test = $test_service;
    }
    public function index(TestService $test_service)
    {
        $this->test->hoge();
        $test_service->hoge();
    }
}
```

③Routeを通す  
\Routes\web.phpに2文追加  

``` php : web.php
use App\Http\Controllers\TestController;
Route::resource('/test', TestController::class);
```

④アクセス  
Laravel立ち上げて、`/test`でアクセスすると`hoge2hoge2`と表示される  

総評  
サンプルはごまんとあるが、DIの設定したり、色々やっていて、すぐにできるものはなかった。  
もう少し余裕が出来たら、DIも含めた設定とかもやりたいが、とりあえず最小サンプルはこれで。  
これさえ出来れば「Laravelプロジェクトを作って、collection,Eloquentのテスト環境を作る」の案件もクリアしたようなものだろう。  
Collection使うだけならワーカーサービスに`use Illuminate\Support\Collection;`を書けばCollectionは使えるようになるので、後は適当なメソッド作ってそこにアクセスできるようにすればよいだけだ。  

一番参考になったサイト  
[Laravel ルーティング[web.php][Route] 8.x](https://noumenon-th.net/programming/2019/09/25/route/)  
`routes/web.php`の`Route::resource('/sample', SampleController::class);`の書き方がDIを通さず実現する一番手っ取り早い書き方で、他のサイトでは説明されていなかった。  
useしないといけないので、実用的では使ってはいけないだろうけど、とりあえず動かすだけならこれで十分  

他参考サイト  
こいつらはDIからファサードの登録まで実用的で必要最低限の設定をしているので、できるならこっちでやったほうがいい。  
[Laravel でサービス(Service)クラスの作り方](https://qiita.com/ntm718/items/14751e6d52b4bebde810)  
[【Laravel】サービスクラス作成手順](https://daiki-sekiguchi.com/2018/08/31/laravel-how-to-make-service-class/)  
[LaravelでServiceクラスを作成する手順まとめ！](https://himakuro.com/laravel-service-class-guide)  

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

会社のルーティング

``` PHP
    // プラン連携メンテナンス
    Route::group(['prefix' => 'web-cooperation'], function () {
        Route::group(['prefix' => 'plan-cooperation'], function () {
            Route::group(['prefix' => 'maintenance'], function () {
                Route::get('/list/{golfcode}', 'Api\WebCooperation\PlanCooperationController@getMaintenanceList');
                Route::put('/', 'Api\WebCooperation\PlanCooperationController@maintenancePlan');
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
3年くらい前にもマスターでそういうことが起こったことがあるってだけで、根本的な原因はわからないって金谷君が言ってた。  

ちなみに405は許可されていないメソッド（GETとかPOSTとか）でアクセスしようとした時に発生するエラーらしい。  
[405エラー](https://wa3.i-3-i.info/word15669.html)  
Mehotd Not Allowed  
Allowed : 許可された  

[Laravelで405や419エラーに遭遇した場合の対処法](https://qiita.com/aminevsky/items/04cdf17686e28c9847c4)  
結構記事はあるけども、どれもルーティングに関する簡単なミスばかり。  
TeelaのProxy周りが特殊なのかもねぇ。  

---

## Laravelでログを出力する

[Laravelでログを出力する](https://uedive.net/2019/3625/laravellog/)  
ファサードで整えられたログ出力ではなく、プレーンな状態でのログ出力の方法が分からなかったのでメモ。  

①use宣言する  
`use Illuminate\Support\Facades\Log;`  

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
