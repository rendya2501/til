# Eloquent SQL出力

①

```PHP
\Log::debug($query->toSql());
\Log::debug($query->getBindings());
```

②

```PHP
\Log::debug(
    vsprintf(
        str_replace('?', '%s', str_replace('?', "'?'", $query->toSql())),
        $query->getBindings()
    )
);
```

③

```PHP
\DB::enableQueryLog();  // ①ロギングを有効化する
$answer->details($id);  // ②クエリを実行する
dd(\DB::getQueryLog()); // ③ログを出力する
```
