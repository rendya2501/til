# ジェネリック

---

## ジェネリック制約とは

[【c#】ジェネリック制約まとめ](https://qiita.com/daria_sieben/items/aa28a014656c9a0990ed)  

ジェネリックを使う際に宣言の後ろに

`[where ジェネリックで使う型 : 制限したい型]`

をつけることで使える型を制限出来ることです。  
こうすることによってこのクラスでしか使えないようにしたいとか、このクラスでは使ってほしくないなどの使い方が出来るようになります。  

``` C# : ジェネリックの対象をクラスのみにしたい場合
public class MyClass<T> where T : class
{
}
```

``` txt :  制約一覧
ジェネリック制約                説明
where T : class                 class(参照型)のみ制約
where T : struct                Nullableを除く全ての値型のみ制約
where T : <クラス名>            指定したクラスのみで制約
where T : <インターフェース名>  指定したインターフェースのみで制約
where T : new()                 引数なしのパブリックコンストラクタがある型のみで制約
where T : U                     Uに基づいた型で制約される
```

---

## ジェネリックにnullを指定する方法

azmさんに質問された内容。ジェネリックは弱いのでまとめる。  

Javaの時にやったジェネリックに基づけば、ArrayList等においてジェネリックは入れる型を限定し、取り出すときはキャストいなくていい仕組みであったはず。  
あらかじめ実行する処理は定義しておいて、型はその時々で柔軟に切り替える仕組みがジェネリック。  
なので、絶対に型は必要。  
しかし、nullは型がない。  
いつだかis演算子でnullを判定した場合、nullには型がないので常にfalseになる、ってまとめた気がする。  
型が必要なのに型がないのでそもそも無理な話なのだ。  
結局、Objectを指定することで解決したらしい。  

C#8.0からではあるが、null許容型が追加されたらしい。  
Framerwork4.8の7.3どまりのフロントではもちろんエラーになる。  

``` C#
    static void Class<T>() where T : class { }
    static void NullableClass<T>() where T : class? { }
    
    static void Main()
    {
        Class<string>();
        // C#8.0では警告みたいなのが出るが普通に行けることを確認した。
        NullableClass<string?>();
        // 結局渡したい物が何もないからnullにしたいだけであって、それならということでobjectを渡して何とかしたみたい。
        Class<object>();
    }
```
