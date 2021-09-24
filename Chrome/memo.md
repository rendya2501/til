# GoogleChromeメモ

## F12,Json掃き出し

1. 一番上のログを右クリックでGlobalなんちゃらで掃き出し。
2. JSON.stringify(temp1)でjson掃き出し
3. 一番右にcopyボタンがあるので、コピー
4. cntl+nで新規ファイル作成。
5. 貼り付け
6. 右下プレーンテキストをクリックしてjsonに変更
7. 右クリック、フォーマット

`JSON.stringify()`  
JavaScript のオブジェクトや値を JSON 文字列に変換します。  

---

## Chrome開発者ツールのコンソールに表示したObjectをクリップボードにコピーする

<https://dackdive.hateblo.jp/entry/2015/09/10/100117>  

`copy(temp1)`  

これだけでいける。すごいぞ。  
temp1がObjectで素直にjsonになってくれるならいいけど、そうじゃないなら、JSON.stringify()と合わせて出力すればいけるかも。  
`copy(JSON.stringify(temp1))`  

---

## パスワード忘れてしまったけどブラウザには記憶されていて見たい場合

F12のデベロッパーツールでパスワード欄選択してtype="password"をtextに変更することで丸見えになる。  
