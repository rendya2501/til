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

/**
 * Web連携のプラン連携用サービスクラス
 */
class OptimisationGuzzleClient
{

    // DB参照エラーメッセージ
    const DB_SHOW_ERROR_MESSAGE_NOT_EXIST_MASTER_PLAN_DATA = '公開プランGORAマスタにデータが存在しません。';
    // DB更新エラーメッセージ
    const DB_UPDATE_ERROR_MESSAGE = '主要関連テーブルのレコード更新に失敗しました。';
    const DB_DELETE_ERROR_MESSAGE = '主要関連テーブルのレコード削除に失敗しました。';
    const DB_FAILDED_DELETE_TARGET_SITE = '公開プラン対象サイトマスタのレコード削除に失敗しました。';
    const DB_FAILDED_UPDATE_TARGET_SITE = '公開プラン対象サイトマスタのレコード更新に失敗しました。';
    const DB_FAILDED_CREATE_ERROR_INFO = '公開プラン連携エラー情報マスタのレコード作成に失敗しました。';
    const DB_FAILDED_UPDATE_ERROR_INFO = '公開プラン連携エラー情報マスタのレコード更新に失敗しました。';
    // HTTPステータスコード
    const HTTP_STATUS_CODE_OK = 200;
    const HTTP_STATUS_CODE_BAD_REQUEST = 400;
    const HTTP_STATUS_CODE_UNAUTHORIZED = 401;
    const HTTP_STATUS_CODE_FORBIDDEN = 403;
    const HTTP_STATUS_CODE_CONFLICT = 409;
    const HTTP_STATUS_CODE_UNPROCESSABLE_ENTITY = 422;
    const HTTP_STATUS_CODE_INTERNAL_ERROR = 500;
    const HTTP_STATUS_CODE_SERVICE_UNAVAILABLE = 503;
    // リトライカウント最大値
    const RETRY_MAX_COUNT = 5;
    //税率区分 0:通常
    const TAX_RATE_TYPE = 0;
    // 削除タイプ
    const DELETE_TYPE_LOGICAL = 0;
    const DELETE_TYPE_PHYSICAL = 1;
    // APIのURI
    const API_URI = 'plan';
    const LOGIN_API_URI = 'auth/login';
    // メッセージ類
    const FEE_NOT_FOUND = '料金が見つかりませんでした。';
    const FAILED_LOGIN = 'ログインに失敗しました';
    const FORBIDDEN = 'プラン連携フラグがOFFになっています。';
    const TOO_MANY_HYBRID_CONTENTS_ON_SAVEPLAN = '大量のプランが生成される恐れがあるため、予約受付期間終了日をプレー対象期間開始日より前に設定してください。';
    const TOO_MANY_HYBRID_CONTENTS_ON_SAVEFEE = '大量のプランが生成される恐れがあります。このプランの予約受付期間終了日をプレー対象期間開始日より前に設定してください。';
    const RES_START_TIME_GT_RES_END_TIME = '予約受付開始日を当日以降に設定してください。';
    /**
     * 一連のプラン作成API処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function callCreatePlanAPI($params, $web_link)
    {
        // ログイン処理実行
        $login_response = $this->callLoginAPI($web_link);
        // 200以外なら処理終了。レスポンスをそのまま返す
        if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
            return $login_response;
        }
        // クライアント生成
        $client = $this->createHttpClient();
        // リクエスト生成
        $request = new Request(
            'POST',
            $web_link->ServiceURL . self::API_URI,
            ['Authorization' => 'Bearer ' . $login_response['message']],
            json_encode($params)
        );
        // リトライ回数分APIを実行する
        for ($i = 0; $i < self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI($client, $request);
            // 200なら処理終了
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                break;
            }
        }
        return $response;
    }

    /**
     * 一連のプラン更新API処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function callUpdatePlanAPI($params, $web_link)
    {
        // ログイン処理実行
        $login_response = $this->callLoginAPI($web_link);
        // 200以外なら処理終了。レスポンスをそのまま返す
        if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
            return $login_response;
        }
        // クライアント生成
        $client = $this->createHttpClient();
        // リクエスト生成
        $request = new Request(
            'PATCH',
            $web_link->ServiceURL . self::API_URI,
            ['Authorization' => 'Bearer ' . $login_response['message']],
            json_encode($params)
        );
        // リトライ回数分APIを実行する
        for ($i = 0; $i < self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI($client, $request);
            // 200なら処理終了
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                break;
            }
        }
        return $response;
    }

    /**
     * 一連のプラン削除API処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function callDeletePlanAPI($params, $web_link)
    {
        // ログイン処理実行
        $login_response = $this->callLoginAPI($web_link);
        // 200以外なら処理終了。レスポンスをそのまま返す
        if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
            return $login_response;
        }
        // クライアント生成
        $client = $this->createHttpClient();
        // リクエスト生成
        $request = new Request(
            'DELETE',
            $web_link->ServiceURL . self::API_URI . '?linkage_plan_ids=' . $params,
            ['Authorization' => 'Bearer ' . $login_response['message']]
        );
        // リトライ回数分APIを実行する
        for ($i = 0; $i < self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI($client, $request);
            // 200なら処理終了
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                break;
            }
        }
        return $response;
    }

    /**
     * 一連のプラン取得API処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function callGetPlanAPI($params, $web_link)
    {
        // ログイン処理実行
        $login_response = $this->callLoginAPI($web_link);
        // 200以外なら処理終了。レスポンスをそのまま返す
        if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
            return $login_response;
        }
        // クライアント生成
        $client = $this->createHttpClient();
        // リクエスト生成
        $request = new Request(
            'GET',
            $web_link->ServiceURL . self::API_URI . '?linkage_plan_ids=' . $params,
            ['Authorization' => 'Bearer ' . $login_response['message']]
        );
        // リトライ回数分APIを実行する
        for ($i = 0; $i < self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI($client, $request);
            // 200なら処理終了
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                break;
            }
        }
        return $response;
    }

    /**
     * 一連のログインAPI処理を実行します。
     *
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,messageで構成される連想配列
     */
    private function callLoginAPI($web_link)
    {
        // ログインパラメータ生成
        $params = [
            'linkage_vendor_id' => $web_link->VendorCode,
            'linkage_golf_course_id' => $web_link->LinkGolfCode,
            'password' => $web_link->Password
        ];
        // リクエスト生成
        $request = new Request(
            'POST',
            $web_link->ServiceURL . self::LOGIN_API_URI,
            ['Content-Type' => 'application/json'],
            json_encode($params)
        );
        // クライアント生成
        $client = $this->createHttpClient();
        // ログインAPI実行
        $login_response = $this->executeAPI($client, $request);
        // ログインがうまくいかなかったら処理を中断する。
        if (!($login_response['status'] >= 200 && $login_response['status'] < 300)) {
            return [
                'status' => $login_response['status'],
                'message' => self::FAILED_LOGIN
            ];
        }
        // ログイン結果が200でもlinkage_targets配列の中に'plan'文字列がなければ403エラーとなってしまうので処理を中断する。
        if (!in_array('plan', $login_response['content']['linkage_targets'], true)) {
            return [
                'status' => self::HTTP_STATUS_CODE_FORBIDDEN,
                'message' => self::FORBIDDEN
            ];
        }
        // Bearerトークンを抜き出して返却する
        return [
            'status' => self::HTTP_STATUS_CODE_OK,
            'message' => $login_response['content']['access_token']
        ];
    }

