# マークダウンメモ

[Markdown記法 チートシート](https://qiita.com/Qiita/items/c686397e4a0f4f11683d)

---

## markdown open preview to the side

Markdown All in One  
markdownlint  

---

## Excelの表をMarkdownで表現したい

- VSCode拡張「Excel to Markdown table」をインストール。  
- エクセルからコピーして来てマークダウンファイルに貼り付ける。  
- 表にしたい部分を選択して右クリック。コマンドパレットを選択。  
- 「Excel to Markdown Table」を実行する。  

[ExcelやGoogleスプレッドシートをMarkdown出力するVS Codeの拡張機能「Excel to Markdown table」が便利すぎる件](https://dev.classmethod.jp/articles/excel-to-markdown-table/)  

---

## markdown 引用 分けたい

``` md
>引用1

>引用2
```

とすると、警告が表示されて不快。  

``` md
>引用1

&nbsp

>引用2
```

とすると、警告は出ないが、間隔が広すぎる & そもそも`&nbsp`なんて覚えていられない。  

何かいい方法がないか検索してみたら同じような症状に悩んでいる人がいたので、拝借させてもらった。  

``` md
>1つ目の引用

<!-- -->
>2つ目の引用
```

このように一行空けてからHTMLのコメントを入れることで警告も出ず、間隔もいい感じになった。  
`ctrl + k + c`で簡単に挿入できるのでとりあえずはこれで行くことにする。  

[Markdownで連続したblockquoteを分割して表示する方法](https://www.umurausu.info/blog/archives/blockquotes-in-markdown.html)  
