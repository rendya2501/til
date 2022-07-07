# CSSの基本

[基本的にここを参考にした](https://www.tagindex.com/stylesheet/basic/)  

---

## 書式の基本

### セレクタ・プロパティ・値

``` css
p { color: red }
```

p :: セレクタ (スタイルを適用させる対象)  
color :: プロパティ  
red :: 値  

### セレクタの組み合わせ

#### Aセレクタ Bセレクタ （子孫セレクタ）

セレクタとセレクタを半角スペースで区切って記述すると、Aセレクタの範囲内にあるBセレクタにのみ、スタイルを適用させることができます。
例では、p要素内のstrong要素にのみ、color: red が適用されます。それ以外のstrong要素には、このスタイルは適用されません。  

``` css
p strong { color: red; }
```

![alt](https://www.tagindex.com/stylesheet/basic/image/selector1.gif)  

#### Aセレクタ > Bセレクタ （子供セレクタ）

セレクタとセレクタを [>] で区切って記述すると、Aセレクタの直下にあるBセレクタにのみ、スタイルを適用させることができます。  
直下とは位置的な下ではなく、階層的な下のことです。（直接の子要素という意味）  
例では、p要素の直下にあるstrong要素にのみ、color: red が適用されます。それ以外のstrong要素には、このスタイルは適用されません。  
この指定方法は、IEではバージョン7から対応しています。（ただし、表示モードが標準モードの場合に限ります）  

``` css
p > strong { color: red; }
```

![子供セレクタのイメージ](https://www.tagindex.com/stylesheet/basic/image/selector2.gif)  

#### Aセレクタ + Bセレクタ （隣接セレクタ）

セレクタとセレクタを + で区切って記述すると、両方のセレクタが同じ親要素を持ち、Aセレクタの直後にあるBセレクタにのみ、スタイルを適用させることができます。  
例では、h2要素と同じ親要素（body要素）を持ち、かつh2要素の直後にあるp要素にのみ、color: red が適用されます。それ以外のp要素には、このスタイルは適用されません。  
この指定方法は、IEではバージョン7から対応しています。（ただし、表示モードが標準モードの場合に限ります）  

``` css
h2 + p { color: red; }
```

![隣接セレクタのイメージ](https://www.tagindex.com/stylesheet/basic/image/selector3.gif)  

---

## CSS Class指定の仕方

[クラス名を使った指定](https://www.tagindex.com/stylesheet/basic/format2.html)  

### 要素名に続けてクラス名を指定

要素名に続けてクラス名を指定すると、そのタイプの要素でのみクラスの指定が適用されます。

``` css : 要素名に続けてクラス名を指定する方法
p.example1 { color: red; }
```

### クラス名だけで指定

クラス名だけで指定すると、要素のタイプに関連しないクラスの指定が行えます。

``` css : クラス名だけで指定する方法
.example2 { color: blue; }
.example3 { font-size: 80%; }
```

### 子孫セレクタ

セレクタとセレクタを半角スペースで区切って記述すると、特定の範囲内にある要素にのみスタイルを適用させることができます。

``` css
div.example p { color: red; }
```

次の例では、example が指定されたdiv要素内のp要素にのみ、color: red が適用されることになります。それ以外のp要素には、このスタイルは適用されません。

``` html
<style type="text/css">
    div.example p { color: red; }
</style>

<body>
    <div class="example">
        <p>このテキストにスタイルが適用されます</p>
        <p>このテキストにスタイルが適用されます</p>
    </div>
    <div>
        <p>このテキストには適用されません</p>
    </div>
</body>
```

### 要素は上書きされる

<https://codepen.io/pen/>  

入れ子になったクラスへのアクセスはスペースで区切った後にクラス名を指定すると有効になる。  
親のクラスの影響を受ける。  
プロパティの内容は上書きできる。親で色を赤としておきながら、緑を指定すれば緑になる。  

``` html
<div class="example">
    <div>適応されない</div>
    <p>このテキストにスタイルが適用されます</p>
    <p class="example2">このテキストにスタイルが適用されます</p>
    <p class="example3">このテキストにスタイルが適用されます</p>
</div>
```

``` css
/* 全体に影響させる */
div.example p { padding: 10px; color: red; }
/* 親のpaddingを打ち消す。カラーは上書きされる。 */
div.example .example2 { padding:0px; color: green; }
/* 親padding + margin */
div.example .example3 { margin:10px; color: blue; }
```

---

## CSS Class名命名規則

[ID名とクラス名のサンプル集](https://www.tagindex.com/stylesheet/basic/naming.html)  
