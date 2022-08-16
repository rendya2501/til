# PHP_namespace,use

---

## namespace

- PHP 5.3で導入。  
- 同じ名前の名前空間は定義できない。  
- 名前空間の定義は一番最初で行う。  
  - require_onceより後ではダメ。  
- 1つのファイルの中に複数の名前空間を定義できる。  
  - この場合の定義方法は2パターン存在する。  
  - 名前空間を中括弧`{}`で囲った場合、その範囲外にコードを書くことはできない。  
- 複数のファイルで同じ名前空間を定義することができる。  
  - しかし、同じ名前空間になるので同名の関数は定義できない。  
    →サブ名前空間を使えばよい  
- サブ名前空間とはディレクトリのように名前空間を`\`で区切って記述した名前空間のこと。  

``` php
namespace test {
    class Hoge{
        public function __construct(){
            echo "Hoge\n";
        }
    }
    function Hoge_Func(){
        echo "Hoge_Func\n";
    }
}
namespace {
    $hoge = new \test\Hoge; // Hoge
    \test\Hoge_Func();      // Hoge_Func
}
```

``` php : 1つのファイルに複数の名前空間を定義するパターン1
namespace hoge;
function getGreeting() {
    return 'hoge';
}
// namespace hugaの始まりまではhoge名前空間
// 始まりから先はhuga名前空間
namespace huga;
function getGreeting() {
    return 'huga';
}
```

``` php : 1つのファイルに複数の名前空間を定義するパターン2
namespace hoge{
    function getGreeting() {
        return 'hoge';
    }
}
namespace huga{
    function getGreeting() {
        return 'huga';
    }
}
```

---

## use

- 名前空間のインポート,インポートのエイリアスに用いる。  
- インポートの前にファイルのrequireは別途必要。  

- 名前空間などのエイリアス（別名）を作成。  
- 名前空間の全て、または一部をインポート。  
- クラスをインポート。  
- 関数をインポート。  
- 定数をインポート。  

実務で使っていた例はuseでクラスをインポートする使い方だった。  
1ファイル,1名前空間,1クラスだから困ることがなかったんだ。  

``` php : tokyo.php
// サブ名前空間
namespace asia\japan\kanto\tokyo;

class pref {
    public function getPref() {
        return "tokyo\n";
    }
}
```

``` php
require_once 'tokyo.php';

// エイリアス
use asia\japan\kanto as area;
$a = new area\tokyo\pref;
echo $a->getPref(); // tokyo

// サブ名前空間の一部をインポート
use asia\japan\kanto;
$b = new kanto\tokyo\pref;
echo $b->getPref(); // tokyo

// クラスのインポート
use asia\japan\kanto\tokyo\pref;
$c = new pref;
echo $c->getPref(); // tokyo
```

---

[【PHP超入門】名前空間（namespace・use）について](https://qiita.com/7968/items/1e5c61128fa495358c1f)  
