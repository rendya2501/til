# foreach

---

## 概要

反復可能な値に対してループ処理を行う  

- 2つの構文がある  
- 配列とオブジェクトに対してだけ使用可能。それ以外はエラーとなる。  
- 文字列には使えない。  
  - PHPの文字列はC#と違い、Char型の配列という扱いではないのでエラーになる。  
  - 余談だが、PHPにCHAR型は存在しない。String型のみである  

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

``` php
$arr = [1,2,3,4];
foreach ($arr as $key => $value) {
    echo "{$key} => {$value}\n";
}
```

---

## foreachで作った変数はforeach抜けた後も有効

割と罠。  
公式のリファレンスにも書いてありました。  
unsetはマナー的にあってもいいのかもしれないですね。  

---

## 空配列

空配列をforeachに渡した場合、foreachの内部処理はされずに、foreachを抜ける。  
エラーにはならない。  

``` php
    // $item = []だとエラーにならず、処理されずに終わる。
    foreach($item as $value){
    }
```

---

## invalid argument supplied for foreach()

[Warning: Invalid argument supplied for foreach() とでたら。。。](https://hacknote.jp/archives/19783/)  

supplied : 供給  
違法な引数をforeachに供給した  

何をしたらforeachでこのようなエラーが出るのか答えられなかったのでまとめ。  
PaizaIOで確かめてもこのエラーにならなかった。  

foreachにnullとかを渡せばエラーになるのはわかったけど、なぜエラー内容が違うのかわからないままだ。  

``` php
<?php
    $str = "aaaa";
    foreach ($str as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, string given in /workspace/Main.php on line 4

    $null = null;
    foreach ($null as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, null given in /workspace/Main.php on line 14

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
