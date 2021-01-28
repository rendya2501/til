# Eloquent関係

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

## ->get() ->first() でそれぞれ何もなかった場合のreturn値

get → 空配列  
first → null  

---

## ->each null

```php
// getの結果がnullの場合、eachでエラーになる。
$test = $this->getOpenPlanGDO(553, 111, 111);
$test->each(
    function ($item) {
        \Log::debug($item);
    }
);

// getの結果がnullの場合も想定してeachの前にnull判定が必要。
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

---

## Eloquent SQL出力

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

valueメソッドで済む。  
[いつものところ](https://blog.capilano-fw.com/?p=665#select)  

```php
->select('AdditionPrice')
->first() ?? 0

// データがなければおそらくnull扱いだと思われる。
// 現に [?? 0]はちゃんと機能する。
->value('AdditionPrice') ?? 0
```

---

## コピーと削除

```php
// 削除
// 公開プランGORA金額マスタ
Models\TmOpenPlanGORAPrice::
    where('GolfCode', $golfCode)
    ->where('PlanCode', $planCode)
    ->where('OpenPlanCode', $openPlanCode)
    ->get()
    ->each(
        function ($item) {
            $item->delete();
            Logger::dataChangeLog(Logger::LOG_TYPE_DELETE, $item);
        }
    );
// if ($gora_price->isNotEmpty()) {
//     Models\TmOpenPlanGORAPrice::
//         where('GolfCode', $golfCode)
//         ->where('PlanCode', $planCode)
//         ->where('OpenPlanCode', $openPlanCode)
//         ->delete();
//     Logger::dataChangeLogMultiple(Logger::LOG_TYPE_DELETE, $gora_price);
// }

// コピー
// 公開プランGORA金額マスタ
Models\TmOpenPlanGORAPrice::
    where($base_key)
    ->get()
    ->each(
        function ($master) use ($target_key) {
            $new_master = $master->replicate();
            $new_master->GolfCode = $target_key['GolfCode'];
            $new_master->OpenPlanCode = $target_key['Code'];
            $new_master->PlanCode = $target_key['PlanCode'];
            $new_master->saveWithInsertField();
            Logger::dataChangeLog(Logger::LOG_TYPE_CREATE, $new_master);
        }
    );
// foreach ($master_list as $master) {
//     $new_master = $master->replicate();
//     $new_master->GolfCode = $target_key['GolfCode'];
//     $new_master->OpenPlanCode = $target_key['Code'];
//     $new_master->PlanCode = $target_key['PlanCode'];
//     $new_master->saveWithInsertField();
//     Logger::dataChangeLog(Logger::LOG_TYPE_CREATE, $new_master);
// }
```
