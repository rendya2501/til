# VSCodeメモ

## 除外するファイルの複数指定

カンマで区切る。  
`.xml , .xaml`  

[VS Code の「検索」時に「含めるファイル」や「除外するファイル」を指定する際はカンマ区切りにする](https://obel.hatenablog.jp/entry/20200409/1586410881)  

---

## grepで複数行の検索の仕方

どちらかといえば、正規表現の記事にまとめるべきだが、VSCodeのことで困ったのでこっちに書く。  
ログを調べる時に備考が長すぎて、arrayを折りたためなくて困ったので、複数行を検索して空白で置換できないかと思って調べた。  
特定の行から特定の行までを検索する線で調べたのだが、catコマンドを使うものばかり出てきて、その時は結局わからなかったが、後日「VSCode 複数行 正規表現」で調べたら理想の結果を得られたのでまとめる。  

`開始文字[\s\S\n]*? 終了文字`  

``` regexp
'remarks' => '[\s\S\n]*?',
```

[VS Codeで複数行に渡って正規表現を利用する](https://qiita.com/birdwatcher/items/dee34a11619b11e1fe81)  

---

## Remote-SSH でサーバ上のファイルを直接編集する方法

[[VSCode] Remote-SSH でサーバ上のファイルを直接編集する方法](https://webbibouroku.com/Blog/Article/vscode-remote-ssh)  
この記事を実際にやってみて、行けたならこれで改めてまとめる。  
→  
大体この記事の通りでよかった。  

エラー発生  
Could not establish connection to "<サーバーのホスト名>"  

[VS Code Remote Development SSHセットアップ中にハマったこと](https://qiita.com/igrep/items/3a3ba8e9089885c3c9f6)  

設定→Remoteで検索→「Remote - SSH」を選択→Remote.SSH: Config Fileに以下の文言を設定  
`C:\Users\<user_name>\.ssh\config`  

自分はここまででつながった。  

---

## 除外するファイル 複数指定

複数の条件は カンマ「,」 区切り  

[[vscode] 複数ファイルからの検索方法](https://www.chihayafuru.jp/tech/index.php/archives/2294)  

---

## `フォルダ右クリック→Codeで開く` のコマンド

`code .`  

コマンドプロンプトでもいけた。  
ターミナルならなんでもいけると思われる。  
