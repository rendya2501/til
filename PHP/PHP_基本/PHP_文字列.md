# PHP_文字列

---

## nullと文字列結合演算子

<https://tomcky.hatenadiary.jp/entry/20180810/1533908140>

nullを文字列結合するとNULLは常に空文字列に変換されるので、is_nullに引っかからなくなる、びっくりphp言語仕様。  
この場合はempry()を使うといいらしい。  
エラーメッセージだけ取得する関数作って、それを直接、エラー変数にぶち込んだらis_null利かなくなって嵌ったので残す。  

``` php
function getNull() {
    return null;
}
$tmp = getNull();
print is_null($tmp) ? 1 : 0; // 1

$ee = null;
$ee .= $tmp;
print is_null($ee) ? 1 : 0; // 0

$ee = null;
$ee .= null;
print is_null($ee) ? 1 : 0; // 0
```

---

## 最後の文字を消す

別にまとめる程のことでもないかもしれないが、色々方法があるので、一番手っ取り早い奴だけまとめておく  

対象文字列、スタート位置、長さ  
`substr($string, $start, $length)`  

``` php
$mystring = "This is a PHP program.";
echo substr($mystring, 0, -1);
// 出力：This is a PHP program
```

[PHP で文字列から最後の文字を削除する](https://www.delftstack.com/ja/howto/php/php-remove-last-character-from-string/)  
