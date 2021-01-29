<?php
/**
 * GuzzleClientの最適化のためのサンプル
 *
 * @package PlanCooperationGORAWorkerService
 * @author s.ito <s.ito@alp-inc.jp>
 */

require 'vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Psr7\Request;

//  /**
//  * Web連携のプラン連携用サービスクラス
//  */
// class OptimisationGuzzleClient
// {
//     /**
//      * 一連のプラン作成API処理を実行します
//      *
//      * @param array     $params   APIパラメータ配列
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function callCreatePlanAPI($params, $web_link)
//     {
//         // ログイン処理実行
//         $login_response = $this->callLoginAPI($web_link);
//         // 200以外なら処理終了。レスポンスをそのまま返す
//         if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
//             return $login_response;
//         }
//         // Bearer文字列をヘッダーに登録したクライアント取得。
//         $client = $this->getHttpClient($login_response['message']);
//         // リトライ回数分APIを実行する
//         for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
//             // API実行
//             $response = $this->executeAPI(
//                 $client,
//                 $params,
//                 $web_link->ServiceURL.self::API_URI,
//                 'post'
//             );
//             // 200なら処理終了
//             if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
//                 break;
//             }
//         }
//         return $response;
//     }

//     /**
//      * 一連のプラン更新API処理を実行します
//      *
//      * @param array     $params   APIパラメータ配列
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function callUpdatePlanAPI($params, $web_link)
//     {
//         // ログイン処理実行
//         $login_response = $this->callLoginAPI($web_link);
//         // 200以外なら処理終了。レスポンスをそのまま返す
//         if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
//             return $login_response;
//         }
//         // Bearer文字列をヘッダーに登録したクライアント取得。
//         $client = $this->getHttpClient($login_response['message']);
//         // リトライ回数分APIを実行する
//         for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
//             // API実行
//             $response = $this->executeAPI(
//                 $client,
//                 $params,
//                 $web_link->ServiceURL.self::API_URI,
//                 'patch'
//             );
//             // 200なら処理終了
//             if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
//                 break;
//             }
//         }
//         return $response;
//     }

//     /**
//      * 一連のプラン削除API処理を実行します
//      *
//      * @param array     $params   APIパラメータ配列
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function callDeletePlanAPI($params, $web_link)
//     {
//         // ログイン処理実行
//         $login_response = $this->callLoginAPI($web_link);
//         // 200以外なら処理終了。レスポンスをそのまま返す
//         if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
//             return $login_response;
//         }
//         // Bearer文字列をヘッダーに登録したクライアント取得。
//         $client = $this->getHttpClient($login_response['message']);
//         // リトライ回数分APIを実行する
//         for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
//             // API実行
//             $response = $this->executeAPI(
//                 $client,
//                 $params,
//                 $web_link->ServiceURL.self::API_URI,
//                 'delete'
//             );
//             // 200なら処理終了
//             if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
//                 break;
//             }
//         }
//         return $response;
//     }

//     /**
//      * 一連のプラン取得API処理を実行します
//      *
//      * @param array     $params   APIパラメータ配列
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function callGetPlanAPI($params, $web_link)
//     {
//         // ログイン処理実行
//         $login_response = $this->callLoginAPI($web_link);
//         // 200以外なら処理終了。レスポンスをそのまま返す
//         if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
//             return $login_response;
//         }
//         // Bearer文字列をヘッダーに登録したクライアント取得。
//         $client = $this->getHttpClient($login_response['message']);
//         // リトライ回数分APIを実行する
//         for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
//             // API実行
//             $response = $this->executeAPI(
//                 $client,
//                 $params,
//                 $web_link->ServiceURL.self::API_URI,
//                 'get'
//             );
//             // 200なら処理終了
//             if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
//                 break;
//             }
//         }
//         return $response;
//     }

    
//     /**
//      * 一連のプラン取得API処理を実行します
//      *
//      * @param array     $params   APIパラメータ配列
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function sequenceAPI($params, $web_link, $method)
//     {
//         // ログイン処理実行
//         $login_response = $this->callLoginAPI($web_link);
//         // 200以外なら処理終了。レスポンスをそのまま返す
//         if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
//             return $login_response;
//         }
//         // Bearer文字列をヘッダーに登録したクライアント取得。
//         $client = $this->getHttpClient($login_response['message']);
//         // リトライ回数分APIを実行する
//         for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
//             // API実行
//             $response = $this->executeAPI(
//                 $client,
//                 $params,
//                 $web_link->ServiceURL.self::API_URI,
//                 'get'
//             );
//             // 200なら処理終了
//             if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
//                 break;
//             }
//         }
//         return $response;
//     }

