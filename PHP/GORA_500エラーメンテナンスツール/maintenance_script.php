<?php

// 1.エントリーチェック

// 引数チェック
if ($argc != 2) {
    print $argc;
    print "引数が不正です。\n";
    exit;
}
// 解析対象ファイル名取得
$filename = $argv[1];


// 2.ログ解析
// 観測行
$row_cnt = 1;
$hit_row_count_array = [];
// 観測文字列
$str = '';
$result_array = [];
// 観測判定フラグ
$flag1 = false;
$flag2 = false;
$flag3 = false;
$flag4 = false;
$flag5 = false;
$flag6 = false;
$flag7 = false;
$flag8 = false;

// gzファイルオープン
$zh = gzopen($filename, "rb");
if ($zh == false) {
    print "指定ファイルのオープンに失敗しました。\n";
    exit;
}
// 全行ループ
while (gzeof($zh) == false) {
    // 1行取得
    $line = gzgets($zh);
    $row_cnt += 1;
    $hit_flag = false;

    // 各文字列を観測したらフラグを上げていく。
    // 途中で別の文字列を観測したらリセットする。
    if (!$flag1 && strstr($line, '500') != false) {
        $hit_flag = true;
        $flag1  = true;
    }
    if ($flag1 && strstr($line, 'array (') != false) {
        $hit_flag = true;
        $flag2  = true;
    }
    if ($flag2 && strstr($line, "  'message' => 'Internal Server Error',") != false) {
        $hit_flag = true;
        $flag3  = true;
    }
    if ($flag3 && strstr($line, ')') != false) {
        $hit_flag = true;
        $flag4  = true;
    }
    if ($flag4 && strstr($line, 'App\Services\WebCooperation\PlanCooperationGORAWorkerService->callCreatePlanAPI') != false) {
        $hit_flag = true;
        $flag5  = true;
    }
    if ($flag5 && strstr($line, '***INPUT***') != false) {
        $hit_flag = true;
        $flag6  = true;
    }
    if ($flag6 && strstr($line, 'https://gateway-api.golf.rakuten.co.jp/linkage/1.0/plan') != false) {
        $hit_flag = true;
        $flag7  = true;
    }
    if ($flag7 && strstr($line, 'POST') != false) {
        $hit_flag = true;
        $flag8  = true;
        // このタイミングで観測行を取得する
        $hit_row_count_array[] = $row_cnt;
    }
    if ($flag8 && strstr($line, 'POST') == false) {
        // 結果を取得し続けるためフラグを上げ続ける
        $hit_flag = true;
        // 次のログが始まったら観測を終了して結果に格納する
        if (strstr($line, 'App\Services\WebCooperation\PlanCooperationGORAWorkerService->callCreatePlanAPI') != false) {
            $result_array[] = $str;
            $flag1 = false;
            $flag2 = false;
            $flag3 = false;
            $flag4 = false;
            $flag5 = false;
            $flag6 = false;
            $flag7 = false;
            $flag8 = false;
            $str = '';
            continue;
        }
        $str .= $line;
    }
    // 1回でもヒットしなかったら全てリセット
    if (!$hit_flag) {
        $flag1 = false;
        $flag2 = false;
        $flag3 = false;
        $flag4 = false;
        $flag5 = false;
        $flag6 = false;
        $flag7 = false;
        $flag8 = false;
    }
}
//gzファイルクローズ
gzclose($zh);


// 3.結果発表
print "\n";
if (count($hit_row_count_array) == 0) {
    print "1件も観測できませんでした。\n";
    exit;
} else {
    foreach ($hit_row_count_array as $hit_row_count) {
        print $hit_row_count . "行目からのパラメータを取得しました。\n";
    }
}


