# mermaid

---

## mermaid概要

- フローチャートやガントチャート、円グラフなどをMarkdownに描けるオープンソースのライブラリ  
- テキストのみで手軽にきれいなダイアグラムを書くことができるツール  

---

## プレビューサイト

[Mermaid Live Editor](https://mermaid-js.github.io/mermaid-live-editor/)  

---

## mermaid + クラス図

|記号 | 意味                 | サンプル|
|:-|:-|:-|
|<|-- | Generalization(汎化) | ClassA <|-- ClassB|
|<|.. | Realization(実現)    | ClassC <|.. ClassD|
|<--  | Association(関連)    | ClassE <-- ClassF|
|<..  | Dependency(依存)     | ClassG <.. ClassH|
|*--  | Composition(合成)    | ClassI *-- ClassJ|
|o--  | Aggregation(集約)    | ClassK o-- ClassL|
|--   | Link(線)             | ClassM -- ClassN|
|..   | Link(破線)           | ClassO .. ClassP|
|-|-|-|
|*    | Abstract             | foo*, foo()*|
|$    | Static               | foo$, foo()$|
|+    | Public               | +foo, +foo()|
|-    | Private              | -foo, -foo()|
|#    | Protected            | #foo, #foo()|
|~    | Package/Internal     | ~foo|

継承(inheritance) → 汎化(generalization)  
実装(implemente) → 実現(realization)  

頭に`classDiagram`をつける。  
順序を逆にするなら`direction BT`をつける。  

ラベルを付ける場合 : `ClassA <-- ClassB : Label`  
関連の向きを変える場合 : `ClassA --> ClassB`  

変数 : `foo`  
型を指定する場合 : `int foo`  

関数 : `foo()`  
戻り値の型を指定する場合 : `foo(): int`  
引数を指定する場合 : `foo(bar, baz)`  
引数の型を指定する場合 `foo(int bar, String baz)`  

``` mermaid : 例
classDiagram
direction BT

class Hoge{
    -hensuu
    +Method()*
}
```

[mermaidのクラス図メモ](https://zenn.dev/tak_uchida/articles/da583cf960e854)  

---

## 改行

``` mermaid
flowchart
  Node["Hoge\nFuga"]

  API -- "HTTP\n(GET/POST/PUT/DELETE)" --> Controller
```

[Mermaidでフローチャートを描くなら「Mermaid Graphical Editor」！ | astah in 5 min](https://ja.astahblog.com/2022/11/30/mermaid_flowchart_on_vscode/)  

---

## クラス図 パッケージ

いつの間にかパッケージが実装されていた。  

``` mermaid
classDiagram

namespace BaseShapes {
  class Triangle
  class Rectangle
}

namespace Hoge {
  class Huga
}
```

[Implement `package` on class diagram by ksilverwall · Pull Request #4206 · mermaid-js/mermaid](https://github.com/mermaid-js/mermaid/pull/4206)  
[Xユーザーのにゃんだーすわんさん: 「Mermaidのクラス図にpackage(namespace)が実装されてたことを知った。やったぜ！ https://t.co/12Ccz9ttqq」 / X](https://twitter.com/tadsan/status/1669720348391268353)  

---

## 例

``` mermaid
graph TB
  FT[Fivetran]
  HT[Hightouch]
  dbt
  DP[DataPortal]
  S[(Stripe)]
  HS[(HubSpot)]
  GS[Google Sheets]
  DD[(DynamoDB)]
  CS[Census]
  BQ[(BigQuery)]
  HS-->|extract|FT
  S-->|extract|FT
  DD-->|extract|FT
  FT-->|load|BQ
  BQ-->|load|DP
  GS-->|load|DP
  BQ-->CS-->GS
  BQ-->dbt-->|transform|BQ
  BQ-->HT-->|reverse ETL|HS
```

``` mermaid
graph TB
  Start([Start])-->B{if a > b}
  B-->|True| End
  B-->|False| IFS[/while\]
  IFS-->C[a++]
  C-->IFB[\  /]
  IFB-->End([End])
```

---

## 参考

[NotionのMermaid記法](https://twitter.com/paranishian/status/1559657386125668352)  
[【Mermaidの紹介】Qiitaでダイアグラム・チャートが簡単に書けるようになりました！](https://qiita.com/Qiita/items/c07f3262d8f3b25f06c9)  

[公式](https://mermaid-js.github.io/mermaid/#/)  
[公式2](https://mermaid.js.org/intro/)  

1.コメントアウト  
2.括弧を使う  
3.リンクを張る  
4.色を付ける  
[ワンランク上のMermaid(mermaid.js)を書く](https://qiita.com/pitao/items/a860001bae6256dcef1a)  

platumlのサイトだが、多重度の書き方等、mermaidにそのまま移植できるのである程度参考になる。  
[plantuml](https://plantuml.com/ja/class-diagram)  
