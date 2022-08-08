# PHP_演算子

---

## 演算子早見表

``` txt
-> : アロー演算子
=> : ダブルアロー演算子
:: : スコープ定義演算子
:? : エルビス演算子
```

---

## アロー演算子 [->]

クラスのメソッドやプロパティにアクセスするための演算子。  
左辺から右辺を取り出す。  

---

## ダブルアロー演算子 [=>]

配列や連想配列において、キーに対してバリューを指定する際に使用する演算子。  

``` php
// 配列
$names = array(0=>"山田", 1=>"田中", 2=>"髙橋");
// 連想配列
$person = array("name"=>"山田", "age"=>22, "gender"=>"男性");
```

---

## スコープ定義演算子 [::]

クラスの定数や静的なメソッドにアクセスする演算子。  

---

## エルビス演算子 [:?]

phpにはfalseの時だけ実行するエルビス演算子なるものが存在する。  
falseの時だけ実行してほしい処理って割とあるので、C#にもあってほしいと願うのだがどうなるかね。  

```php : エルビス演算子の使いどころさん
// statusCodeが500の時エラーメッセージを取得する
$status = 500;
$message = 'err';
$err = null;

// falseの時だけ実行される。200 === 500でfalse。
$status === 200 ?: $err .= $message;

print $err ?? 'naiyo';
```

---

## 三項演算子

phpの三項演算子は癖が強く、なんでそうなるの？って動作をしたのでまとめたのだが、だいぶ前にまとめた内容なので、また綺麗にまとめたい。  

``` php
<?php
    const DELETE_TYPE_PHYSICAL = 1;
    const DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE = 0;
    const DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE = 2;
    
    $delType = DELETE_TYPE_PHYSICAL;
    $landingAchieveFlag = true;
    
    $extendDelType = $delType === DELETE_TYPE_PHYSICAL
        // 物理削除はそのまま
        ? DELETE_TYPE_PHYSICAL
        : $landingAchieveFlag
            // 論理削除+実績あり
            ? DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE
            // 論理削除+実績なし
            : DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            
    $extendDelType2 = (function ($delType, $landingAchieveFlag) {
        if ($delType === DELETE_TYPE_PHYSICAL) {
            return DELETE_TYPE_PHYSICAL;
        } else {
            if ($landingAchieveFlag) {
                return DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE;
            } else {
                return DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            }
        }
    })($delType,$landingAchieveFlag);
    
    var_dump($extendDelType);
    var_dump($extendDelType2);
```

---

## 参考

[【PHP】アロー演算子→とスコープ定義演算子::](https://qiita.com/sho91/items/c6503e7d344ca29aa03f)  
