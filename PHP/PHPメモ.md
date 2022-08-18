# PHPメモ

---

## PHP概要

- "PHP: Hypertext Preprocessor"  
- 「PHPはHTMLのプリプロセッサである」とPHP自身を再帰的に説明している  
- HTML埋め込み型の構文（Hypertext Preprocessorたる所以）  
- 強い動的型付け  
- クラスベースオブジェクト指向のサポート  
- 例外処理 (try, catch, throw) のサポート  

- サーバーサイドWebアプリケーション構築のための豊富な組み込み関数  
- データベースへの容易なアクセス（ベンダーごとの組み込み関数、PDO）  
- PECLによる言語機能の拡張  
- 多くのWebサーバへの組み込みの標準サポート  

---

## PHP VSCode 環境構築

大体ここ参考にした。  
XAMPP安定。  

[【2022年版】Visual Studio CodeでPHP開発環境＋デバッグ作業でステップ実行できるようにする方法](https://my-web-note.com/vscode-php-develop-debug/)  
[Visual Studio CodeでPHP開発環境を構築する手順](https://zhuzhuming.com/technology/vscode_phpdebugenv/)  

---

## 書き始め

- `<?php`で書き始め`?>`で閉じる。  
- `?>`は省略可能。  

``` php
<?php
    // 処理
?>
```

---

## PHP6

PHPの参考書を読んでいた時にコラムに書いてあった奴。  
なぜPHP6はないのか。

要約すると、文字コードのUTF-16対応が6のメインだったけど、出来たものがメモリを食い過ぎて使い物にならなくなったのでゴミを化したらしい。  

[独習PHP]  
>PHP5の後、当初はPHP6のリリースが予定されており、実行エンジンの内部処理がUTF-16で全て置き換えられる予定でした。  
しかし、予想以上に大規模な変更になったことから開発は滞り、様々な問題も生じた事から、2010年に結局、開発は断念されることになりました。  
その後、PHP6で予定されていた機能の多くは、PHP5のマイナーバージョンに取りこまれ、PHP6は永久欠番になったのです。  

[PHPは何故バージョン6が存在しないのか？](https://wordpress.ideacompo.com/?p=16906)  
>今から10年以上まえになりますが、2010年に、PHPがバージョン5.3だった時に、PHPバージョン6の開発が精力的に行われていたそうです。  
目玉機能のひとつとして、unicode対応というのがあり、UTF16の正式対応が決まっていたそうです。  
そうした文字エンコード対応を重要視した開発をおこなっていた時に、メモリの使用量が想定したよりも膨らみすぎて、正直使い物にならなくなってしまったようです。  
mbstringをプラグインインストールからするのがめんどくさいので、こういう点も対応してもらいたいんですが、文字コードはインターネットでの黒歴史が多いジャンルですからね。SHIFT-JISとか、EUCとか・・・  
そのため、5.3から6にメジャーバージョンアップするソースが、全てゴミと化してphpのバージョン事態も5.3にロールバックしてしまったという痛ましい事件が起きたらしいです。  
その後、色々な機能追加によるバージョンアップする時に、開発員たちは、封印したバージョンとして6をすっ飛ばして、7を選択したのだそうです。  

---

## cURL error 56: TCP connection reset by peer

内容的にはネットワークの部類に入るのだろうけど、実務でphpで通信処理を書いている時に発生したのでここに書く。  
参考サイト的にはpeerとは相手のことを指すので、この場合、接続相手から接続を切られてしまったことを意味するとか。  
実務で発生した現象だと、管理サイトにはプランが出ていたので、リクエストは受け付けられた模様。  
その後の切断処理がうまくいかなかったと思われる。  
ここはプログラムでどうにかできる問題では無い。  

<https://deep-blog.jp/engineer/5443/>  

---

## PHPのプロセスを止める方法

`ps aux | grep php`  
これでプロセスID（左から２つ目の値）を調べる。  

ルートユーザー、またはルートになれるユーザーでコマンド実行  
`sudo kill -9 (プロセスID)`  

<https://flashbuilder-job.com/php/635.html>

---

## ::class

Laravelでよく見るけど、どういう意味なのかわからなかったのでまとめ。  

- `::class`とは、クラスの完全修飾名を文字列として取得するための記述。  
- PHP5.5から使用可能。  
- PHP8.0以降は、オブジェクト（インスタンス）に対しても `::class` を使って、そのクラスの完全修飾名を取得することができる。  
- 自分がここでそのクラス名を使うとどのクラスを実行することになるのかを調べられる。  
- 名前解決やエイリアスの設定で用いられる。  

[【PHP】::classとは何か？名前解決や完全修飾名の意味や使い方を実例で解説（ダブルコロン２つ クラス、名前空間、最初のバックスラッシュ）](https://prograshi.com/language/php/php-class-namespace/)  

### シンプルな例

名前空間「App\Service」の中の「TestClass」の完全修飾名は「App\Service\TestClass」  
名前空間「App\Service」の中で「echo TestClass::class」を実行すると「App\Service\TestClass」となる。  

``` php
<?php
namespace App\Service{
    class TestClass{
        public static function greeting(){
            echo "Hello!";
        }
    }
    echo TestClass::class;
    // App\Service\TestClass
}
```

### \クラス::class

名前空間の中など、どのファイルからでも同じクラス名を指定するには、冒頭に「\」バックスラッシュをつけ、「\名前空間\クラス名」のように指定する。  

`App\Baa`名前空間で`App\Service\TestClass::class`とすると`App\Bbb\App\Service\TestClass`となってしまう。  

頭に`\`をつけて`\App\Service\TestClass::class`とすることで`App\Service\TestClass`となる。  

``` php
<?php
namespace App\Service{
    class TestClass{
        public static function greeting(){
            echo "Hello!";
        }
    }
}

namespace App\Bbb{
    class TestClass{
        public static function greeting(){
            echo "Hello! Bbb";
        }
    }
    // App\Bbb\App\Service\TestClass
    echo App\Service\TestClass::class."\n";
    // App\Service\TestClass
    echo \App\Service\TestClass::class."\n";
    
    // PHP Fatal error:  Uncaught Error: Class "App\Bbb\App\Service\TestClass" not found in /workspace/Main.php:20
    // echo App\Service\TestClass::class::greeting();

    // Hello! Bbb
    echo TestClass::class::greeting()."\n";
    // Hello!
    echo \App\Service\TestClass::class::greeting()."\n";
}
```

---
