<?php
// 引数チェック
if ($argc != 2) {
    print $argc;
    print "引数が不正です。\n";
    exit;
}

// 解析対象ファイル名取得
$filename = $argv[1];
// 検索結果出力ファイル名
$dfilename = preg_replace("/\.log\.gz$/", "", $filename) . "_.php";

//gzファイルオープン
$zh = gzopen($filename, "rb");
if ($zh == false) {
    print "gzファイルのオープンに失敗しました。\n";
    exit;
}

$row_cnt = 1;
$hit_row_count_array = [];
$flag1 = false;
$flag2 = false;
$flag3 = false;
$flag4 = false;
$flag5 = false;
$flag6 = false;
$flag7 = false;
$flag8 = false;
$str = '';
$result_array = [];

while (gzeof($zh) == false) {
    // 1行取得
    $line = "";
    $line = gzgets($zh);
    $row_cnt += 1;
    $hit_flag = false;

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
        $hit_row_count_array[] = $row_cnt;
    }
    if ($flag8 && strstr($line, 'POST') == false) {
        $hit_flag = true;
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

// 出力ファイルオープン
$fp = fopen($dfilename, "wb");
if ($fp == false) {
    print "出力ファイルのオープンに失敗しました。\n";
    exit;
}
// 出力内容生成
$php = '<?php' . "\n";
foreach ($result_array as $key => $value) {
    $php .= 'function getArray' . $key + 1 . '(){' . "\n";
    $php .= '    return ' . $value . ';' . "\n";
    $php .= '}';
}
// ファイル出力
fputs($fp, json_decode(json_encode($php), true));
//出力ファイルクローズ
fclose($fp);

print "\n";
foreach ($hit_row_count_array as $hit_row_count) {
    print $hit_row_count . "行目からのパラメータを取得しました。\n";
}
print "処理が完了しました。\n";
