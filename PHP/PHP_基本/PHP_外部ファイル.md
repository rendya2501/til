# PHP_外部ファイル読み込み

---

## 読み込み命令一覧

- require  
- require_once  
- include  
- include_once  
- autoload  
- spl_autoload_register  

---

## require

require でエラーが発生した場合、致命的なエラー(Fatal Error)となり、その後の処理が停止する。  
読み込むことが必須となるファイルであったり、その後の処理に影響を与える処理が書かれているファイルを読み込む場合は、requireでファイルを読み込む。  

``` php
<?php
require('test.php');
//or
require 'test.php';

echo $name;//佐藤
```

``` php : test.php
<?php
$name = '佐藤';
```

---

## include

include でエラーが発生した場合は、警告(Warning)を出力しますが、その後の処理は継続される。  
画像ファイルを読み込むなど、後続の処理に問題が発生しないファイルを読み込む場合はincludeでファイルを読み込む。  

``` php
<?php
include('test.php');
//or
include 'test.php';

echo $name;//佐藤
```

``` php : test.php
<?php
$name = '佐藤';
```

---

## once の有無による違い

require_once文,include_once文 は、一度だけrequire (include) する。  
ファイルがすでに読み込まれている場合は、再読み込みをしない。  

読み込むファイルの内容によっては変数に値が再代入され上書きされてしまったり、関数の再定義などが起こることによってエラーの原因となってしまうことがあります。（関数は再定義が不可能。）  
このような問題が起こることを未然に防ぎたい場合に、require_once文 (include_once文)を使って一度のみ読み込ませ、再度読み込ませないようにすることができます。  

``` php : external_file.php
<?php
echo "Hello World!";
```

``` php
<?php 
echo "1回目\n";
// 外部ファイル external_file.php はまだ読み込まれていないので読み込む
require_once("external_file.php");
echo "\n";
echo "2回目\n";
// 先程読み込まれたので、今回は読み込まれない
require_once("external_file.php");
echo "\n";
echo "3回目\n";
// 先程読み込まれたので、今回は読み込まれない
require_once("external_file.php");

// 1回目
// Hello World!
// 2回目
// 
// 3回目
```

---

## autoload

通常は`require_once`等でファイルを明示的に読み込む必要があるが、これを自動的に行ってくれる仕組み。  
オートロードを使用するためにはcomposerが必要。  
composerによって生成された`composer.json`に登録したファイルを読み込む。  

詳しい設定の仕方はここらへんを参考にされたし。  
[PHPのオートロード(autoload)](https://qiita.com/atwata/items/5ba72d3d881a81227c2a)  
[(PHP) autoload(オートロード)とは – composerを理解する –](https://hara-chan.com/it/programming/php-autoload-composer/)  

---

## spl_autoload_register

未定義のクラスを使うとエラーになる。  
そのエラーとなる前に、最後の救済メソッドとして登録しておいたメソッドを実行する。  
それがこの`spl_autoload_register`であり、未定義のクラスを探しにいく処理を書いておくことで、エラーを回避する。  

autoloadが使える環境ならそちらを使うのがメジャーっぽい。  
外部ファイルの読み込みはjsonで登録できるようにして、読み込んだほうがいいに決まっている。  
コードに書くものでもあるまい。  
`spl_autoload_register`はそのファイルに直接記述するものではなく、ある程度決まったクラスや書き方があるので、それをUtilみたいに裏で定義して動かすモノっぽい。  

### シンプルなサンプル

``` php
<?php
spl_autoload_register(function($class) {
    echo "Want to load $class.\n";
    require __DIR__ . "/${class}.php";
});
$foo = new Foo();
echo $foo->name;

// Want to load Foo.
// 佐藤
```

``` php : Foo.php
<?php
class Foo {
    public $name = '佐藤';
}
```

### Util的なサンプル

こちらのソースを使って検証してみたが、確かに動いた。  
[PHP で、spl_autoload_register を使って、require_once 地獄を脱出しよう](https://qiita.com/misogi@github/items/8d02f2eac9a91b4e6215)  

``` php
<?php
/**
 * Classが定義されていない場合に、ファイルを探すクラス
 */
class ClassLoader
{
    // class ファイルがあるディレクトリのリスト
    private static $dirs;

    /**
     * クラスが見つからなかった場合呼び出されるメソッド
     * spl_autoload_register でこのメソッドを登録してください
     * @param  string $class 名前空間など含んだクラス名
     * @return bool 成功すればtrue
     */
    public static function loadClass($class)
    {
        foreach (self::directories() as $directory) {
            // 名前空間や疑似名前空間をここでパースして
            // 適切なファイルパスにしてください
            $file_name = "{$directory}/{$class}.php";

            if (is_file($file_name)) {
                require $file_name;

                return true;
            }
        }
    }

    /**
     * ディレクトリリスト
     * @return array フルパスのリスト
     */
    private static function directories()
    {
        if (empty(self::$dirs)) {
            $base = __DIR__;
            self::$dirs = array(
                // ここに読み込んでほしいディレクトリを足していきます
                $base . '/controllers',
                $base . '/models',
                $base
            );
        }
        return self::$dirs;
    }
}

// これを実行しないとオートローダーとして動かない
spl_autoload_register(array('ClassLoader', 'loadClass'));
```

``` php
<?php
require_once 'ClassLoader.php';

$foo = new Foo();
echo $foo->name;
// 佐藤
```

``` php : Foo.php
<?php
class Foo {
    public $name = '佐藤';
}
```

---

## 参考リンク

[【PHP基礎】外部ファイルの読み込みとは（required, include)](https://tech.pjin.jp/blog/2020/12/30/php5_1_1/)  
[PHPで外部ファイル](https://qiita.com/nogson/items/db15d6e35433154fce8e)  
[PHPのオートロード(autoload)](https://qiita.com/atwata/items/5ba72d3d881a81227c2a)  
[PHP で、spl_autoload_register を使って、require_once 地獄を脱出しよう](https://qiita.com/misogi@github/items/8d02f2eac9a91b4e6215)  
