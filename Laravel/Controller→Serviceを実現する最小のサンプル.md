# Controller→Serviceを実現する最小のサンプル

---

## 実装

①**Serviceの作成**  
`\app`にServicesフォルダを作成する。  
そのフォルダの中にTestService.phpを作成する。  

``` php : TestService.php
<?php
namespace App\Services;

class TestService{ 
    public function Hoge() { 
        echo 'hoge2'; 
    } 
}
```

②**Controllerの作成**  
`\app\Http\Controllers`にTestController.phpを作成する  

``` php : TestController.php
<?php
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

③**Routeを通す**  
`\Routes\web.php`に2文追加  

``` php : web.php
use App\Http\Controllers\TestController;
Route::resource('/test', TestController::class);
```

④**アクセス**  
Laravel立ち上げて、`/test`でアクセスすると`hoge2hoge2`と表示される  

---

## 総評  

サンプルはごまんとあるが、DIの設定したり、色々やっていて、すぐにできるものはなかった。  
もう少し余裕が出来たら、DIも含めた設定とかもやりたいが、とりあえず最小サンプルはこれで。  

これさえ出来れば「Laravelプロジェクトを作って、collection,Eloquentのテスト環境を作る」の案件もクリアしたようなものだろう。  

Collection使うだけならワーカーサービスに`use Illuminate\Support\Collection;`を書けばCollectionは使えるようになるので、後は適当なメソッド作ってそこにアクセスできるようにすればよいだけだ。  

`routes/web.php`の`Route::resource('/sample', SampleController::class);`の書き方がDIを通さず実現する一番手っ取り早い書き方で、他のサイトでは説明されていなかった。  
useしないといけないので、実務では使えないけど、とりあえず動かすだけならこれで十分。  

---

## 一番参考になったサイト  

[Laravel ルーティング[web.php][Route] 8.x](https://noumenon-th.net/programming/2019/09/25/route/)  

---

## 他参考サイト  

こいつらはDIからファサードの登録まで実用的で必要最低限の設定をしているので、できるならこっちでやったほうがいい。  

[Laravel でサービス(Service)クラスの作り方](https://qiita.com/ntm718/items/14751e6d52b4bebde810)  
[【Laravel】サービスクラス作成手順](https://daiki-sekiguchi.com/2018/08/31/laravel-how-to-make-service-class/)  
[LaravelでServiceクラスを作成する手順まとめ！](https://himakuro.com/laravel-service-class-guide)  
