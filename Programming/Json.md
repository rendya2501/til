# JSON(JavaScript Object Nation)

[JSONについて調べてみた](https://qiita.com/chihiro/items/dd1912c1406dbfe16b72)  
[JSONとは？データフォーマット（データ形式）について学ぼう！](<https://products.sint.co.jp/topsic/blog/json#:~:text=JSON%E3%81%A8%E3%81%AF%E3%80%8CJavaScript%E3%81%AE,%E3%81%A8%E3%81%A6%E3%82%82%E7%B0%A1%E5%8D%98%E3%81%AB%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82>)  
[[Tips] Newtonsoft.Jsonライブラリの使用方法](https://blog.hiros-dot.net/?p=8766#toc19)  

---

## JSONとは

データ表現記法の1つ。  
JavaScriptのオブジェクトの書き方を元にしたデータ定義方法・データ交換フォーマット  

JavaScript のオブジェクト表記構文のサブセットで、XML と比較すると簡潔に構造化されたデータを記述することができる。  
シンプルな構造のため人間が理解しやすいデータフォーマットとなっています。  
文字コードは UTF-8 固定。  

様々な言語でサポートされている。  
主に各プログラミング言語間のデータの受け渡しに利用される。  

---

## JSONの基本構文

`{}: 中括弧`で囲う。  
`{}`の中に キー(変数)とバリュー(値)のペアを`:`で連結して記述します。  
キーはダブルクォーテーションで囲む。  

``` json : 最もシンプルな例
{"IP":"192.168.1.1"} // OK:正しい書き方
```

``` json : 間違った書き方
{IP:"192.168.1.1"}   // NG:キーをダブルクォーテーションで囲んでいない
{'IP':"192.168.1.1}  // NG:キーをダブルクォーテーションで囲んでいない。シングルクォーテーションは×
{"IP":192.168.1.1}   // NG:192.168.1.1 は数字ではなく文字列扱い
```

## 文字列

文字列はダブルクォーテーションで囲みます。  

``` json
{"name":"takahiro"}  // 文字列データ takahiro をダブルクォーテーション囲む
```

文字列にダブルクォーテーションを含めたい場合はエスケープ処理をする必要がある。  

``` txt
\"      ダブルクォーテーション
\\      バックスラッシュ
\/      スラッシュ
\b      バックスペース
\f      改ページ（フォームフィード）
\n      改行（new line）
\r      復帰（carriage return）
\t      タブ（horizontal tab）
\uXXXX  4桁16進表記のUnicode文字
```

``` json : 文字列中にダブルクォーテーションを含めたい場合
{"name":"\"takahiro\""}  // 文字列の中に「"takahiro"」を入れている
```

## 数値

整数や浮動小数点数、指数表記が使用できます。  

``` json
{"age" : 18, "weight" : 54.3, "exp" : 1.0e-3 }
```

## ヌル

ヌルは「null」で表します。

``` json
{"age" : null}
```

## 真偽値

真偽値は、 true または false で表します。  

``` json
{
  "真":true,
  "偽":false
}
```

## オブジェクト

オブジェクトは`{} : 中括弧`で表します。  
以下の場合は [network] という変数が [IP] と [SUBNET] を持ったオブジェクトを持っていることになります。  

``` json
{
  "network" : {
    "IP":"192.168.1.2",
    "SUBNET":"255.255.255.o0"
  }
}
```

## 配列

値は配列にすると複数持つことができます。  
配列は [] で表し、カンマで区切って複数入れることができます。  
値には文字列、数値、ヌル、真偽値、オブジェクト、配列のすべてを入れることができます。  

``` json
{"fruit":["orange","apple","banana"]}
```

## 全部混ぜたやつ

``` json
{
  "name": "taro",
  "age": 23,
  "sex": "man",
  "hobby": {
    "test" : 12,
    "outdoor": {
      "land": ["running", "walking"],
      "sea": ["swimming", "fising"]
    },
    "indoor": ["movie", "game", "card"],
    "ArrayInObject" : [
      {"obj1" : 1},
      {"obj2" : 2}
    ]
  }
}
```

## JSON のファイル形式

``` txt
エンコーディング  UTF-8
拡張子            .json
MIMEタイプ        application/json
```

※マイム【MIME Multipurpose Internet Mail Extensions】  
SMTPに加えて設定されるメッセージ送信標準のひとつで、Eメールに画像、動画、音声などのファイルを添付できる機能。  
