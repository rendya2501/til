# PHP_クラス

---

## クラスの宣言

- クラスにアクセス修飾子はつけることはできない。  
  - (C#は普通につけられる)  
- コンストラクタはfunction句が必要。  
  - (C#はクラス名だけで良い)  
- クラスのnewに`()`は必要ない。`$a = new class;`  
  - (C#は必要`var a = new class();`)  

``` php
// クラスにアクセス修飾子はつけられない
class Hoge{
    // コンストラクタにはfunction句が必要
    public function __construct(){

    }
}

// クラスの宣言に()は必要ない。
$a = new Hoge;
```

--

## コンストラクタ

`__construct()`  

---

## アクセス修飾子

- public    : クラス内、クラス外のどこからでもアクセス可能  
- private   : 同じクラス内からのみアクセス可能  
- protected : 同じクラス、及び子クラスからアクセス可能  

クラスのメンバーやメソッドに定義することができる。  
クラス自身に定義することはできない。  

---

## メモ

基本的にクラス内部の要素や関数を使いたい場合は`$this->`でアクセスする必要がある。  
双方向リストを実装している時にやらかしたミスをメモっておく。  

``` php
class LinkedList{
    private $dummy;
    
    public function __construct(){
        $this->dummy = new Node(null,null,null);
        // ×
        $this->dummy->next = dummy;
        // ○
        $this->dummy->next = $this->dummy;
    }

    public function InsertBefore(){}

    public function InsertFirst($value):Node{
        // ×
        return InsertBefore($this->dummy,$value);
        // ○
        return $this->InsertBefore($this->dummy,$value);
    }
}
```

---

## オーバーロード

結論からいうと、PHPにオーバーロードはない。  
しかし、それなりに実現の仕方はあるらしい。  

[PHPでのオーバーロード（引数ちがいの同名関数）](https://qiita.com/yasumodev/items/cf3da2a2f5547358e780)  
