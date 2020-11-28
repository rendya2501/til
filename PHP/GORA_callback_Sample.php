<?php

class PlanCooperationGORAWorkerService
{
    const HTTP_STATUS_CODE_OK = 200;
    const RETRY_MAX_COUNT = 5;
        
    private function updateOpenPlanTargetSiteMaster($aaaa, $bbbb)
    {
        echo "yattaze". $aaaa. $bbbb;
    }
        
    private function failFunc()
    {
        echo "sippai";
    }
    
    public function savePlan() 
    {
        $success_callback_info = [
            'func' => 'updateOpenPlanTargetSiteMaster',
            'param' => [
                'Test',
                'Test2'
            ]
        ];
        
        $response = $this->execAPI(
            $success_callback_info,
            'failFunc'
        ); 
    }

    /**
     * API実行の共通処理
     *
     * @param array $successful_callback API成功時コールバック情報[関数名,パラメータ]
     * @param array $failed_callback     API失敗時コールバック情報[関数名,パラメータ]
     *
     * @return array レスポンス
     */
    private function execAPI(
        $successful_callback = null,
        $failed_callback = null
    ) {
        // ログインAPI実行
        // 403判定等

        // 成否判定フラグ
        $is_success = false;
        // 指定回数リトライ
        for ($i=0; $i<self::RETRY_MAX_COUNT; $i++) {
            // ログインAPI実行
            $response['status'] = 200;
            // API実行成功
            if ($response['status'] === self::HTTP_STATUS_CODE_OK) {
                // 成功のコールバックがあるなら実行する
                if ($successful_callback) {
                    call_user_func_array(
                        array($this, $successful_callback['func_name']),
                        $successful_callback['param']
                    );
                }
                $is_success = true;
                break;
            }
        }
        // リトライ全て失敗した場合
        if (!$is_success) {
            // 失敗のコールバックがあるなら実行する
            if ($failed_callback) {
                call_user_func_array(
                    array($this, $failed_callback['func_name']),
                    $failed_callback['param']
                );
            }
        }
        // レスポンスをそのまま返却
        return $response;
    }
}


$obj = new PlanCooperationGORAWorkerService();
$obj->savePlan();
