# PHP_配列

---

## 配列の種類

- 通常の配列  
- 連想配列  
- 多次元配列  

---

## 配列の初期化

- `array()` で作成する。  
- `[]` (角括弧) で作成する。  
  - `[]`は配列の短縮構文。PHP5.4以降で使用可能。  
- 値を格納することで、0から順番に添字（要素番号）が割り振られる。  
- 配列にはなんでも入る  

``` php
$arr = array('値1', '値2', '値3');

$arr = ['値1', '値2', '値3'];
```

``` php
$arr[0] = '値1';
$arr[1] = '値2';
$arr[2] = '値3';
$arr[5] = '値6';

$arr = [0 => '値1',1 => '値2', 5 => '値6'];
```

``` php
class Hoge{}
$arr = [1,"aa",new Hoge(),true];
foreach ($arr as $value){
     var_dump($value);
}
// int(1)
// string(2) "aa"
// object(Hoge)#1 (0) {
// }
// bool(true)
```

---

## 連想配列

---

## 連想配列への追加の仕方

`+=演算子` でいける。  

```php
 $plan_params = ['type' => 'regular'];
 $plan_params += ['type2' => 'regular'];
 
 print_r($plan_params);
// 結果:
//  Array
// (
//     [type] => regular
//     [type2] => regular
// )
```

---

## 多重配列への追加

```php
$params['plans'][] = ['cat' => 'meow', 'dog' => 'woof'];
$params['plans'][] = ['cow' => 'moo','computer' => 'beep' ];
print_r($params);
print(json_encode($params));
// 結果
// {"plans":[{"cat":"meow","dog":"woof"},{"cow":"moo","computer":"beep"}]}
```
