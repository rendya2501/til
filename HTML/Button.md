# Button

---

## aタグをボタンにする

Blazorで出来たけど、普通にButtonを使うべし。
submitはformタグの中で有効。
この時Typeがsubmit属性の場合、formのactionに書かれているurlに飛ぶ、という動作になる模様。  
デフォルトでsubmit属性なので、`onclick="history.back()"`としても、`submit`としての動作が優先されるので、`type="button"`にしてsubmitではない動作を保証するようにしましょうって話か。  

[ボタンは\<button>で実装しよう](https://qiita.com/sukoyakarizumu/items/978df93755c4720d5cdc)  

`<button type="button">`  
[「戻る」ボタンを作るときは\<button type="button" >で。](https://qiita.com/hibikikudo/items/937d451bb6570a55a47c)  

``` html
  <form action="/index.html">
  <div>
    <button>ボタン</button>
    <button type='submit'>submit ボタン</button>
    <button type='button'>button ボタン</button>
    <button onclick="history.back()">戻る</button>
    <button type="submit" id="submit">登録</button>
  </div>
  </form>
```

[buttonタグの使い方](https://code-kitchen.dev/html/button/)  
