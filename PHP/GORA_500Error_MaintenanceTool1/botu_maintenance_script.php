<?php
class Hoge
{
    private $filename;

    /**
     * 
     */
    public function gzGrep($argc, $argv)
    {
        // 引数チェック
        if ($argc != 2) {
            print $argc;
            print "引数が不正です。\n";
            exit;
        }

        // 解析対象ファイル名取得
        $this->filename = $argv[1];
        // 検索結果出力ファイル名
        $dfilename = preg_replace("/\.log\.gz$/", "", $this->filename) . "_.php";

        //gzファイルオープン
        $zh = gzopen($this->filename, "rb");
        if ($zh == false) {
            print "gzファイルのオープンに失敗しました。\n";
            exit;
        }

        // 出力ファイルオープン
        $fp = fopen($dfilename, "wb");
        if ($fp == false) {
            print "出力ファイルのオープンに失敗しました。\n";
            exit;
        }

        $cnt = 1;
        $hit_row_count = 0;
        $flag1 = false;
        $flag2 = false;
        $flag3 = false;
        $flag4 = false;
        $flag5 = false;
        $flag6 = false;
        $flag7 = false;
        $flag8 = false;
        $str = '';

        while (gzeof($zh) == false) {
            // 1行取得
            $line = "";
            $line = gzgets($zh);
            $cnt += 1;
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
                $hit_row_count = $cnt;
            }
            if ($flag8 && strstr($line, 'POST') == false) {
                $hit_flag = true;
                if (strstr($line, 'App\Services\WebCooperation\PlanCooperationGORAWorkerService->callCreatePlanAPI') != false) {
                    break;
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

        $php = '<?php' . "\n";
        $php .= 'function getArray(){' . "\n";
        $php .= '    return ' . $str . ';' . "\n";
        $php .= '}';
        // ファイル出力
        fputs($fp, json_decode(json_encode($php), true));
        //出力ファイルクローズ
        fclose($fp);
        //gzファイルクローズ
        gzclose($zh);

        print "\n";
        print $hit_row_count . "行目からのパラメータを取得しました。\n";

        require $dfilename;
        $source = getArray();
        $this->createSQL($source);

        print "処理が完了しました。\n";
    }

    /**
     * SQL生成
     */
    private function createSQL($source)
    {
        // 基本情報取得
        $golf_code = (int)substr($source['plans'][0]['linkage_plan_id'], 0, 4);
        $plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 4, 6);
        $open_plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 10, 6);
        $web_link_code = $golf_code . '3';
        $linkage_plan_id = substr($source['plans'][0]['linkage_plan_id'], 0, 16);

        $result = null;
        // UPDATE文
        $result = "UPDATE TmOpenPlanTargetSite SET WebPlanID='" . $linkage_plan_id . "',LinkFlag=1 \n";
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

        $dfilename =  $this->filename . "_.sql";
        // 出力ファイルオープン
        $fp = fopen($dfilename, "wb");
        if ($fp == false) {
            print "出力ファイルのオープンに失敗しました。\n";
            exit;
        }
        // ファイル出力
        fputs($fp, $result);
        //出力ファイルクローズ
        fclose($fp);
    }
}


$hoge = new Hoge();
$hoge->gzGrep($argc, $argv);