// 4.結果を一時ファイルに出力
// 検索結果出力ファイル名
$dfilename = preg_replace("/\.log\.gz$/", "", $filename) . "_.php";
// 出力ファイルオープン
$fp = fopen($dfilename, "wb");
if ($fp == false) {
    print "出力ファイルのオープンに失敗しました。\n";
    exit;
}
// 出力内容生成
$php = '<?php' . "\n";
foreach ($result_array as $key => $value) {
    $php .= 'function getArray' . ((int)$key + 1) . '(){' . "\n";
    $php .= '    return ' . $value . ';' . "\n";
    $php .= '}';
}
// ファイル出力
fputs($fp, json_decode(json_encode($php), true));
//出力ファイルクローズ
fclose($fp);


// 5.一時関数ファイルから配列を生成
// 指定ファイル読み込み
require $dfilename;
// 配列の生成
$source_array = [];
foreach ($result_array as $key => $value) {
    $funcname = "getArray" . ((int)$key + 1);
    $source_array[] = $funcname();
}


// 6.SQL生成
$result = '';
foreach ($source_array as $key => $source) {
    // 基本情報取得
    $golf_code = (int)substr($source['plans'][0]['linkage_plan_id'], 0, 4);
    $plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 4, 6);
    $open_plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 10, 6);
    $web_link_code = $golf_code . '3';
    $linkage_plan_id = substr($source['plans'][0]['linkage_plan_id'], 0, 16);

    $result .= '-- ' . ((int)$key + 1) . '番目のクエリ' . "\n";
    // UPDATE文
    $result .= "UPDATE TmOpenPlanTargetSite SET WebPlanID='" . $linkage_plan_id . "',LinkFlag=1 \n";
    $result .= "WHERE GolfCode=" . $golf_code . " AND PlanCode=" . $plan_code . " AND OpenPlanCode=" . $open_plan_code . " AND WebLinkCode=" . $web_link_code;
    $result .= "\n\n";

    // INSERT文
    foreach ($source['plans'] as $value) {
        $result .= 'INSERT INTO TmOpenPlanGORAPRice VALUES(';
        // GolfCode
        $result .= $golf_code . ",";
        // PlanCode
        $result .= $plan_code . ",";
        // OpenPlanCode
        $result .= $open_plan_code . ",";
        // Seq
        $result .= (int)substr($value['linkage_plan_id'], -4) . ",";
        // PlayFee→base_price
        $result .= $value['basis_content']['base_price'] . ",";
        // Tax→sales_tax
        $result .= $value['basis_content']['sales_tax'] . ",";
        // AdditionPrice→additional_fee
        $result .= $value['basis_content']['additional_fee'] . ",";
        // GolfUseTax→golf_use_tax
        $result .= $value['basis_content']['golf_usage_tax'] . ",";
        // OtherTax→other_tax
        $result .= $value['basis_content']['other_tax'] . ",";
        // 3BPrice→3b_additional_fee
        $result .= $value['basis_content']['3b_additional_fee'] . ",";
        // 2BPrice→2b_additional_fee
        $result .= $value['basis_content']['2b_additional_fee'] . ",";
        // InsertDateTime→Now()
        $result .= date("Y-m-d H:i:s") . ",";
        // InsertStaffCode→'MIGRATION'
        $result .= "'tool'" . ",";
        // UpdateDateTime→Now()
        $result .= date("Y-m-d H:i:s") . ",";
        // UpdateStaffCode→'MIGRATION'
        $result .= "'tool'" . ",";
        // OperationLogID→無理
        $result .= 0;
        $result .= ")\n";
    }
    $result .= "\n\n";
}


// 7.生成したSQLを出力
// 出力ファイルオープン
$fp = fopen($filename . "_.sql", "wb");
if ($fp == false) {
    print "出力ファイルのオープンに失敗しました。\n";
    exit;
}
// ファイル出力
fputs($fp, $result);
// 出力ファイルクローズ
fclose($fp);


// 8.一時関数ファイル削除
unlink($filename . "_.php");


// 9.処理終了
print "処理が完了しました。\n";
