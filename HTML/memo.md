# HTMLに関する色々

---

## HTML5の廃止

<https://future-architect.github.io/articles/20210621a/>  
<https://www.tohoho-web.com/html/memo/htmlls.htm>  

全然知らなかった。
HTML5は2021年1月28日に廃止された模様。  
次期HTMLのバージョンはHTML Living Standardと呼ぶらしい。  
ヴァージョンに関して解説してあるページがあるのでそこを参照されたし。  
ざっくりいうと、開発元(W3C)とWeb系(Apple,Mozilla)の強い会社でバチバチにやりあって、Webの会社が勝利したって結末。  
色々あって面白い。  

---

## 見た目参考

[Apple風のかっこよくて美しいデザインの背景49種類を簡単に実装できるスタイルシート -HUE.css](https://coliss.com/articles/build-websites/operation/css/gradient-background-hue-css.html)  

[便利なCSSジェネレーターを4つ集めました](https://twitter.com/mndgn_y/status/1471465529571672065)  
ローディングアニメーション :: Epic Spinners  
グラスモーフィズム :: grassmorphism-generator  
ニューモフィズム :: Neumorphism.io  
フレックスボックス :: Flex layout Generator  

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

## ただの文字列でも無理にタグを付けたほうがよいのか？

ないらしい。  
XAMLではTextBlockだの、Labelだの指定する必要があるが、HTMLでは必要はないらしい。  

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
