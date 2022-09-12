# PHP_配列関数

---

## array_shift

array の最初の値を取り出して返します。  
配列 array は、要素一つ分だけ短くなり、全ての要素は前にずれます。  
数値添字の配列のキーはゼロから順に新たに振りなおされる。  
リテラルのキーはそのまま。  

``` php
<?php
$stack = array("orange", "banana", "apple", "raspberry");
$fruit = array_shift($stack);
var_dump($fruit);
// orange
print_r($stack);
// Array
// (
//     [0] => banana
//     [1] => apple
//     [2] => raspberry
// )
```

---

## array_splice

配列の一部を削除したり、他の要素で置換できる関数  

``` php : 書式
array_splice ( array &$input , int $offset [, int $length = count($input) [, mixed $replacement = array() ]] ) : array

&$input      // 操作対象の配列
$offset      // 要素の抽出開始位置(インデックス指定)
$length      // 取り出す要素数
$replacement // 削除個所に挿入する配列
```

``` php : 指定した範囲を削除
<?php
//             0          1         2        3
$fruits = [ "りんご", "みかん", "れもん", "メロン" ];

// 削除された要素を出力
// "れもん", "メロン"
print_r(array_splice($fruits, 2, 2));

// 削除後の配列を出力
// "りんご", "みかん"
print_r($fruits);
```

``` php : 指定した範囲を配列で置き換える
<?php

$fruits = [ "りんご", "みかん",  "れもん", "メロン" ];
$addFruiuts = [ "ぶどう", "もも" ];

// 配列の要素を置換して、削除された要素を出力
// "れもん", "メロン"
print_r(array_splice($fruits, 2, 2, $addFruiuts));

// 置換後の配列を出力
// [0] => りんご,[1] => みかん,[2] => ぶどう,[3] => もも
print_r($fruits);
```

[PHPのarray_splice関数の挙動を確認する](https://qiita.com/a05kk/items/25fef3a1ab1ec673438a)  