//     /**
//      * 一連のログインAPI処理を実行します。
//      *
//      * @param TmWebLink $web_link Web連携マスタ
//      *
//      * @return array status,messageで構成される連想配列
//      */
//     private function callLoginAPI($web_link)
//     {
//         // ログインパラメータ生成
//         $params = [
//             'linkage_vendor_id' => $web_link->VendorCode,
//             'linkage_golf_course_id' => $web_link->LinkGolfCode,
//             'password' => $web_link->Password
//         ];
//         // ログイン用クライアント生成
//         $client = $this->getLoginHttpClient();
//         // ログインAPI実行
//         $login_response = $this->executeAPI(
//             $client,
//             $params,
//             $web_link->ServiceURL.self::LOGIN_API_URI,
//             'post'
//         );
//         // ログインがうまくいかなかったら処理を中断する。
//         if (!($login_response['status'] >= 200 && $login_response['status'] < 300)) {
//             return [
//                 'status' => $login_response['status'],
//                 'message' => self::FAILED_LOGIN
//             ];
//         }
//         // ログイン結果が200でもlinkage_targets配列の中に'plan'文字列がなければ403エラーとなってしまうので処理を中断する。
//         if (!in_array('plan', $login_response['content']['linkage_targets'], true)) {
//             return [
//                 'status' => self::HTTP_STATUS_CODE_FORBIDDEN,
//                 'message' => self::FORBIDDEN
//             ];
//         }
//         // Bearerトークンを抜き出して返却する
//         return [
//             'status' => self::HTTP_STATUS_CODE_OK,
//             'message' => $login_response['content']['access_token']
//         ];
//     }

//     /**
//      * HTTPクライアントを取得します
//      *
//      * @param string $bearer_token Bearer認証トークン
//      *
//      * @return \GuzzleHttp\Client
//      */
//     private function getHttpClient($bearer_token)
//     {
//         return new \GuzzleHttp\Client(
//             [
//                 'http_errors' => false,
//                 'headers' => [
//                     'Authorization' => 'Bearer '.$bearer_token
//                 ]
//             ]
//         );
//     }

//     /**
//      * HTTPクライアントを取得します
//      *
//      * @return \GuzzleHttp\Client
//      */
//     private function getLoginHttpClient()
//     {
//         return new \GuzzleHttp\Client(
//             [
//                 'http_errors' => false,
//                 'headers' => [
//                     'Content-Type' => 'application/json'
//                 ]
//             ]
//         );
//     }
    
//     /**
//      * APIを実行します。
//      *
//      * @param Client $client      クライアント
//      * @param array  $params      APIパラメータ
//      * @param string $url         サービス先URL
//      * @param string $method_name メソッド名
//      *
//      * @return array status,message,contentで構成される連想配列
//      */
//     private function executeAPI($client, $params, $url, $method_name)
//     {
//         // 呼び出し元の関数名を取得する
//         $dbg = debug_backtrace();
//         $calling_func_name = $dbg[1]['function'];
//         // // INPUTログ生成
//         // SiteControllerWorkerService::writeWebCooperationLog(
//         //     __CLASS__.'->'.$calling_func_name,
//         //     "\n***INPUT***\n".$url."\n"
//         //     .$method_name."\n"
//         //     .var_export($params, true),
//         //     \Config::get('const.portal.gora'),
//         //     SiteControllerWorkerService::LOG_TYPE_CODE_IO
//         // );
//         // queryかjsonか
//         $body_type = $method_name === 'delete' || $method_name === 'get'
//             ? 'query'
//             : 'json';
//         // API実行
//         $response = $client->request(
//             $method_name,
//             $url,
//             [$body_type => $params]
//         );
//         // // OUTPUTログ生成
//         // SiteControllerWorkerService::writeWebCooperationLog(
//         //     __CLASS__.'->'.$calling_func_name,
//         //     "\n***OUTPUT***\n"
//         //     .$response->getStatusCode()."\n"
//         //     .var_export(\GuzzleHttp\json_decode($response->getBody(), true), true),
//         //     \Config::get('const.portal.gora'),
//         //     SiteControllerWorkerService::LOG_TYPE_CODE_IO
//         // );
//         // rewindしないと、responseを再使用できない。
//         $response->getBody()->rewind();
//         // レスポンスを分離して返却
//         return $this->divideResponse($response);
//     }
// }


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
            'base_uri' => $base_url.'/',
            'headers' => [
                'Content-Type' => 'application/json'
            ]
        ]
    );
}

try {
    // GORALoginAPI
    $base_url = 'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/auth';
    $client = getLoginHttpClient($base_url);

    // 色々準備
    $method_name = 'post';
    $path = 'login';
    $body_type = 'json';
    $params = [
        'linkage_vendor_id' => '19',
        'linkage_golf_course_id' => '19910',
        'password' => 'miDlmkTSCvD37mwR5X8T'
    ];
    $payload = [$body_type => $params];

    // API実行
    $response = $client->request(
        $method_name,
        $path,
        $payload
    );
    // レスポンス分解
    //$response_content = json_decode($response->getBody(), true, 512, 0);
    //$bearer_token = $response_content['data'][0]['access_token'];

    // var_dump($url);
    // foreach ($request->getHeaders() as $name => $values) {
    //     echo $name . ': ' . implode(', ', $values) . "\r\n";
    // }
    $request = new Request('GET', 'http://httpbin.org');
    echo $request->getUri()->getScheme(); // http
    echo $request->getUri(); // http://httpbin.org
} catch (Exception $e) {
    echo json_encode($e);
}
