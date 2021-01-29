# php色々

## 配列をカンマ区切りの文字列に変換する方法

implode関数を使用する。  
ちなみにLaravelのEroquent使ってcollectionをカンマ区切りの文字列にするサンプルです。

```php
$some_list = Models\TableModel::
    where()
    // pluckは指定したキーの値だけを取得できる。
    ->pluck('FiledName')
    ->toArray();
$str = implode(",", $some_list);
```

## 連想配列をforeachするときにインデックスも欲しい

```php
$array = array( 
    'cat' => 'meow', 
    'dog' => 'woof', 
    'cow' => 'moo', 
    'computer' => 'beep' 
);

foreach( array_keys( $array ) as $index => $key ) {
    // display the current index + key + value
    echo 'index:' . $index . ' key:' . $key . ' value:' . $array[$key] . "\n";
}
// 結果
// index:0 key:cat value:meow
// index:1 key:dog value:woof
// index:2 key:cow value:moo
// index:3 key:computer value:beep
```

## 多重配列への追加

```php
$params['plans'][] = ['cat' => 'meow', 'dog' => 'woof'];
$params['plans'][] = ['cow' => 'moo','computer' => 'beep' ];
print_r($params);
print(json_encode($params));
// 結果
// {"plans":[{"cat":"meow","dog":"woof"},{"cow":"moo","computer":"beep"}]}
```

## PHPのforeachで作った変数はforeach抜けた後も有効

公式のリファレンスにも書いてありました。  
unsetはマナー的にあってもいいのかもしれないですね。  

---

## 多次元配列をimplode()する最も簡単な方法

全部結果は同じ。でも案3が一番おすすめ。短くてわかりやすい。  

```php
$params = array (
  'plans' => 
  array (
    0 => 
    array (
      'linkage_plan_id' => '05539999950000020001',
      'is_searchable' => false,
    ),
    1 => 
    array (
      'linkage_plan_id' => '05539999950000020002',
      'is_searchable' => false,
    ),
  ),
);
// 案1
print(
    implode(
        ",",
        array_map(
            fn($el) => $el['linkage_plan_id'], 
            $params['plans']
        )
    )
);
// 案2
print(
    implode(
        ',',
        array_map(
            'implode',
            $params['plans'],
            array_fill(0, count($params['plans']),
            '')
        )
    )
);
// 案3
print(
    implode(
        ',',
        array_column($params['plans'], 'linkage_plan_id')
    )
);
```
