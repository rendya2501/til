# php色々

## 連想配列をforeachするときにインデックスも欲しい

```php
$array = array( 
    'cat' => 'meow', 
    'dog' => 'woof', 
    'cow' => 'moo', 
    'computer' => 'beep' 
);

foreach( array_keys( $array ) as $index => $key ) {
    // display the current index + key + value
    echo 'index:' . $index . ' key:' . $key . ' value:' . $array[$key] . "\n";
}
// 結果
// index:0 key:cat value:meow
// index:1 key:dog value:woof
// index:2 key:cow value:moo
// index:3 key:computer value:beep
```

## 多重配列への追加

```php
$params['plans'][] = ['cat' => 'meow', 'dog' => 'woof'];
$params['plans'][] = ['cow' => 'moo','computer' => 'beep' ];
print_r($params);
print(json_encode($params));
// 結果
// {"plans":[{"cat":"meow","dog":"woof"},{"cow":"moo","computer":"beep"}]}
```

## PHPのforeachで作った変数はforeach抜けた後も有効

公式のリファレンスにも書いてありました。  
unsetはマナー的にあってもいいのかもしれないですね。  

---

## 多次元配列をimplode()する最も簡単な方法

全部結果は同じ。でも案3が一番おすすめ。短くてわかりやすい。  

```php
$params = array (
  'plans' => 
  array (
    0 => 
    array (
      'linkage_plan_id' => '05539999950000020001',
      'is_searchable' => false,
    ),
    1 => 
    array (
      'linkage_plan_id' => '05539999950000020002',
      'is_searchable' => false,
    ),
  ),
);
// 案1
print(
    implode(
        ",",
        array_map(
            fn($el) => $el['linkage_plan_id'], 
            $params['plans']
        )
    )
);
// 案2
print(
    implode(
        ',',
        array_map(
            'implode',
            $params['plans'],
            array_fill(0, count($params['plans']),
            '')
        )
    )
);
// 案3
print(
    implode(
        ',',
        array_column($params['plans'], 'linkage_plan_id')
    )
);
```

---

## PHPのプロセスを止める方法

<https://flashbuilder-job.com/php/635.html>

`ps aux | grep php`  
これでプロセスID（左から２つ目の値）を調べる。  

ルートユーザー、またはルートになれるユーザーでコマンド実行  
`sudo kill -9 (プロセスID)`  

---

## 多次元配列中の特定のキーを全て削除する方法

```php
$plan_params = array (
  'plans' => 
  array (
    0 => 
    array (
      'type' => 'regular',
      'linkage_plan_id' => '05539999950000020001',
      'basis_content' => 
      array (
        'name' => '4_伊藤テスト_連携改善_固定_1',
        'base_price' => 18400.0,
      )
    ),
    1 => 
    array (
      'type' => 'regular',
      'linkage_plan_id' => '05539999950000020002',
      'basis_content' => 
      array (
        'name' => '4_伊藤テスト_連携改善_固定_1',
        'base_price' => 18500.0,
      )
    ),
    2 => 
    array (
      'type' => 'regular',
      'linkage_plan_id' => '05539999950000020003',
      'basis_content' => 
      array (
        'name' => '4_伊藤テスト_連携改善_固定_1',
        'base_price' => 18600.0,
      )
    )
  ),
);

array_walk(
    $plan_params['plans'],
    function(&$v){
        unset($v['type']);
        unset($v['linkage_plan_id']);
        unset($v['basis_content']['name']);
    }
);
print_r($plan_params);
```

---

## 連想配列への追加の仕方

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

## コールバック

[いい感じのコールバックサンプル]<https://qiita.com/dublog/items/0eb8bcea2fc452c0b4b2>

```php
const API_URI = "API_URI";
const ServiceURL = "https://";

function add()
{
    $params = ['1,2,3,4,5'];
    $try = function ($token) use ($params) {
        return [
            'PATCH',
            ServiceURL . API_URI,
            [
                'Authorization' => 'Bearer '.$token,
                'Content-Type' => 'application/json'
            ],
            json_encode($params)
        ];
    };
    retry($try);
}

// 共通のリトライ処理を別メソッドとして用意しておき、それぞれの処理を噛ませる。
function retry(callable $try)
{
    $token = "tekitou_token";

    $result = $try($token);

    print_r($result);
}

add();
```

## エルビス演算子の使いどころさん

```php
// statusCodeが500の時エラーメッセージを取得する
$status = 500;
$message = 'err';
$err = null;

// falseの時だけ実行される。200 === 500でfalse。
$status === 200 ?: $err .= $message;

print $err ?? 'naiyo';
```
