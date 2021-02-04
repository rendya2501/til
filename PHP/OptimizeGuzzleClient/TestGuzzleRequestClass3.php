<?php

/**
 * GuzzleClient
 *
 * @package PlanCooperationGORAWorkerService
 * @author s.ito <s.ito@alp-inc.jp>
 */

require 'vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Psr7\Request;

/**
 * GORALoginAPI
 */
function callLoginAPI()
{
    // パラメータ生成
    $params = [
        'linkage_vendor_id' => 'aa',
        'linkage_golf_course_id' => 'sssss',
        'password' => 'asfsdfsdfsdfsdfsdfs'
    ];
    // リクエスト生成
    $request = new Request(
        'POST',
        'https://httpbin.org/post',
        ['Content-Type' => 'application/json'],
        json_encode(['json' => $params])
    );
    // クライアント生成
    $client = new Client(
        [
            'http_errors' => false
            // 'base_uri' => 'https://httpbin.org'
        ]
    );
    // API実行
    $response = executeAPI($client, $request);
    // レスポンス分解
    // $response_content = json_decode($response->getBody(), true, 512, 0);
    // トークンを返却
    return 'Login'; //$response_content['data'][0]['access_token'];
}

/**
 * GORAGetAPI
 */
function callGetAPI()
{
    // ログインAPIを実行してトークンを取得
    $token = callLoginAPI();
    // パラメータ生成
    $params = 'linkage_plan_ids=0553274689010021,0553274689010022,0553274689010023,0553274689010024';
    // リクエスト生成
    $request = new Request(
        'GET',
        'https://httpbin.org/get',
        ['Authorization' => 'Bearer ' . $token],
        json_encode(['query' => $params])
    );
    // クライアント生成
    $client = new Client(
        [
            'http_errors' => false,
            // 'base_uri' => 'https://httpbin.org'
        ]
    );
    // API実行
    $response = executeAPI($client, $request);
    // レスポンス分解
    // $response_content = json_decode($response->getBody(), true, 512, 0);
    //print_r($response_content);
    return 'GET'; // $response_content;
}


/**
 * APIを実行します。
 *
 * @param Client  $client  クライアント
 * @param Request $request リクエスト
 *
 * @return array status,message,contentで構成される連想配列
 */
function executeAPI($client, $request)
{
    // 呼び出し元の関数名を取得する
    $dbg = debug_backtrace();
    $calling_func_name = $dbg[1]['function'];

    // // INPUTログ生成
    // SiteControllerWorkerService::writeWebCooperationLog(
    //     __CLASS__ . '->' . $calling_func_name,
    //     "\n***INPUT***\n" . $request->getUri() . "\n"
    //         . $request->getMethod() . "\n"
    //         . var_export($request->getBody(), true),
    //     \Config::get('const.portal.gora'),
    //     SiteControllerWorkerService::LOG_TYPE_CODE_IO
    // );

    echo $request->getMethod();
    echo "\n";
    echo $request->getUri();
    echo "\n";
    print_r($request->getHeaders());
    echo "\n";
    var_dump(json_decode($request->getBody()), true);
    echo "\n";
    $body = json_decode($request->getBody(), true);
    $key = array_key_first($body);
    var_dump($key);
    echo "\n";

    // API実行
    $response = $client->request(
        $request->getMethod(),
        $request->getUri(),
        [
            'headers' => $request->getHeaders(),
            $key => $body[$key]
        ]
    );
    echo $response->getBody();
    // // OUTPUTログ生成
    // SiteControllerWorkerService::writeWebCooperationLog(
    //     __CLASS__ . '->' . $calling_func_name,
    //     "\n***OUTPUT***\n"
    //         . $response->getStatusCode() . "\n"
    //         . var_export(\GuzzleHttp\json_decode($response->getBody(), true), true),
    //     \Config::get('const.portal.gora'),
    //     SiteControllerWorkerService::LOG_TYPE_CODE_IO
    // );

    // rewindしないと、responseを再使用できない。
    // $response->getBody()->rewind();
    // レスポンスを分離して返却
    return 'Test'; //$response;
}


function Test()
{
    $body = [
        'json' => [
            'linkage_vendor_id' => 'aa',
            'linkage_golf_course_id' => 'sssss',
            'password' => 'asfsdfsdfsdfsdfsdfs'
        ]
    ];
    // リクエスト生成
    // print_r($body);
    // $encode = json_encode($body);
    // print_r(json_decode($encode,true));
    print_r($body);

    // リクエスト生成
    $request = new Request(
        'POST',
        'https://httpbin.org/post',
        ['Content-Type' => 'application/json']
    );
    // クライアント生成
    $client = new Client(['http_errors' => false]);
    $response = $client->request(
        $request->getMethod(),
        $request->getUri(),
        [
            'headers' => $request->getHeaders(),
            $body
        ]
    );
    echo $response->getBody();
}

try {
    // callGetAPI();
    $params = 'linkage_plan_ids=0553274689010021,0553274689010022,0553274689010023,0553274689010024';

    // echo http_build_query(['0553274689010021,0553274689010022,0553274689010023,0553274689010024'],'?',',');
    $get_request = new Request(
        'GET',
        'https://httpbin.org/get' . '?' . $params,
        ['Authorization' => 'Bearer ' . 121212]
    );

    var_dump(mb_strstr($get_request->getRequestTarget(), '?'));

    echo json_decode($get_request->getBody(), true) ?? "a";
    echo "\n";
    // クライアント生成
    $client = new Client(['http_errors' => false]);
    $response = $client->send($get_request);
    echo $response->getBody();
} catch (Exception $e) {
    echo json_encode($e);
}
