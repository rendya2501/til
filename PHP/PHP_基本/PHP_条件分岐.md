# PHP_条件分岐

---

## if

phpのifは括弧で囲む標準タイプ。  
省略形もあり。  
特記すべきことはない。  

``` php
// 例1 : 基本形
if ($a > $b) {
    echo "aはbより大きい";
} else {
    echo "aはbより小さい";
}
```

``` php
// 例2 : 省略形
if ($a > $b) echo "aはbより大きい";

if ($a > $b) 
    echo "aはbより大きい";
```

``` php
// 例3 : else if
if ($aaa) {
    // 処理a
} else if ($bbb) {
    // 処理b
} else {
    // 処理c
}
```

---

## switch

caseの最後コロンはセミコロンでも問題ない。  
phpは ==演算子による緩やかな比較を行う。  

``` php
// 例1 : 基本形
switch ($i){
    case 0:
        echo "iは0に等しい";
        break;
    case 1:
        echo "iは1に等しい";
        break;
    default:
        echo "iは0でも1でもない";
        break;
}
```

``` php
// 例2 : caseの最後がセミコロン & or条件の記述
switch($beer)
{
    case 'ツボルグ';
    case 'カールスバーグ';
    case 'ステラ';
    case 'ハイネケン';
        echo 'いいっすねぇ';
        break;
    default;
        echo 'ほかのを選んでみませんか?';
        break;
}
```

```php
// 例3 : 型の厳密比較をしたい場合等のテクニック
$value = '';
switch (true) {
    case $value === 0:
        echo 'valueは0です';
        break;
    case $value === 1:
        echo 'valueは1です';
        break;
    default:
        echo 'valueは0でも1でも2でもありません';
        break;
}
// valueは0でも1でも2でもありません。
```

``` php
// C#のswitch式っぽいこと
$aaa = (function() use ($variable){
    switch ($variable) {
        case 'value':
            return 1;
        default:
            return 2;
    }
})();
echo $aaa;
// 2
```

[公式](https://www.php.net/manual/ja/control-structures.switch.php)  
