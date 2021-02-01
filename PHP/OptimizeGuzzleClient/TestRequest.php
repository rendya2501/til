<?php
require 'vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Psr7\Request;


function getRequest($method, $url, $body)
{
    return new Request(
        $method,
        $url,
        ['Content-Type' => 'application/json'],
        json_encode($body)
    );
}

function getPostParam()
{
    $method = 'post';
    $url = 'https://httpbin.org/post';
    $body_type = 'json';
    $params = [
        'linkage_vendor_id' => '00',
        'linkage_golf_course_id' => '00000',
        'password' => 'passwordpassword'
    ];
    $body = [$body_type => $params];
    return getRequest(
        $method,
        $url,
        $body
    );
}


function getGetParam()
{
    $method = 'get';
    $url = 'https://httpbin.org/get';
    $body_type = 'query';
    $params = '123456789,987654321';
    $body = [$body_type => $params];
    return getRequest(
        $method,
        $url,
        $body
    );
}

function executeRequestVer()
{
    $client = new Client(['http_errors' => false]);
    $request = getGetParam();

    echo $request->getUri();
    echo "\n";
    echo $request->getMethod();
    echo "\n";
    echo $request->getBody();
    echo "\n";

    // API実行
    $response = $client->send($request);

    // レスポンス分解
    $response_content = json_decode($response->getBody(), true, 512, 0);
    var_dump($response_content);
}



/**
 * HTTPクライアントを取得します
 *
 * @return \GuzzleHttp\Client
 */
function getClient($base_url)
{
    return new Client(
        [
            'http_errors' => false,
            'base_uri' => $base_url . '/',
            'headers' => [
                'Content-Type' => 'application/json'
            ]
        ]
    );
}

function executeClientVer()
{
    $base_url = 'https://httpbin.org';
    $client = getClient($base_url);

    // 色々準備
    $method_name = 'post';
    $path = 'post';
    $body_type = 'json';
    $params = [
        'linkage_vendor_id' => '00',
        'linkage_golf_course_id' => '00000',
        'password' => 'passwordpassword'
    ];
    $payload = [$body_type => $params];

    // API実行
    $response = $client->request(
        $method_name,
        $path,
        $payload
    );

    // echo $client->getUri();
    // echo "\n";
    // echo $client->getMethod();
    // echo "\n";
    // echo $client->getBody();
    // echo "\n";

    // レスポンス分解
    $response_content = json_decode($response->getBody(), true, 512, 0);
    var_dump($response_content);
}

try {
    // executeRequestVer();
    executeClientVer();
} catch (Exception $e) {
    echo json_encode($e);
}

// /**
//  * APIを実行します。
//  *
//  * @param Client $client      クライアント
//  * @param array  $params      APIパラメータ
//  * @param string $url         サービス先URL
//  * @param string $method_name メソッド名
//  *
//  * @return array status,message,contentで構成される連想配列
//  */
// function executeAPI($client, $request)
// {
//     // 呼び出し元の関数名を取得する
//     $dbg = debug_backtrace();
//     $calling_func_name = $dbg[1]['function'];

//     // INPUTログ生成
//     SiteControllerWorkerService::writeWebCooperationLog(
//         __CLASS__ . '->' . $calling_func_name,
//         "\n***INPUT***\n" . $request->getUri() . "\n"
//             . $request->getMethod() . "\n"
//             . var_export($request->getBody(), true),
//         \Config::get('const.portal.gora'),
//         SiteControllerWorkerService::LOG_TYPE_CODE_IO
//     );

//     // API実行
//     $response = $client->send($request);

//     // OUTPUTログ生成
//     SiteControllerWorkerService::writeWebCooperationLog(
//         __CLASS__ . '->' . $calling_func_name,
//         "\n***OUTPUT***\n"
//             . $response->getStatusCode() . "\n"
//             . var_export(\GuzzleHttp\json_decode($response->getBody(), true), true),
//         \Config::get('const.portal.gora'),
//         SiteControllerWorkerService::LOG_TYPE_CODE_IO
//     );

//     // rewindしないと、responseを再使用できない。
//     $response->getBody()->rewind();
//     // レスポンスを分離して返却
//     return $this->divideResponse($response);
// }
