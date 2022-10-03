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

## Overlay

``` html
    <!-- <b-container class="overlay" v-if="isBusyFlag">
      <b-spinner />
    </b-container> -->
```

``` css
/** 読み込みクルクル */
.overlay {
  /* 要素を重ねた時の順番 */
  z-index: 1;

  /* 画面全体を覆う設定 */
  position: fixed;
  top: 0;
  left: 0;
  min-width: 100%;
  min-height: 100%;
  background-color: rgba(0, 0, 0, 0.5);

  /* 画面の中央に要素を表示させる設定 */
  display: flex;
  align-items: center;
  justify-content: center;
}
```

---

## bootstrap-vueでテキストの太字

`<p class="font-weight-bold">Bold text.</p>`

[Text](https://getbootstrap.com/docs/4.1/utilities/text/)  

---

## justify-contetで右寄せにならない

[justify-content「flexアイテムの水平方向の揃え方」](https://web-designer.cman.jp/css_ref/abc_list/justify-content/)  

・display : flex であること。  
・ブラウザ対応が必要。  

``` css
/* ----- 右揃え ----- */
#id2 {
  -webkit-justify-content: flex-end;         /* Safari etc. */
  -ms-justify-content    : flex-end;         /* IE10        */
  justify-content        : flex-end;
}
```

---

## ブロック要素の中央寄せ

[ブロック内でブロック要素を上下左右中央揃えにする技](https://qiita.com/HiromuMasuda0228/items/6a51c2ce24c69c937092)

---

## DOCTYPE

`<!DOCTYPE html>`
html文書であることを示すタグ
一番上に絶対に書く
