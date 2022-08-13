# PHP_関数

---

## 関数の基本

- `function`で宣言する。  

シンプルな関数の定義

``` php
function 関数名 () {
    // 処理
}
```

PHP 7.0 から引数や戻り値の型宣言が可能。  

``` php
function 関数名 (引数の型 $引数): 関数の戻り値 {
    // 処理
    return $戻り値;
}
```

---

## デフォルト引数

PHP 7.0 から使用可能。  

``` php
function hoge(string $str = null) {
    var_dump($str);
}
```

---

## null許容引数

型名の前に ? を追加することで NULL 許容型として宣言できる。  
PHP 7.1 から使用可能。  

``` php
function hoge(?string $str) {
    var_dump($str);
}
```

[【PHP】引数の二種類の NULL 許容型の宣言方法とその違い](https://qiita.com/ymm1x/items/e53ded283080ca3a42b4)

---

## void

PHP 7.1からvoid宣言が可能。  
戻り値がないことを明記できるようになった。  

``` php
function hoge(): void {
}
```

あくまで戻り値がないことを宣言するだけ。  
何も返さないreturnはエラーにならず、自動的にvoidになる。  
voidを変数受けしてもエラーにはならない。中身はNULLになる。  
戻り値の型がない or return だけならvoid宣言した時と同じ状態になる模様。  

``` php
function hoge_void(string $str): void{
}

function hoge_return(string $str){
    return;
}

function hoge(string $str){
}

$hoge_void = hoge_void("aaa");
$hoge_return = hoge_return("aaa");
$hoge = hoge("aaa");

var_dump(hoge_void("aaa"));   // NULL
var_dump(hoge_return("aaa")); // NULL
var_dump(hoge("aaa"));        // NULL
var_dump($hoge_void);   // NULL
var_dump($hoge_return); // NULL
var_dump($hoge);        // NULL
```

void宣言しているのに、戻り値を返すとエラーになる。

``` php
function hoge_void(): void{
    // PHP Fatal error:  A void function must not return a value in /workspace/Main.php on line 21
    return "hoge";
}
var_dump(hoge_void());
```

---

## 型宣言 (タイプヒンティング)

関数の宣言時、関数が受け取る引数の型を指定して、指定型以外のデータが渡された場合にエラーが発生する機能。  

指定できる型一覧

- 文字列型  
- 整数型  
- 浮動小数点数型  
- 配列型  
- 論理型  
- Callable（コールバック）  
- クラス  
- インターフェース  

[PHP の関数 ( function ) について 【初心者向け】](https://wepicks.net/phpref-functions_userdefined/)  

---

## 関数内関数(ローカル関数)

[公式]
>PHP では、関数やクラスはすべてグローバルスコープにあります。  
関数の内部で定義したものであっても関数の外部からコールできますし、その逆も可能です。  

PHPのローカル関数は恐ろしい。  
スコープがグローバルにまで広がってしまう。  
先に定義しておかないと使えない。  
変数は隠蔽されるのに変数以外(関数、クラス、インターフェース、トレイト)はグローバル公開される。謎。  

C#程柔軟じゃないので使う意味はないのでは？  

``` php
function foo() 
{
    function bar() 
    {
        echo "I don't exist until foo() is called.\n";
    }
    // 関数内部で呼び出すなら定義した後でないとダメ。  
    bar();
}
// ここでは関数bar()はまだ定義されていないのでコールすることはできません。
// bar();

foo();
// foo()の実行によって bar()が定義されるためここではbar()をコールすることができます。
bar();
```

クロージャーにすると`foo`内だけのスコープになる。  

``` php
function foo() 
{
    $bar = function() 
    {
        echo "I don't exist until foo() is called.\n";
    };
    $bar();
}
foo();
//bar();
```

- 参考  
  - [公式](https://www.php.net/manual/ja/functions.user-defined.php)  
  - [PHPでは関数内に関数を書けるしグローバル関数になる](https://qiita.com/rana_kualu/items/f569524e5727572f6780)  

---

## 可変関数

変数名の後ろに`()`を付与することで、変数の値と同名の関数を呼び出すことができる機能。  

``` php
function hoge(){
    echo 'Hello!';
}

$func = 'hoge';
$func();
// Hello!


// これは駄目
// 'hoge'();
```

---

## コールバック

phpのコールバックは3種類ある。  

- 可変関数を使う方法  
- call_user_func関数を使う方法  
- 無名関数を使う方法  

無名関数がおすすめ。  

call_user_func関数は文字列で指定するのでなんかいやだ。  
関数名を文字列に変換するしてくれるnameofみたいなのがあればいいのだが、軽く調べてみたけど、なさそうですね。  
現在実行している関数名を取得する方法はあるけど、クラスの中のこの関数を文字列に変換するみたいな処理はないっぽい。  

[いい感じのコールバックサンプル](https://qiita.com/dublog/items/0eb8bcea2fc452c0b4b2)  
[PHPでコールバック関数を利用する](https://qiita.com/tricogimmick/items/23fb5958b6ea914bbfb5)  
[(PHP) コールバック関数とは？使い方を分かりやすく解説](https://hara-chan.com/it/programming/php-callback/)  

### 可変関数のコールバック

``` php
// ①コールバック関数定義
function callback_func()
{
    return "foo";
}

// ②コールバック関数を受け取る関数を定義
function execute($call_back)
{
    // ④ 可変関数を利用して文字列で渡された名称の関数を呼び出す
    // 変数の後ろに()を付けてコール
    echo "callback function result : {$call_back()}\n";
}

// ③関数にコールバック関数を渡す
execute("callback_func");

```

### 無名関数のコールバック

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

### call_user_func() 関数を利用したコールバック

可変関数を利用したコールバック関数ではクラスやオブジェクトのメソッドをコールバック関数として渡すことができない。  
それを可能にするのが`call_user_func()関数`。  

``` php
class SampleClass
{
    // 普通のメソッド
    function callbackMethod()
    {
        return "hoge";
    }
    // 静的メソッド
    static function staticCallbackMethod()
    {
        return "fuga";
    }
}

// コールバック関数を受け取る関数
function execute($call_back)
{
    echo "callback function result : {call_user_func($call_back)}";
}

# 普通のクラスメソッドをコール
$obj = new SampleClass();
execute(array($obj, "callbackMethod"));// hoge

# 静的メソッドをコール
execute(array("SampleClass", "staticCallbackMethod")); // fuga
```
