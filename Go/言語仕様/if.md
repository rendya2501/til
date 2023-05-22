# if

Go言語はif文の中で変数を宣言し利用することが出来る。  
スコープはifの内部までであることを確認した。  

``` go
// 変数iを宣言し、1を代入する。
// i が 1 であれば hoge を表示する
if i := 1; i == 1 {
    fmt.Println("hoge")
    fmt.Println(i)
    // hoge
    // 1
} else {
    fmt.Println("fuga")
}

// undefind
fmt.Println(i)
```

[Go言語 入門【環境構築とコーディング】 - RAKUS Developers Blog | ラクス エンジニアブログ](https://tech-blog.rakus.co.jp/entry/20211015/go)  
