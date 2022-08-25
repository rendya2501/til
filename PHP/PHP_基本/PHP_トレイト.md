# PHP_トレイト

---

## トレイト概要

継承とはまた別の方法でクラスを拡張できる仕組み。  
単一継承言語でコードを再利用するための仕組み。  
クラスという単位とは別の単位で機能をまとめて再利用ができるようにする仕組み。
継承が垂直方向の拡張ならトレイトは水平方向の拡張。  

- PHP5.4.0から導入された。
- public、protected、privateなどのアクセス修飾子を利用可能。  
- C#ではインターフェースのデフォルト実装で同じようなことが可能。  

---

## 手っ取り早くトレイトを使う

``` php
// トレイトの宣言
trait Hoge_Trait{
    // トレイトのプロパティ
    public $Hoge_prop = "Hoge_prop\n";
    
    // 関数の定義
    public function CallHoge(){
        echo "Hoge\n";
    }
}

class Fuga{
    // 使用するトレイトを宣言
    use Hoge_Trait;
    
    public function CallHogeProp(){
        // トレイトへのアクセスは$this
        echo $this->Hoge_prop;
        // トレイトのプロパティは書き換え可能
        $this->Hoge_prop = 2222;
    }
}

$fuga = new Fuga;
// トレイトの関数を実行
$fuga->CallHogeProp();
// トレイトのプロパティを表示
echo $fuga->Hoge_prop;

// Hoge_prop
// 2222
```

---

## 使用方法

``` php : traitの作成方法
trait トレイト名 {
    アクセス修飾子 $メンバ名 = 値;

    アクセス修飾子 function メソッド名() {
        処理
    }
}
```

``` php : class内での使用方法
class クラス名 {

use トレイト名;

// 以降class処理

}
```

---

## 参考リンク

[単一継承言語の救世主？？ PHPのtraitについて解説！](https://beyondjapan.com/blog/2021/10/php-trait/)  
[クラスを水平方向に拡張できるPHPの「トレイト」](https://atmarkit.itmedia.co.jp/ait/articles/1806/27/news008.html)  
[Laravelから学ぶPHPのTraitの使い所](https://qiita.com/kazuhei/items/dd4e275c03eb6916f522)
