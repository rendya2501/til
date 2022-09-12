# PHP_通信

---

## file_get_contents関数

ファイルの内容を全て読み込む関数。  
ファイルの代わりにURLを指定して外部のサイトにアクセスする機能がある。  
Webブラウザのような操作はできない。  

``` php : post例
    $url = 'https://httpbin.org/post';
    $header = [
        'Accept-Charset: UTF-8',
        'Content-Type: application/x-www-form-urlencoded'
    ];
    $param = [
        'msg' => 'メッセージ',
    ];
    $context = [
        'http' => [
            'method'  => 'POST',
            'header'  => $header,
            'content' => json_encode($param)
        ]
    ];
    $html = file_get_contents($url, false, stream_context_create($context));
    echo $html;
```

---

## cURL

cURLは、最初からPHPに組み込まれている関数ではなく、ダニエル・ステンバーグさんが開発したPHPの外部ライブラリ。  
Webブラウザのような機能を提供するライブラリ。  
http通信やhttps通信、ファイルのアップロード、クッキー、ユーザー認証などにも利用できる。  
cURLはデフォルトで通信を暗号化する。  

---

## guzzle http client

今、主流のhttpクライアント。  
Laravelではデフォルトで入っている模様。  
パッケージとして提供されているので、コンポーザーでこれ単体でインストール可能。  

>Guzzle は、HTTP 要求の送信を容易にし、簡単に Web サービスと統合できるようにする PHP HTTP クライアントです。  
[公式](https://docs.guzzlephp.org/en/stable/)  

---

## 受け取るAPI側

JSONで受け取る例  

``` php
$user_id  = filter_input(INPUT_POST, 'user_id');
$password = filter_input(INPUT_POST, 'password');

// $post_json_data にJSON形式で格納される
$post_json_data = file_get_contents("php://input");
// JSONファイルを連想配列へ変換
$post_obj_data = json_decode($post_json_data, true);
```

`php://input` は読み込み専用のストリームで、 リクエストの body 部から生のデータを読み込むことができます。  
`php://input` は、 `enctype=”multipart/form-data”` に対しては使用できません。  

---

## 参考リンク

[PHPでPOST送信まとめ](https://qiita.com/okdyy75/items/d21eb95f01b28f945cc6)  
[APIクライアント開発時のモックに使えるhttpbinの紹介](https://qiita.com/sameyasu/items/adacceb8a1bee893599b)  
[PHP でルーターを書いてみる](https://zenn.dev/iamwanabi/articles/52326ab451373b)  
