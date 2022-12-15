# Button

---

## aタグをボタンにする

結論:**普通にボタンを使え**  

`aタグ`、`inputタグ`もボタンとして使う事ができる。  
何が違うのか調べた。  

また、Blazorでは`a`タグを以下のように記述する事でボタンとしての動作としてリンクへの遷移を実現できた。  
ボタンでOnClickに処理を記述する必要もなかったので、ボタンの意味を疑った。  

`aタグ`にボタンロールを与えたら見た目もボタンになるのかと思っていたが、bootstrapのクラスによってボタンになっているだけだった。  
クラスを外したらただのリンクになった。  

``` html
<a href='/user/edit/@user.UserId' class="btn btn-secondary" role="button">
    Edit
</a>
```

■**検証**  

このまとめをしている時はHTMLを知らない状態なので基礎的な事からまとめていく。  

submitはformタグの中で有効。  
この時、`type`がsubmit属性の場合、formのactionに書かれているurlに飛ぶ、という動作になる模様。  
※`onSubmit`はactionの前に実行されるイベントの模様。  

ボタンの`type`はデフォルトでsubmit属性なので、`onclick="history.back()"`としても、`submit`としての動作が優先されるので、actionのurlに飛んでしまう。  
「戻る」など、submitでactionが実行されないようにするためには`type="button"`にする必要がある。  

``` html
<form action="" onSubmit="sample()">
  <!-- アラート表示 -->
  <button>default ボタン</button>
  <!-- アラート表示 -->
  <button type='submit'>submit ボタン</button>
  <!-- アラート表示 -->
  <button onclick="history.back()">default submit 戻る</button>
  <!-- アラート表示されない -->
  <button type='button' onclick="history.back()">button 戻る</button>
</form>

<script>
  function sample() {
    alert("aaa");
  }
</script>
```

- メモ  
  - `action=""`は何もしない。  
  - buttonのtypeのデフォルトはsubmitであることが分かった。  

>\<div class="button">や\<a class="button">といったように、ボタンを\<button>以外で実装するのは極力避けよう、という話。  
[ボタンは\<button>で実装しよう](https://qiita.com/sukoyakarizumu/items/978df93755c4720d5cdc)  
[「戻る」ボタンを作るときは\<button type="button" >で。](https://qiita.com/hibikikudo/items/937d451bb6570a55a47c)  

[buttonタグの使い方](https://code-kitchen.dev/html/button/)  
[document.フォーム名.action …… フォームの送信先を設定する](http://www.htmq.com/js/form_action.shtml)  
