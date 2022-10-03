# HTMLメモ

---

## HTML HEAD

ページが読み込まれてもウェブブラウザーには表示されない部分。  
構成情報などを記述する場所と覚えて差し支えないか。  

例:  
・`<title>` といった情報や CSS へのリンク (もし HTML を CSS で修飾したいならば)  
・独自のファビコンへのリンク  
・メタデータ (HTML を誰が書いたのかとかその HTML を表現する重要なキーワードなど)  

※メタデータ : データを説明するデータ  
`<meta charset="utf-8">`

---

## DOCTYPE

`<!DOCTYPE html>`
html文書であることを示すタグ
一番上に絶対に書く

---

## ブロック要素  

・高さ、余白を設定できる。  
・中央寄せ等は出来ない。  

・ブロックレベル要素は常に新しい行から始まり、利用可能な全幅を取ります (できる限り左右に伸びます)。  
・blockの要素を中央に配置したいときにはmargin-right:auto; margin-left:autoという指定により横に中央配置にすることができます。  
・代表的なタグ :: div,p等  

[ブロックレベル要素](https://developer.mozilla.org/ja/docs/Web/HTML/Block-level_elements)

---

## インライン要素  

・高さ、余白は設定できない。  
・中央寄せ等の設定ができる。  

・基本的にブロック要素の中で用いられる。  
・代表的なタグ :: a,span等  

[インライン要素](https://developer.mozilla.org/ja/docs/Web/HTML/Inline_elements)  
