# PHP_演算子

---

## エルビス演算子

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
