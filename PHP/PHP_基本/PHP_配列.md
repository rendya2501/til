# PHP_配列

---

## 配列の種類

- 通常の配列  
- 連想配列  
- 多次元配列  

---

## 通常配列

- PHPの配列はキーが数値の連想配列みたいな扱いな模様。  
- なのでキーは任意に飛ばすことができる。  
- 基本的に配列にはなんでも入る。  

■**キーは飛ばせる**  

``` php
$arr[0] = '値1';
$arr[1] = '値2';
$arr[2] = '値3';
$arr[5] = '値6';

$arr = [0 => '値1',1 => '値2', 5 => '値6'];
```

■**なんでも入る**

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

## 配列の初期化

- `array()` で作成する。  
- `[]` (角括弧) で作成する。  
  - `[]`は配列の短縮構文。PHP5.4以降で使用可能。  
- 値を格納することで、0から順番に添字（要素番号）が割り振られる。  

■**基本的な初期化**

``` php
$arr = array('値1', '値2', '値3');

$arr = ['値1', '値2', '値3'];
```

---

## 通常配列への追加

1. array_push  
2. `[]` 演算子  
3. array_merge  

``` php
// 1. array_push
$hoge = ['AAA','BBB'];
array_push($hoge,'CCC','DDD');

// 2. []
$hoge = ['AAA','BBB'];
$hoge[] = 'CCC';
$hoge[] = 'DDD';

// 3. array_merge
// PHPで配列はキーが数値の連想配列なので、array_mergeが使える。
// また、array_mergeはキーが数値の場合、キーを採番し直すのでちょうど追加したような形になる。
$hoge = ['AAA','BBB'];
$hoge = array_merge($hoge,['CCC','DDD']);

print_r($hoge);
// Array
// (
//     [0] => AAA
//     [1] => BBB
//     [2] => CCC
//     [3] => DDD
// )
```

---

## 連想配列

添字に好きな名前を付けられる配列  
インデックスが数値ではなく任意の文字列でキーを定義した配列  

---

## 連想配列への追加

1. array_merge  
2. `+=` 演算子  
3. `[]` 演算子  

```php
// 1. array_merge
$arr = ['key1' => 'value1'];
$arr = array_merge($arr,['key2'=>'value2']);

// 2. +=
$arr = ['key1' => 'value1'];
$arr += ['key2' => 'value2'];

// 3. []
$arr = ['key1'=>'value1'];
$arr['key2'] = 'value2';

print_r($arr);
//  Array
// (
//     [key1] => value1
//     [key2] => value2
// )
```

### array_mergeと+演算子の違い

- 同一キーの場合  
  - array_merge : value値を上書きする  
  - +演算子 : value値を上書きしない  

``` php
$hoge_1 = ['key1'=>'value1'];
$hoge_2 = ['key1'=>'value2'];

print_r(array_merge($hoge_1,$hoge_2));
// Array
// (
//     [key1] => value2
// )
print_r($hoge_1 + $hoge_2);
// Array
// (
//     [key1] => value1
// )
```

- キーが数値の場合
  - array_merge : キーを採番し直す  
  - +演算子 : そのまま  

``` php
$hoge_1 = [5=>'value1'];
$hoge_2 = [8=>'value2'];

print_r(array_merge($hoge_1,$hoge_2));
// Array
// (
//     [0] => value1
//     [1] => value2
// )

print_r($hoge_1 + $hoge_2);
// Array
// (
//     [5] => value1
//     [8] => value2
// )
```

### ダメなパターン

``` php
$arr = ['key1' => 'value1'];
$arr[] = ['key2' => 'value2'];
print_r($arr);
// Array
// (
//     [key1] => value1
//     [0] => Array
//         (
//             [key2] => value2
//         )
// )


$arr = ['key1' => 'value1'];
$arr['key2'] = ['value2'];

print_r($arr);
// Array
// (
//     [key1] => value1
//     [key2] => Array
//         (
//             [0] => value2
//         )
// )
```

---

## jsonを意識した多重配列 + 連想配列

``` php
// 初期化
$params['plans'] = [
    ['cat' => 'meow', 'dog' => 'woof'],
    ['cow' => 'moo','computer' => 'beep' ]
];
print(json_encode($params));
// {"plans":[{"cat":"meow","dog":"woof"},{"cow":"moo","computer":"beep"}]}
```

``` php
// これと
$params1['plans'][] = ['cat' => 'meow', 'dog' => 'woof'];
print(json_encode($params1));
// {"plans":[{"cat":"meow","dog":"woof"}]}

// これは同じ
$params2['plans'] = [['cat' => 'meow', 'dog' => 'woof']];
print(json_encode($params2));
/// {"plans":[{"cat":"meow","dog":"woof"}]}
```

## 多重配列 + 連想配列への追加

array_merge案はまずないな。  
`+=`が一番わかりやすい気がする。  

```php
// 1. [] その1
$params['plans'][] = ['cat' => 'meow', 'dog' => 'woof'];
$params['plans'][] = ['cow' => 'moo','computer' => 'beep'];

// 1. [] その2
$params['plans'] = [['cat' => 'meow', 'dog' => 'woof']];
$params['plans'][] = ['cow' => 'moo','computer' => 'beep'];

// 2. +=
$params['plans'] = ['cat' => 'meow', 'dog' => 'woof'];
$params['plans'] += ['cow' => 'moo','computer' => 'beep'];

// 3. array_merge
$params['plans'] = [['cat' => 'meow', 'dog' => 'woof']];
$params['plans'] = array_merge($params['plans'],[['cow' => 'moo','computer' => 'beep' ]]);

print(json_encode($params));
// 結果
// {"plans":[{"cat":"meow","dog":"woof"},{"cow":"moo","computer":"beep"}]}
```

---

## 参考リンク

[【PHP】連想配列、配列への追加](https://qiita.com/kazu56/items/6947a0e4830eb556d575)  
[PHPの配列をマスターしよう！【基本から初心者向け】](https://wepicks.net/phpref-array/)  
