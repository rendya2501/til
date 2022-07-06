# Eloquent,Collection関係まとめ

---

## LaravelのCollection

[【5.5対応】Laravel の Collection を使い倒してみたくなった 〜 サンプルコード 115 連発 1/3](https://qiita.com/nunulk/items/9e0c4a371e94d763aed6)  

Collection : 配列のラッパークラス  

どうもCollectionはLaravelに含まれている機能というだけで、Collection自体がパッケージとして提供されているわけではなさそうだ。  
なのでCollectionを使うならLaravelを使わないとダメみたい。  
というわけで、次は適当にLaravelプロジェクトを作成して、Collectionを使ってみるだけのサンプルをやってみる事かな。  

Laravel 標準の ORM である Eloquent で複数レコードを取得する際に、この Collection のインスタンス (正確には Collection を継承したクラスのインスタンス) で返ってきたりしますが、もちろん、アプリケーション内で自由に使うことができます。  
→  
だからそのままLinq的記述が出来たわけか。  

---

## ->get() ->first() でそれぞれ何もなかった場合のreturn値

- get → 空Collection  
- first → null  

getの戻り値は一見すると空配列のように見えるが、EloquentなのでCollection型。  
`$test = collect()`をやった時と同じ状態だと思われる。  
ちなみにgettypeで型を調べてみると`object`といわれる。  

``` log
\Log::debug(gettype($get_list))
local.DEBUG: object  
\Log::debug($get_list);の結果
local.DEBUG: []  
```

---

## ->get()->delete()

getしてそのままdeleteできるかと思ったけど、できなかった話。  

```php
// エラーになる
$model->get()->delete()

// こうしないと削除できない
$model->get()->each(
  function($item){
    $item->delete();
  }
);
```

---

## nullのcollectionをeachした場合

nullをeachするとエラーになる。  
foreachと考え方は同じかと思う。  

```php
// first使って結果がnullの場合、eachでエラーになる。
$test = $this->getPlan(553, 111, 111);
$test->each(
    function ($item) {
        \Log::debug($item);
    }
);

// 結果がnullの場合も想定してeachの前にnull判定が必要。
if(!is_null($test)) $test->each(
    function ($item) {
        \Log::debug($item);
    }
);
```

---

## Laravelでコレクションをfilterするとインデックスが連続でなくなる

`->filter()`や`->reject()`すると連番が振られてしまう問題。  
小一時間悩んだが、`->values()`するだけで済む問題だった。  
しかし、知らなければ永遠に調べ続けることになるのだ。  

<https://yoshinorin.net/2018/05/26/laravel-filterd-items-key/>

``` PHP
$filterd = $collection->filter(function ($v) {
    return ($v["category"] === "A");
})->values(); //ここでvaluesを呼ぶ
```

---

## Eloquent SQL出力方法まとめ

②がおすすめ。

```PHP
// ①
\Log::debug($query->toSql());
\Log::debug($query->getBindings());

// ②
\Log::debug(
    vsprintf(
        str_replace('?', '%s', str_replace('?', "'?'", $query->toSql())),
        $query->getBindings()
    )

// ③
\DB::enableQueryLog();  // ①ロギングを有効化する
$answer->details($id);  // ②クエリを実行する
\DB::getQueryLog(); // ③ログを出力する
```

---

## LaravelでDBからデータを取得するとき、1つのカラムの情報だけを取得したい場合

valueメソッドがおすすめ  
[いつものところ](https://blog.capilano-fw.com/?p=665#select)  

```php
->select('FieldName')
->first() ?? 0

// データがなければおそらくnull扱いだと思われる。
// 現に [?? 0]はちゃんと機能する。
->value('FieldName') ?? 0
```

---

## コピーと削除

getした後、ifやforeachを挟んでいるところがたくさんあったけど、eachを使えばそんなことする必要がないよという戒め。  
getは何もなければ空配列が帰ってくる。eachは空配列なら実行されない。  
なので、ifで空配列かどうか判定する必要がないし、foreachと同じ動作なんだからeach使おうよというお話。  

```php : 削除
Models\PlanPriceTable::
    where('Code1', $code1)
    ->where('Code2', $code2)
    ->where('Code3', $code3)
    ->get()
    ->each(
        function ($item) {
            $item->delete();
            Logger::dataChangeLog(Logger::LOG_TYPE_DELETE, $item);
        }
    );
```

``` php : コピー
Models\PlanPriceTable::
    where($base_key)
    ->get()
    ->each(
        function ($master) use ($target_key) {
            $new_master = $master->replicate();
            $new_master->Code1 = $target_key['Code1'];
            $new_master->Code2 = $target_key['Code2'];
            $new_master->Code3 = $target_key['Code3'];
            $new_master->saveWithInsertField();
            Logger::dataChangeLog(Logger::LOG_TYPE_CREATE, $new_master);
        }
    );

// いちいちforeachする必要がない
// foreach ($master_list as $master) {
//     $new_master = $master->replicate();
//     $new_master->GolfCode = $target_key['GolfCode'];
//     $new_master->OpenPlanCode = $target_key['Code'];
//     $new_master->PlanCode = $target_key['PlanCode'];
//     $new_master->saveWithInsertField();
//     Logger::dataChangeLog(Logger::LOG_TYPE_CREATE, $new_master);
// }
```

---

## DBGetでdecimalがstringでgetされる問題

DB上ではフィールドの型がdecimalで定義されているのに、stringで取得されてしまう問題が発生した。  
対処法としてはModelにフィールドの型を定義するためのオプションが用意されているっぽいので、見よう見まねでそれを使った。  
[stack神](https://stackoverflow.com/questions/48288519/eloquent-casts-decimal-as-string)  

```php
class OpenPlanPriceTable extends Model
{
    /**
     * フィールド型
     */
    protected $casts = [
        'MainFee' => 'float',
        'AdditionPrice' => 'float',
        'OtherPrice' => 'float',
    ];
}
```

---

## 配列をカンマ区切りの文字列に変換する方法

implode関数を使用する。  
ちなみにLaravelのEroquent使ってcollectionをカンマ区切りの文字列にするサンプルです。

```php
$some_list = Models\TableModel::
    where()
    // pluckは指定したキーの値だけを取得できる。
    ->pluck('FiledName')
    ->toArray()
    ->implode(',');
```

---

## コレクションがネストしている項目を取得したい場合

pluckメソッドを使うとよい。  

laravel collection where nested  

<https://tektektech.com/laravel-pluck/>  
[Search an array nested in a laravel collection](https://stackoverflow.com/questions/44787584/search-an-array-nested-in-a-laravel-collection)  

```php
    $collection = collect(
        array (
            array (
                'linkage_plan_id' => 'aaaaaabbbbbccccccddd',
                'basis_content' => 
                array (
                    'name' => 'Test1',
                    'base_price' => 18400,
                )
            ),
            array (
                'linkage_plan_id' => 'aaaaaabbbbbccccccddd',
                'basis_content' => 
                array (
                    'name' => 'Test1',
                    'base_price' => 18400,
                )
            )
        )
    );
    $collection->pluck('basis_content')->pluck('base_price')->unique();
    $collection->pluck('basis_content.base_price')->unique();
```

---

## Selectのやり方

全てか、1つのフィールドだけど取得する例はたくさんあるけど、  
複数の特定の列を指定するやり方で迷ったので備忘録として残す。  

``` PHP
User::select('*')->get();

$getBusinessDate = Models\Table::
    where('Code', $golf_code)
    ->select('BusinessDate')
    ->first();

->select(
    'Teble.Code as Code',
    'Teble.Name as Name'
)
```
