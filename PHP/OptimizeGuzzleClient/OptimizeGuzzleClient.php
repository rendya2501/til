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
        // リクエスト生成
        $request = new Request(
            'POST',
            $web_link->ServiceURL . self::API_URI,
            [
                'Authorization' => 'Bearer '.$login_response['message'],
                'Content-Type' => 'application/json'
            ],
            json_encode($params)
        );
        // クライアント生成
        $client = new Client(['http_errors' => false]);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->execCommonAPI($client, $request);
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
        // リクエスト生成
        $request = new Request(
            'PATCH',
            $web_link->ServiceURL . self::API_URI,
            [
                'Authorization' => 'Bearer '.$login_response['message'],
                'Content-Type' => 'application/json'
            ],
            json_encode($params)
        );
        // クライアント生成
        $client = new Client(['http_errors' => false]);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->execCommonAPI($client, $request);
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
     * @param string    $params   APIパラメータ文字列
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
        // リクエスト生成
        $request = new Request(
            'DELETE',
            $web_link->ServiceURL . self::API_URI . '?linkage_plan_ids=' . $params,
            ['Authorization' => 'Bearer '.$login_response['message']]
        );
        // クライアント生成
        $client = new Client(['http_errors' => false]);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->execCommonAPI($client, $request);
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
     * @param string    $params   APIパラメータ文字列
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
        // リクエスト生成
        $request = new Request(
            'GET',
            $web_link->ServiceURL . self::API_URI . '?linkage_plan_ids=' . $params,
            ['Authorization' => 'Bearer '.$login_response['message']]
        );
        // クライアント生成
        $client = new Client(['http_errors' => false]);
        // リトライ回数分APIを実行する
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // API実行
            $response = $this->execCommonAPI($client, $request);
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
        $client = new Client(['http_errors' => false]);
        // ログインAPI実行
        $login_response = $this->execCommonAPI($client, $request);
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
     * 共通API実行処理
     *
     * @param Client  $client  クライアントクラス
     * @param Request $request リクエストクラス
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function execCommonAPI($client, $request)
    {
        // 呼び出し元の関数名を取得する
        $dbg = debug_backtrace();
        $calling_func_name = $dbg[1]['function'];
        // INPUTログ生成
        SiteControllerWorkerService::writeWebCooperationLog(
            __CLASS__.'->'.$calling_func_name,
            "\n***INPUT***\n".$request->getUri()."\n"
            .$request->getMethod()."\n"
            .var_export(
                json_decode($request->getBody(), true)
                ?? mb_strstr($request->getRequestTarget(), '?'),
                true
            ),
            \Config::get('const.portal.gora'),
            SiteControllerWorkerService::LOG_TYPE_CODE_IO
        );
        // API実行
        $response = $client->send($request);
        // OUTPUTログ生成
        SiteControllerWorkerService::writeWebCooperationLog(
            __CLASS__.'->'.$calling_func_name,
            "\n***OUTPUT***\n"
            .$response->getStatusCode()."\n"
            .var_export(\GuzzleHttp\json_decode($response->getBody(), true), true),
            \Config::get('const.portal.gora'),
            SiteControllerWorkerService::LOG_TYPE_CODE_IO
        );
        // rewindしないと、responseを再使用できない。
        $response->getBody()->rewind();
        // レスポンスを適切に分離して返却
        return $this->divideResponse($response);
    }

    /**
     * レスポンスにステータス等を割り振る
     *
     * @param \Psr\Http\Message\ResponseInterface $response レスポンス内容
     *
     * @return array status,message,contentで構成される連想配列
     */
    private function divideResponse($response)
    {
        $status = $response->getStatusCode();
        $message = '';
        $content = null;

        try {
            $response_content = null;
            $response_body = $response->getBody()->getContents() ?? null;
            if (!empty($response_body)) {
                $response_content = \GuzzleHttp\json_decode($response_body, true);
            }
    
            switch ($status) {
                case self::HTTP_STATUS_CODE_OK:
                    // 成功
                    $content = $response_content['data'][0];
                    break;
                case self::HTTP_STATUS_CODE_BAD_REQUEST:
                    // バリデーションエラー
                    foreach ($response_content as $item) {
                        switch ($item['error_code']) {
                            case 400:
                                switch ($item['message']) {
                                    case 'should be present and not empty if other specified fields with specified value are present':
                                        $message .= '指定された値を持つ他の指定されたフィールドが存在する場合は入力必須です。'."\n";
                                        break;
                                    default:
                                        $message = $item['message'] ?? '';
                                        break;
                                }
                                break;
                            case 4013:
                                $message .= sprintf('"%s"で指定された項目の長さが最大最小の範囲外です。'."\n", $item['field']);
                                break;
                            case 4014:
                                $message .= sprintf('"%s"で指定された項目の大きさが最大最小の範囲外です。'."\n", $item['field']);
                                break;
                            case 4027:
                                $message .= sprintf('"%s"で指定された項目は空欄を受け付けていません。'."\n", $item['field']);
                                break;
                            case 4029:
                                switch ($item['message']) {
                                    case 'all datetime should be in the available list':
                                        $message .= '全ての日付時刻が利用可能なリストに収まっている必要があります。'."\n";
                                        break;
                                    case 'value(s) have to be one of available values:':
                                    case 'value have to be one of available values:':
                                        $message .= sprintf('"%s"で指定された項目は利用可能な値のうちの1つでなければなりません。'."\n", $item['field']);
                                        break;
                                    case 'date time must be in available list if other specified fields with specified value are present':
                                        $message .= '指定された値を持つ他の指定されたフィールドが存在する場合、日付時刻が利用可能なリストに収まっている必要があります。'."\n";
                                        break;
                                    default:
                                        $message = $item['message'] ?? '';
                                        break;
                                }
                                break;
                            case 4034:
                                $message .= sprintf('"%s"で指定された項目の大きさを最大値以下にしてください。'."\n", $item['field']);
                                break;
                            case 4040:
                                $message .= sprintf('"%s"で指定された項目の大きさを最小値以上にしてください。'."\n", $item['field']);
                                break;
                            case 4048:
                                $message .= sprintf('"%s"で指定された項目は入力必須です。'."\n", $item['field']);
                                break;
                            case 4049:
                                $message .= '指定された値を持つ他の指定されたフィールドが存在する場合は入力必須です。'."\n";
                                break;
                            case 4051:
                                $message .= '他の指定されたフィールドが存在する場合は入力必須です。'."\n";
                                break;
                            case 4055:
                                $message .= '他のフィールドと同じ値でなければなりません。'."\n";
                                break;
                            case 4079:
                                $message .= sprintf('期間の開始終了や範囲の最大最小の関係が適切でありません。"%s"で指定された項目の値を確認してください。 '."\n", $item['field']);
                                break;
                            case 4080:
                                $message .= 'カート種類もしくはラウンドが複数指定されています。それぞれ１つのみ指定してください。'."\n";
                                break;
                            default:
                                $message = $item['message'] ?? '';
                                break;
                        }
                    }
                    break;
                case self::HTTP_STATUS_CODE_UNAUTHORIZED:
                    // 認証エラー
                    $message = $response_content['message'] ?? '';
                    break;
                case self::HTTP_STATUS_CODE_FORBIDDEN:
                    // 権限エラー
                    $message = $response_content['message'] ?? '';
                    break;
                case self::HTTP_STATUS_CODE_CONFLICT:
                    // キー重複エラー
                    $message .= '同じ連携プランIDがすでに存在し競合しています。連携プランIDを変更してください。'."\n";
                    break;
                case self::HTTP_STATUS_CODE_UNPROCESSABLE_ENTITY:
                    // 処理できないエンティティ。論理的にデータに問題があります。
                    switch ($response_content['error_code']) {
                        case '4500_CMN001':
                            $message .= '同じ連携プランIDがすでに存在し競合しています。連携プランIDを変更してください。'."\n";
                            break;
                        case '4508_PKG001':
                            // 'msg_attr'の場合もある模様。それでエラーになってしまったので判定処理を追加
                            // 観測できているものはとりあえず登録していく。
                            if (!array_key_exists('msg_attr1', $response_content['message_attributes'])) {
                                switch ($response_content['message_attributes']['msg_attr']) {
                                    case 'plan_aspects':
                                        $message .= '多形内容の時間設定が重複しています。多形内容の時間設定を確認してください。'."\n";
                                        break;
                                    default:
                                        if ($response_content['message']) {
                                            $message = $response_content['message']
                                                . ' : '
                                                . reset($response_content['message_attributes']);
                                        }
                                        break;
                                }
                                break;
                            }
                            switch ($response_content['message_attributes']['msg_attr1']) {
                                case 'basis_reception_start_at after basis_play_date_start':
                                    $message .= '基本予約受付開始日時が基本プレー日開始日より後に設定されています。基本プレー日開始日より前に設定してください。'."\n";
                                    break;
                                case 'basis_reception_end_at after basis_play_date_end':
                                    $message .= '基本予約受付終了日時が基本プレー日終了日より後に設定されています。基本プレー日終了日より前に設定してください。'."\n";
                                    break;
                                case 'basis_play_date_start before current time':
                                    $message .= '基本プレー日開始日が現在の時刻より前に設定されています。現在の時刻より後に設定してください。'."\n";
                                    break;
                                case 'basis_play_date_end after current date + 6 months':
                                    $message .= '基本予約受付終了日時が現在の時刻から6か月後より後に設定されています。基本予約受付終了日時は現在時刻から6か月以内に設定してください。'."\n";
                                    break;
                                case 'course_index':
                                    $message .= 'コースコードが不正です。'."\n";
                                    break;
                                case 'reception_start_at':
                                    $message .= '更新時に基本予約受付開始日時が現在の時間より前、かつ基本予約受付終了日時が基本プレー日終了日の23:59:59に設定されていません。基本予約受付日時を確認してください。'."\n";
                                    break;
                                case 'plan_aspect':
                                    $message .= '多形内容の時間設定が重複しています。多形内容の時間設定を確認してください。'."\n";
                                    break;
                                case 'tee_time_auto_link_settings':
                                    $message .= '自動公開設定リストの開始時間設定が重複しています。自動公開設定リストを確認してください。'."\n";
                                    break;
                                case 'sum of hybrid contents is over 100':
                                    $message .= '多形内容の数が100個を超えています。100個以下にしてください。'."\n";
                                    break;
                                default:
                                    if ($response_content['message']) {
                                        $message = $response_content['message']
                                            . ' : '
                                            . reset($response_content['message_attributes']);
                                    }
                                    break;
                            }
                            break;
                        case '4508_PKG004':
                            // 'msg_attr'の場合もある模様。それでエラーになってしまったので判定処理を追加
                            // 'msg_attr1'がない場合、とりあえず内容をそのまま表示する
                            if (!array_key_exists('msg_attr1', $response_content['message_attributes'])) {
                                if ($response_content['message']) {
                                    $message = $response_content['message']
                                        . ' : '
                                        . reset($response_content['message_attributes']);
                                }
                                break;
                            }
                            switch ($response_content['message_attributes']['msg_attr1']) {
                                case 'has_cancel_auto_revive':
                                    $message .= 'このゴルフ場はキャンセル枠自動復活をしないよう設定されていますが、このプランはキャンセル枠自動復活するよう設定されています。ゴルフ場の設定に合わせてください。'."\n";
                                    break;
                                case 'has_day_before_reception':
                                    $message .= 'このゴルフ場は予約受付締切日数が設定されていませんが、このプランは土日祝日予約受付開始日数および平日予約受付開始日数が空欄ではありません。予約受付開始日数を空欄にしてください。'."\n";
                                    break;
                                case 'allow_coupon_use':
                                    $message .= 'このゴルフ場はクーポン利用不可に設定されていますが、このプランはクーポン利用可に設定されています。ゴルフ場の設定に合わせてください。'."\n";
                                    break;
                                case 'allow_point_use':
                                    $message .= 'このゴルフ場はポイント利用不可に設定されていますが、このプランはポイント利用可に設定されています。ゴルフ場の設定に合わせてください。'."\n";
                                    break;
                                case 'has_point_grant':
                                    $message .= 'このゴルフ場はポイント付与なしに設定されていますが、このプランはポイント付与ありに設定されています。ゴルフ場の設定に合わせてください。'."\n";
                                    break;
                                case 'can_transfer':
                                    $message .= 'このゴルフ場は送迎不可に設定されていますが、このプランのプレースタイルに送迎クラブ（クラブバス）が含まれています。ゴルフ場の設定に合わせてください。'."\n";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            if ($response_content['message']) {
                                $message = $response_content['message']
                                    . ' : '
                                    . reset($response_content['message_attributes']);
                            }
                            break;
                    }
                    break;
                case self::HTTP_STATUS_CODE_INTERNAL_ERROR:
                    // サーバ内エラー
                    if (array_key_exists('details', $response_content)) {
                        foreach ($response_content['details']['message'] as $item) {
                            $message .= ($item ?? '')."\n";
                        }
                    } else {
                        $message = ($response_content['message'] ?? '')."\n";
                    }
                    break;
                case self::HTTP_STATUS_CODE_SERVICE_UNAVAILABLE:
                    // サービス利用不可
                    $message = $response_content['message'] ?? '';
                    break;
                default:
                    // その他、来ないはずだが念の為
                    $message = $response_content['message'] ?? '';
                    break;
            }
        } catch (\Throwable $th) {
            $message = 'GORAで異常が発生しました。';
        } finally {
            return ['status'=>$status, 'message'=>$message, 'content'=>$content];
        }
    }
}

try {

} catch (Exception $e) {
    echo json_encode($e);
}
