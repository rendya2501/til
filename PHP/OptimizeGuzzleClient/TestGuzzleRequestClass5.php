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

// APIのURI
const API_URI = '/plan';
const LOGIN_API_URI = '/auth/login';
const SERVICE_URL = 'https://stg.gateway-api.rakuten.co.jp/linkage/1.0';

/**
 * GORALoginAPI
 */
function callLoginAPI()
{
    // パラメータ生成
    $params = [
        "linkage_vendor_id" => "19",
        "linkage_golf_course_id" => "19910",
        "password" => "miDlmkTSCvD37mwR5X8T"
    ];
    // リクエスト生成
    $request = new Request(
        'POST',
        SERVICE_URL . LOGIN_API_URI,
        ['Content-Type' => 'application/json'],
        json_encode($params)
    );
    // クライアント生成
    $client = new Client(['http_errors' => false]);
    // API実行
    $response = executeAPI($client, $request);
    // トークンを返却
    return $response['data'][0]['access_token'];
}

/**
 * GORAGetAPI
 */
function callGetAPI()
{
    // パラメータ生成
    $params = 'linkage_plan_ids=0553274689010021,0553274689010022,0553274689010023,0553274689010024';
    // リクエスト生成コールバック生成
    $createRequest = function ($token) use ($params) {
        return new Request(
            'GET',
            SERVICE_URL . API_URI . '?' . $params,
            ['Authorization' => 'Bearer ' . $token]
        );
    };
    // API実行
    return commonAPIAction($createRequest);
}

/**
 * 一連のAPI実行処理
 */
function commonAPIAction(callable $createRequest)
{
    // ログインAPIを実行してトークンを取得
    $token = callLoginAPI();
    // リクエスト生成
    $request = $createRequest($token);
    // クライアント生成
    $client = new Client(
        [
            'http_errors' => false,
            'function' => 'functionfunction'
        ]
    );
    var_dump($client->getConfig('function'));
    var_dump($client);
    // API実行
    $response = executeAPI($client, $request);
    return $response;
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
    echo 'call_f : ' . $calling_func_name . "\n";
    echo 'method : ' . $request->getMethod() . "\n";
    echo 'uri    : ' . $request->getUri() . "\n";
    echo 'header : ' . print_r($request->getHeaders(), true) . "\n";
    echo 'body   : ' . print_r(json_decode($request->getBody(), true), true) . "\n";
    echo 'query  : ' . print_r(mb_strstr($request->getRequestTarget(), '?'), true) . "\n";
    echo "\n";

    // API実行
    $response = $client->send($request);

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
    return json_decode($response->getBody(), true);
}


try {
    callGetAPI();
} catch (Exception $e) {
    echo json_encode($e);
}
