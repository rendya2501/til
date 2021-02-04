<?php

/**
 * GuzzleClient
 *
 * @package PlanCooperationGORAWorkerService
 * @author s.ito <s.ito@alp-inc.jp>
 */

require 'vendor/autoload.php';

use GuzzleHttp\Client;

/**
 * GORALoginAPI
 */
function callLoginAPI()
{
    // パラメータ生成
    $params = [
        'linkage_vendor_id' => '19',
        'linkage_golf_course_id' => '19910',
        'password' => 'miDlmkTSCvD37mwR5X8T'
    ];
    // リクエスト生成
    $request = new Request(
        'POST',
        'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/auth/login',
        ['Content-Type' => 'application/json'],
        ['json' => $params]
    );    
    // クライアント生成
    $client = new Client(['http_errors' => false]);
    // API実行
    $response = $client->request(
        $request->method,
        $request->uri,
        $request->body
    );

    // レスポンス分解
    $response_content = json_decode($response->getBody(), true, 512, 0);
    var_dump($response_content);
    // トークンを返却
    return $response_content['data'][0]['access_token'];
}

try {
    // ログインAPIを実行してトークンを取得
    $token = callLoginAPI();
    // // リクエスト生成
    // $request = new Request(
    //     'GET',
    //     'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/plan?linkage_plan_ids=0553274689010021,0553274689010022,0553274689010023,0553274689010024',
    //     ['Authorization' => 'Bearer ' . $token]
    // );
    // // クライアント生成
    // $client = new Client(['http_errors' => false]);
    // // API実行
    // $response = $client->request($request->method, $request->uri, $request->headers, $request->body);
    // // レスポンス分解
    // $response_content = json_decode($response->getBody(), true, 512, 0);
    // print_r($response_content);
} catch (Exception $e) {
    echo json_encode($e);
}


class Request
{
    public $method;
    public $uri;
    public $headers = [];
    public $body = null;

    /**
     * コンストラクタ
     * 
     * @param string                               $method  HTTP method
     * @param string|UriInterface                  $uri     URI
     * @param array                                $headers Request headers
     * @param string|null|resource|StreamInterface $body    Request body
     */
    public function __construct(
        $method,
        $uri,
        array $headers = [],
        $body = null
    ) {
        $this->method = strtoupper($method);
        $this->uri = $uri;
        $this->headers = $headers;
        $this->body = $body;
    }
}
