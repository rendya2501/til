# PHP_無名関数(クロージャ)

---

## 概要

[公式](https://www.php.net/manual/ja/functions.anonymous.php)  
>無名関数はクロージャとも呼ばれ、 関数名を指定せずに関数を作成できるようにするものです。  
callable パラメータとして使う際に便利ですが、用途はそれにとどまりません。  
無名関数の実装には Closure クラスを使っています。  

---

## 基本的な宣言

最後にコロンを忘れずに。  

``` php
$hoge = function() {
    // 処理
};
```

---

## 無名関数の即時実行

phpは`()`を認識したタイミングで実行する模様。  

``` php
(function(){
    echo "hoge";
})();
// hoge
```

---

## 無名関数のタイプヒンティング

型引数と戻り値の型(タイプヒンティング)を指定可能。  

``` php
$hoge = function(string $str): void {
    print $str;
};
$hoge("aaa");
// aaa
```

---

## use

外部スコープの値を使用する場合 `use`キーワードを使う。  
戻り値のタイプヒンティングはuseの後に可能。  

``` php
$str = "hoge";
$hoge = function() use ($str): void{
    print $str;
};
$hoge();
// hoge
```

useを使わない場合`undefined`扱いになってしまう。  

``` php
$str = "hoge";
$hoge = function(){
    print $str;
};
$hoge();
// PHP Warning:  Undefined variable $str in /workspace/Main.php on line 4
```

## use + 参照渡し

[公式]  
>引き継がれた変数の値は、関数が定義された時点のものであり、関数が呼ばれた時点のものではありません。  

無名関数を定義した後に、値が変わっても反映されない。  

``` php
$str = "hoge";
$hoge = function() use ($str){
    print $str;
};
$str = "huga";
$hoge();
// hoge
```

参照渡しすることで途中で値が変わっても反映されるようになる。  

``` php
$str = "hoge";
$hoge = function() use (&$str){
    print $str;
};
$str = "huga";
$hoge();
// huga
```

---

## 無名関数のコールバック

``` php
function execute(){
    // ①無名関数定義
    $hoge = function(string $str) {
        echo $str;
    };
    // ②hugaにコールバックとして無名関数を渡す
    huga($hoge);
}

// コールバックを受け取る関数
function huga(callable $call_back){
    $str = "huga";
    // ③受け取ったコールバックに対して引数を渡して実行する
    $call_back($str);
}

execute();
```

---

## 参考

[公式](https://www.php.net/manual/ja/functions.anonymous.php)  
[【PHP】無名関数にuse()を使用する時の注意点](https://qiita.com/westhouse_k/items/fe527b59146739cf7af3)  
