# markdown_mermaid

---

## mermaid概要

- フローチャートやガントチャート、円グラフなどをMarkdownに描けるオープンソースのライブラリ  
- テキストのみで手軽にきれいなダイアグラムを書くことができるツール  

---

## VSCode + marmaid

拡張機能 : Markdown Preview Mermaid Support をインストール  

[VISUAL STUDIO CODE で MERMAID をプレビューする方法](https://usefuledge.com/vscodemermaidsupport.html)  

---

## mermaid + クラス図

``` txt
記号 | 意味               | サンプル
-----+--------------------+----------------------
<|-- | Inheritance(継承)  | ClassA <|-- ClassB
<|.. | Realization(実現)  | ClassC <|.. ClassD
<--  | Association(関連)  | ClassE <-- ClassF
<..  | Dependency(依存)   | ClassG <.. ClassH
*--  | Composition(委譲)  | ClassI *-- ClassJ
o--  | Aggregation(集約)  | ClassK o-- ClassL
--   | Link(線)           | ClassM -- ClassN
..   | Link(破線)         | ClassO .. ClassP
-----+--------------------+-----------------------
*    | Abstract           | foo*, foo()*
$    | Static             | foo$, foo()$
+    | Public             | +foo, +foo()
-    | Private            | -foo, -foo()
#    | Protected          | #foo, #foo()
~    | Package/Internal   | ~foo
```

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
