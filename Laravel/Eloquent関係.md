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
);

// ③
\DB::enableQueryLog();  // ①ロギングを有効化する
$answer->details($id);  // ②クエリを実行する
\DB::getQueryLog(); // ③ログを出力する
```
