<?php

/**
 * ①ループ中でifで判定して追加
 * ②ループ中でifで判定して削除
 * ③ループ外で一括削除
 * どれが一番早いか調査したテストプログラム
 * 結果は②が一番早い。
 * 
 * 参考
 * ①:0.56209206581116秒
 * ②:0.34536194801331秒
 * ③:0.51715898513794秒
 */


const LOOP_COUNT = 999999;

/**
 * ループ中でifで判定して追加
 */
function LoopAddIf()
{
    for ($i = 1; $i <= LOOP_COUNT; $i++) {
        $web_plan_id = '0553999995000001' . str_pad($i, 6, '0', STR_PAD_LEFT);
        $params = ['type' => 'regular'];
        $params += ['linkage_plan_id' => $web_plan_id];
        if (false) {
            $params += ['point_grant_amount' => 0];
        }
        if (true) {
            $params += ['allowed_day_set' => 1];
        }
        $update_param['plans'][] = $params;
    }
}

/**
 * ループ中でifで判定して削除
 */
function LoopUnsetIf()
{
    for ($i = 1; $i <= LOOP_COUNT; $i++) {
        $web_plan_id = '0553999995000001' . str_pad($i, 6, '0', STR_PAD_LEFT);
        $params = [
            'type' => 'regular',
            'linkage_plan_id' => $web_plan_id,
            'point_grant_amount' => 0,
            'allowed_day_set' => 1
        ];
        if (true) {
            unset($params['point_grant_amount']);
        }
        if (false) {
            unset($params['allowed_day_set']);
        }
        $update_param['plans'][] = $params;
    }
}

/**
 * ループ外で一括削除
 */
function OuterLoopUnset()
{
    for ($i = 1; $i <= LOOP_COUNT; $i++) {
        $web_plan_id = '0553999995000001' . str_pad($i, 6, '0', STR_PAD_LEFT);
        $params = [
            'type' => 'regular',
            'linkage_plan_id' => $web_plan_id,
            'point_grant_amount' => 0,
            'allowed_day_set' => 1
        ];
        $update_param['plans'][] = $params;
    }
    array_walk(
        $update_param['plans'],
        function (&$v) {
            unset($v['point_grant_amount']);
        }
    );
}

$st = microtime(true);
LoopAddIf();
$now = microtime(true);
echo '①:' . ($now - $st) . "秒\n";

$st = microtime(true);
LoopUnsetIf();
$now = microtime(true);
echo '②:' . ($now - $st) . "秒\n";

$st = microtime(true);
OuterLoopUnset();
$now = microtime(true);
echo '③:' . ($now - $st) . "秒\n";
