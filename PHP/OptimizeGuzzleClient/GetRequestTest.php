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
 * HTTPクライアントを取得します
 *
 * @return \GuzzleHttp\Client
 */
function getLoginHttpClient($base_url)
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

function callLoginAPI()
{
    // GORALoginAPI
    // $base_url = 'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/auth';
    // $method_name = 'post';
    // $path = 'login';
    // $body_type = 'json';
    $params = [
        'linkage_vendor_id' => '19',
        'linkage_golf_course_id' => '19910',
        'password' => 'miDlmkTSCvD37mwR5X8T'
    ];
    // $payload = [$body_type => $params];

    $request = new Request(
        'POST',
        'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/auth/login',
        ['Content-Type' => 'application/json'],
        json_encode($params)
    );
    $client = new Client(['http_errors' => false]);

    // API実行
    $response = $client->send($request);
    // レスポンス分解
    $response_content = json_decode($response->getBody(), true, 512, 0);
    return $response_content['data'][0]['access_token'];
}

try {
    $token = callLoginAPI();

    $request = new Request(
        'GET',
        'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/plan?linkage_plan_ids=0553274689010021,0553274689010022,0553274689010023,0553274689010024',
        ['Authorization' => 'Bearer '.$token]
    );
    $client = new Client(['http_errors' => false]);

    // API実行
    $response = $client->send($request);
    // レスポンス分解
    $response_content = json_decode($response->getBody(), true, 512, 0);
    print_r($response_content);
} catch (Exception $e) {
    echo json_encode($e);
}
