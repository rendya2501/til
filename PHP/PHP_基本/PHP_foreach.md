# PHP_foreach

---

## 概要

反復可能な値に対してループ処理を行う  

- 2種類の構文がある`(iterable_expression as $value)`,`(iterable_expression as $key => $value)`  
- 配列とオブジェクトに対してだけ使用可能。それ以外はエラーとなる。  
- 文字列には使えない。  
  - PHPの文字列はC#と違い、Char型の配列という扱いではないのでエラーになる。  
  - 余談だが、PHPにCHAR型は存在しない。String型のみである  
- ループの中で配列の要素を直接変更したい場合は、 $value の前に & をつける  
- foreach ループを終えた後でも、 $value は配列の最後の要素を参照したままとなるのでunset() でその参照を解除しておく必要がある  

[【VB.NETプログラマーから見たPHP】foreachについて](https://vowlog.com/644/)  

---

## foreach (iterable_expression as $value)

- iterable_expression で指定した反復可能な値に 関してループ処理を行う  
- 反復において現在の要素の値が $valueに代入される  

``` php
$arr = [1,2,3,4];
foreach ($arr as $value) {
    $value = $value * 2;
    echo $value."\n";
}
// 2
// 4
// 6
// 8
```

---

## foreach (iterable_expression as $key => $value)

`key => value`形式では、インデックスを取得可能。  

``` php
$arr = [1,2,3,4];
foreach ($arr as $key => $value) {
    echo "{$key} => {$value}\n";
}
// 0 => 1
// 1 => 2
// 2 => 3
// 3 => 4
```

---

## 連想配列 + foreach

連想配列の場合、keyはインデックスではなく、キーの内容がそのまま表示される。

``` php
<?php
$arr = ["a"=>1,"b"=>2,"c"=>3,"d"=>4];
foreach($arr as $value){
    echo "{$value}\n";
}
// 1
// 2
// 3
// 4

foreach($arr as $key => $value){
    echo "{$key} => {$value}\n";
}
// a => 1
// b => 2
// c => 3
// d => 4
```

---

## 値渡し

foreach中でvalueを2倍にしているが、var_dumpしたarrの値は変わっていない。  
これは値渡しの状態なので、元のarrの状態に変化はない。  
foreachの結果を元の配列等にも適応させたい場合、参照渡しを行う必要がある。  

``` php
$arr = array(1, 2, 3, 4);
foreach ($arr as $value) {
    $value = $value * 2;
    echo $value."\n";
}
var_dump($arr);
// 2 4 6 8
// array(4) { [0]=> int(1) [1]=> int(2) [2]=> int(3) [3]=> int(4) }
```

## 参照渡し

ループの中で配列の要素を直接変更したい場合は、 $value の前に `&` をつける。  
変数の先頭に&をつけることで参照しとなり、$arrの値を算出した結果にする事が出来る。  

``` php
$arr = array(1, 2, 3, 4);
             // ↓&
foreach ($arr as &$value) {
    $value = $value * 2;
    echo $value."\n";
}
var_dump($arr);
// 2 4 6 8
// array(4) { [0]=> int(2) [1]=> int(4) [2]=> int(6) [3]=> &int(8) }
                                                       // ↑参照になっている
```

---

## foreachで作った変数はforeach抜けた後も有効

- foreach ループを終えた後でも、 \$value は配列の最後の要素を参照したままとなる  
- 意図しない結果や事故を防ぐために `unset()` でその参照を解除しておく必要がある  

``` php
$arr = array(1, 2, 3, 4, 5);
foreach ($arr as &$value) {
    $value = $value * 2;
    echo $value."\n";
}
// unsetで参照を解除
unset($value);
echo "---\n";
foreach ($arr as $value) {
    $value = $value * 2;
    echo $value."\n";
}
var_dump($arr);
// 2 4 6 8 10
// —
// 4 8 12 16 20
// array(4) { [0]=> int(2) [1]=> int(4) [2]=> int(6) [3]=> int(8) [4]=> int(10)}

// unsetがない場合
// 2 4 6 8 10
// —
// 4 8 12 16 32
// array(4) { [0]=> int(2) [1]=> int(4) [2]=> int(6) [3]=> int(8) [4]=> &int(32) }
// 最後の要素に16が来て、それを2倍するので32になっている
```

割と罠。  
公式のリファレンスにも書いてありました。  
unsetはマナー的にあってもいいのかもしれないですね。  

---

## 空配列

空配列をforeachに渡した場合、foreachの内部処理はされずに、foreachを抜ける。  
エラーにはならない。  

``` php
// $arr = []だとエラーにならず、処理されずに終わる。
foreach($arr as $value){
}
```

---

## invalid argument supplied for foreach()

supplied : 供給  
違法な引数をforeachに供給した  

何をしたらforeachでこのようなエラーが出るのか答えられなかったのでまとめ。  
PaizaIOで確かめてもこのエラーにならなかった。  

foreachにnullとかを渡せばエラーになるのはわかったけど、なぜエラー内容が違うのかわからないままだ。  

[Warning: Invalid argument supplied for foreach() とでたら。。。](https://hacknote.jp/archives/19783/)  

``` php
<?php
    // 文字列にforeachは使えない
    $str = "aaaa";
    foreach ($str as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, string given in /workspace/Main.php on line 4

    // foreachにnullを渡せばもちろんエラー
    $null = null;
    foreach ($null as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, null given in /workspace/Main.php on line 14

    // 配列とオブジェクト以外はダメなのでもちろんエラー
    $num = 1;
    foreach ($num as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, int given in /workspace/Main.php on line 19
```

関数に渡したらエラーになるかと思ったけど、ならなかった。最後まで謎だった。  

``` php
function aa($str) {
    foreach ($str as $a){
        echo $a;
    }
}
$str = null;
aa($str);
```
