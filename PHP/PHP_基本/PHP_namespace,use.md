# PHP_namespace,use

---

## namespace

- PHP 5.3で導入。  
- 同じ名前の名前空間は定義できない。  
- 名前空間の定義は一番最初で行う。  
  - require_onceより後ではダメ。  
- 1つのファイルの中に複数の名前空間を定義できる。  
  - しかし、そんなことはしないだろう。  
  - この場合の定義方法は2パターン存在する。
  - 名前空間を中括弧`{}`で囲った場合、その範囲外にコードを書くことはできない。  
- 複数のファイルで同じ名前空間を定義することができる。  
  - しかし、同じ名前空間になるので同名の関数は定義できない。
    →サブ名前空間を使えばよい
- サブ名前空間とはディレクトリのように名前空間を`\`で区切って記述した名前空間のこと。  

``` php : 名前空間を定義する構文
namespace 名前空間名;

// 例
<?php
namespace test;
```

``` php : 名前空間に属する関数を利用する構文
名前空間名\関数名;
```

``` php : 名前空間に属するクラスを利用する構文
名前空間名\クラス名;
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

---

[【PHP超入門】名前空間（namespace・use）について](https://qiita.com/7968/items/1e5c61128fa495358c1f)  
