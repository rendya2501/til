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

## mermaid + クラス図

[mermaidのクラス図メモ](https://zenn.dev/tak_uchida/articles/da583cf960e854)  

---

## 参考

[NotionのMermaid記法](https://twitter.com/paranishian/status/1559657386125668352)  
[【Mermaidの紹介】Qiitaでダイアグラム・チャートが簡単に書けるようになりました！](https://qiita.com/Qiita/items/c07f3262d8f3b25f06c9)  
[公式](https://mermaid-js.github.io/mermaid/#/)  
