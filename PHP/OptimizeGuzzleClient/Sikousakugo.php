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
        // Bearer文字列をヘッダーに登録したクライアント取得。
        $client = $this->getHttpClient($login_response['message']);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI(
                $client,
                $params,
                $web_link->ServiceURL.self::API_URI,
                'post'
            );
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
        // Bearer文字列をヘッダーに登録したクライアント取得。
        $client = $this->getHttpClient($login_response['message']);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI(
                $client,
                $params,
                $web_link->ServiceURL.self::API_URI,
                'patch'
            );
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
        // Bearer文字列をヘッダーに登録したクライアント取得。
        $client = $this->getHttpClient($login_response['message']);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI(
                $client,
                $params,
                $web_link->ServiceURL.self::API_URI,
                'delete'
            );
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
        // Bearer文字列をヘッダーに登録したクライアント取得。
        $client = $this->getHttpClient($login_response['message']);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI(
                $client,
                $params,
                $web_link->ServiceURL.self::API_URI,
                'get'
            );
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
    private function callCreatePlanAPI2($params, $web_link)
    {
        $request = new Request(
            'GET',
            $web_link->ServiceURL.self::API_URI,
            'Authorization' => 'Bearer '.$bearer_token,
            json_encode($params)
        );
        return $this->sequenceAPI($request, $web_link);
    }

    /**
     * 一連のプラン取得API処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function callGetPlanAPI2($params, $web_link)
    {
        $request = new Request(
            'POST',
            $web_link->ServiceURL.self::API_URI,
            'Authorization' => 'Bearer '.$bearer_token,
            json_encode($params)
        );
        return $this->sequenceAPI($request, $web_link);
    }

    /**
     * 一連のAPI処理を実行します
     *
     * @param array     $params   APIパラメータ配列
     * @param TmWebLink $web_link Web連携マスタ
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function sequenceAPI($request, $web_link)
    {
        $dbg = debug_backtrace();
        $calling_func_name = $dbg[1]['function'];

        // ログイン処理実行
        $login_response = $this->callLoginAPI($web_link);
        // 200以外なら処理終了。レスポンスをそのまま返す
        if ($login_response['status'] !== self::HTTP_STATUS_CODE_OK) {
            return $login_response;
        }
        // Bearer文字列をヘッダーに登録したクライアント取得。
        $client = $this->getHttpClient($login_response['message']);

        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->executeAPI(
                $client,
                $request,
                $calling_func_name
            );
            // 200なら処理終了
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                break;
            }
        }
        return $response;
    }

    /**
     * APIを実行します。
     *
     * @param Client $client      クライアント
     * @param array  $params      APIパラメータ
     * @param string $url         サービス先URL
     * @param string $method_name メソッド名
     *
     * @return array status,message,contentで構成される連想配列
     */
    function executeAPI($client, $request, $calling_func_name = null)
    {
        // 呼び出し元の関数名を取得する
        if ($calling_func_name == null) {
            $dbg = debug_backtrace();
            $calling_func_name = $dbg[1]['function'];
        }

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
}