    /**
     * HTTPクライアントを生成します
     *
     * @return \GuzzleHttp\Client
     */
    private function createHttpClient()
    {
        return new Client(
            [
                'http_errors' => false,
                'headers' => ['Content-Type' => 'application/json']
            ]
        );
    }

    /**
     * APIを実行します。
     *
     * @param Client  $client  クライアント
     * @param Request $request リクエスト
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function executeAPI($client, $request)
    {
        // 呼び出し元の関数名を取得する
        $dbg = debug_backtrace();
        $calling_func_name = $dbg[1]['function'];
        // INPUTログ生成
        SiteControllerWorkerService::writeWebCooperationLog(
            __CLASS__ . '->' . $calling_func_name,
            "\n***INPUT***\n" . $request->getUri() . "\n"
                . $request->getMethod() . "\n"
                . var_export($request->getBody(), true),
            \Config::get('const.portal.gora'),
            SiteControllerWorkerService::LOG_TYPE_CODE_IO
        );
        // API実行
        $response = $client->send($request);
        // OUTPUTログ生成
        SiteControllerWorkerService::writeWebCooperationLog(
            __CLASS__ . '->' . $calling_func_name,
            "\n***OUTPUT***\n"
                . $response->getStatusCode() . "\n"
                . var_export(\GuzzleHttp\json_decode($response->getBody(), true), true),
            \Config::get('const.portal.gora'),
            SiteControllerWorkerService::LOG_TYPE_CODE_IO
        );
        // rewindしないと、responseを再使用できない。
        $response->getBody()->rewind();
        // レスポンスを分離して返却
        return $this->divideResponse($response);
    }

    // /**
    //  * 一連のプラン取得API処理を実行します
    //  *
    //  * @param array     $params   APIパラメータ配列
    //  * @param TmWebLink $web_link Web連携マスタ
    //  *
    //  * @return array status,message,contentで構成される連想配列
    //  */
    // private function sequenceAPI($params, $web_link, $method)
    // {
    //     // ログイン処理実行
    //     $login_response = $this->callLoginAPI($web_link);
    //     // 200以外なら処理終了。レスポンスをそのまま返す
    //     if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
    //         return $login_response;
    //     }
    //     // Bearer文字列をヘッダーに登録したクライアント取得。
    //     $client = $this->createHttpClient($login_response['message']);
    //     // リトライ回数分APIを実行する
    //     for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
    //         // API実行
    //         $response = $this->executeAPI(
    //             $client,
    //             $params,
    //             $web_link->ServiceURL.self::API_URI,
    //             'get'
    //         );
    //         // 200なら処理終了
    //         if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
    //             break;
    //         }
    //     }
    //     return $response;
    // }

    // /**
    //  * HTTPクライアントを取得します
    //  *
    //  * @return \GuzzleHttp\Client
    //  */
    // private function getLoginHttpClient()
    // {
    //     return new \GuzzleHttp\Client(
    //         [
    //             'http_errors' => false,
    //             'headers' => [
    //                 'Content-Type' => 'application/json'
    //             ]
    //         ]
    //     );
    // }
}


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

try {
    // GORALoginAPI
    $base_url = 'https://stg.gateway-api.rakuten.co.jp/linkage/1.0/auth';
    $client = getLoginHttpClient($base_url);

    // 色々準備
    $method_name = 'post';
    $path = '/login';
    $payload = [
        'json' => [
            'linkage_vendor_id' => '19',
            'linkage_golf_course_id' => '19910',
            'password' => 'miDlmkTSCvD37mwR5X8T'
        ]
    ];

    // API実行
    $response = $client->request(
        $method_name,
        $path,
        $payload
    );
    // レスポンス分解
    //$response_content = json_decode($response->getBody(), true, 512, 0);
    //$bearer_token = $response_content['data'][0]['access_token'];
} catch (Exception $e) {
    echo json_encode($e);
}
