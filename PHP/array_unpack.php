<?php

// 4つの引き数を受け取る関数に
function getOpenPlanPrice(
    $golf_code,
    $plan_code,
    $open_plan_code,
    $seq
){
    var_dump($golf_code);
    var_dump($plan_code);
    var_dump($open_plan_code);
    var_dump($seq);
}

// 4つの連想配列を返す関数
function rebateMainKeyFromLinkagePlanID($linkage_plan_id) {
    return [
        'golf_code' => (int)substr($linkage_plan_id, 0, 4),
        'plan_code' => (int)substr($linkage_plan_id, 4, 6),
        'open_plan_code' => (int)substr($linkage_plan_id, 10, 6),
        'seq' => (int)substr($linkage_plan_id, -4)
    ];
}

$linkage_plan_id = '05539999950000020005';

// これでもいける。
// call_user_func_array(
//     'getOpenPlanPrice',
//     rebateMainKeyFromLinkagePlanID($linkage_plan_id)
// );

// 4つの連想配列を4つの引き数にセットしたい→[...]キーワードによる引数のアンパックを使う
// https://stackoverflow.com/questions/40663687/cannot-unpack-array-with-string-keys
getOpenPlanPrice(...rebateMainKeyFromLinkagePlanID($linkage_plan_id));
// Ver7.0まではarray_valuesを挟む必要がある模様。
getOpenPlanPrice(...array_values(rebateMainKeyFromLinkagePlanID($linkage_plan_id)));
?>
