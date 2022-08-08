# out・ref・in

[C# out と ref](https://qiita.com/muro/items/f88b17b5fea3b4537ba7)  

どちらも参照渡しのためのパラメーター修飾子です。  

---

## out

out修飾子はreturn以外でメソッド内からメソッド外へデータを受け渡す場合で使用されます。  
よく使われるものとしてはTryParseメソッドがあります。  

``` C#
    // 呼び出し側
    int val;

    // trueが返りつつ、valは10となる。
    if (OutHoge(out val))
    {
        // C# 7.0から out 引数の利用時に同時に変数宣言できるようになり、あらかじめ変数を宣言しておく必要がなくなりました。
        // if (OutHoge(out int val)) こうできる
        Console.WriteLine(val);
    }

    // 呼び出し先
    bool OutHoge(out int x)
    {
        x = 10;
        return true;
    }
```

---

## ref

ref修飾子はメソッド外からメソッド内へデータを渡し、変更を外部へ反映させる必要がある場合に使用します。  

``` C#
    int x = 0;
    // RefPiyoを抜けた後、xは10になっている。
    RefPiyo(ref x);

    void RefPiyo(ref int x)
    {
        x = 10;
    }
```

### 参照型の参照渡し

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // Ageに10足す
    Hoge(p);
    // 反映される
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=John, Age=30

    void Hoge(Person p) => p.Age += 10;
```

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // Hogeの中でpに新しいインスタンスを代入し、Ageに10足す
    Hoge(p);
    // 反映されない
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=John, Age=20

    void Hoge(Person p)
    {
        p = new Person("Mike", 33);
        p.Age += 10;
    }
```

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // p に新しいインスタンスを代入し、Ageに10足す
    Hoge(ref p);
    // 反映される
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=Mike, Age=43

    void Hoge(ref Person p)
    {
        p = new Person("Mike", 33);
        p.Age += 10;
    }
```

---

## in

一言で言うならば、読み取り専用の参照渡し。  

引数に渡した値がメソッド内で変更されないためには、値渡しを利用していました。  
しかし値渡しは変数をコピーするため、巨大な配列などはコピーに時間がかかるデメリットもありました。  
このinキーワードは、処理の早い参照渡しでありながら読み取り専用であるため、パフォーマンス向上が期待できます。  

C#7.2から利用可能。  

``` C#
    int x = 1;
    // ref 引数と違って修飾不要
    F(x);
    // 明示的に in と付けてもいい
    F(in x);
    // リテラルに対しても呼べる
    F(10);
    // 右辺値(式の計算結果)に対しても呼べる
    int y = 2;
    F(x + y);

    void F(in int x)
    {
        // 読み取り可能
        Console.WriteLine(x);
        // 書き換えようとするとコンパイル エラー
        x = 2;
    }

    // 補足: in 引数はオプションにもできる
    void G(in int x = 1) { }
```
