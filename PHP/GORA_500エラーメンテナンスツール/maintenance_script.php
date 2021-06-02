<?php
// insert文とupdate文を吐き出すツール

// Log取得
$source = readJson();

createSQL($source);

/**
 * SQL生成
 */
function createSQL($source)
{
    // 基本情報取得
    $golf_code = (int)substr($source['plans'][0]['linkage_plan_id'], 0, 4);
    $plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 4, 6);
    $open_plan_code = (int)substr($source['plans'][0]['linkage_plan_id'], 10, 6);
    $web_link_code = $golf_code . '3';
    $linkage_plan_id = substr($source['plans'][0]['linkage_plan_id'], 0, 16);

    // UPDATE文
    print "UPDATE TmOpenPlanTargetSite SET WebPlanID='" . $linkage_plan_id . "',LinkFlag=1 \n";
    print "WHERE GolfCode=" . $golf_code . " AND PlanCode=" . $plan_code . " AND OpenPlanCode=" . $open_plan_code . " AND WebLinkCode=" . $web_link_code;
    print "\n\n";

    // INSERT文
    foreach ($source['plans'] as $value) {
        print 'INSERT INTO TmOpenPlanGORAPRice VALUES(';
        // GolfCode
        print $golf_code . ",";
        // PlanCode
        print $plan_code . ",";
        // OpenPlanCode
        print $open_plan_code . ",";
        // Seq
        print (int)substr($value['linkage_plan_id'], -4) . ",";
        // PlayFee→base_price
        print $value['basis_content']['base_price'] . ",";
        // Tax→sales_tax
        print $value['basis_content']['sales_tax'] . ",";
        // AdditionPrice→additional_fee
        print $value['basis_content']['additional_fee'] . ",";
        // GolfUseTax→golf_use_tax
        print $value['basis_content']['golf_usage_tax'] . ",";
        // OtherTax→other_tax
        print $value['basis_content']['other_tax'] . ",";
        // 3BPrice→3b_additional_fee
        print $value['basis_content']['3b_additional_fee'] . ",";
        // 2BPrice→2b_additional_fee
        print $value['basis_content']['2b_additional_fee'] . ",";
        // InsertDateTime→Now()
        print date("Y-m-d H:i:s") . ",";
        // InsertStaffCode→'MIGRATION'
        print "'tool'" . ",";
        // UpdateDateTime→Now()
        print date("Y-m-d H:i:s") . ",";
        // UpdateStaffCode→'MIGRATION'
        print "'tool'" . ",";
        // OperationLogID→無理
        print 0;
        print ")\n";
    }
}

/**
 * 問題のデータ
 */
function readJson()
{
    return array(
        'plans' =>
        array(
            0 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770001',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 10546,
                    'sales_tax' => 1054,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-04',
                    1 => '2021-09-05',
                    2 => '2021-09-06',
                    3 => '2021-09-07',
                    4 => '2021-09-08',
                    5 => '2021-09-09',
                    6 => '2021-09-10',
                    7 => '2021-09-11',
                    8 => '2021-09-12',
                    9 => '2021-09-13',
                    10 => '2021-09-14',
                    11 => '2021-09-15',
                    12 => '2021-09-16',
                    13 => '2021-09-17',
                    14 => '2021-09-18',
                    15 => '2021-09-19',
                    16 => '2021-09-20',
                    17 => '2021-09-21',
                    18 => '2021-09-22',
                    19 => '2021-09-23',
                    20 => '2021-09-24',
                    21 => '2021-09-25',
                    22 => '2021-09-26',
                    23 => '2021-09-27',
                    24 => '2021-09-28',
                    25 => '2021-09-29',
                    26 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            1 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770002',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 16819,
                    'sales_tax' => 1681,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-06',
                    4 => '2021-09-07',
                    5 => '2021-09-08',
                    6 => '2021-09-09',
                    7 => '2021-09-10',
                    8 => '2021-09-11',
                    9 => '2021-09-12',
                    10 => '2021-09-13',
                    11 => '2021-09-14',
                    12 => '2021-09-15',
                    13 => '2021-09-16',
                    14 => '2021-09-17',
                    15 => '2021-09-18',
                    16 => '2021-09-19',
                    17 => '2021-09-20',
                    18 => '2021-09-21',
                    19 => '2021-09-22',
                    20 => '2021-09-23',
                    21 => '2021-09-24',
                    22 => '2021-09-25',
                    23 => '2021-09-26',
                    24 => '2021-09-27',
                    25 => '2021-09-28',
                    26 => '2021-09-29',
                    27 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            2 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770003',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 10819,
                    'sales_tax' => 1081,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-11',
                    6 => '2021-09-12',
                    7 => '2021-09-13',
                    8 => '2021-09-14',
                    9 => '2021-09-15',
                    10 => '2021-09-16',
                    11 => '2021-09-17',
                    12 => '2021-09-18',
                    13 => '2021-09-19',
                    14 => '2021-09-20',
                    15 => '2021-09-21',
                    16 => '2021-09-22',
                    17 => '2021-09-23',
                    18 => '2021-09-24',
                    19 => '2021-09-25',
                    20 => '2021-09-26',
                    21 => '2021-09-27',
                    22 => '2021-09-28',
                    23 => '2021-09-29',
                    24 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            3 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770004',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 17273,
                    'sales_tax' => 1727,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-06',
                    6 => '2021-09-07',
                    7 => '2021-09-08',
                    8 => '2021-09-09',
                    9 => '2021-09-10',
                    10 => '2021-09-13',
                    11 => '2021-09-14',
                    12 => '2021-09-15',
                    13 => '2021-09-16',
                    14 => '2021-09-17',
                    15 => '2021-09-20',
                    16 => '2021-09-21',
                    17 => '2021-09-22',
                    18 => '2021-09-23',
                    19 => '2021-09-24',
                    20 => '2021-09-25',
                    21 => '2021-09-26',
                    22 => '2021-09-27',
                    23 => '2021-09-28',
                    24 => '2021-09-29',
                    25 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            4 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770005',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 11182,
                    'sales_tax' => 1118,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-06',
                    6 => '2021-09-07',
                    7 => '2021-09-08',
                    8 => '2021-09-09',
                    9 => '2021-09-10',
                    10 => '2021-09-11',
                    11 => '2021-09-12',
                    12 => '2021-09-18',
                    13 => '2021-09-19',
                    14 => '2021-09-20',
                    15 => '2021-09-23',
                    16 => '2021-09-24',
                    17 => '2021-09-25',
                    18 => '2021-09-26',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            5 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770006',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 16364,
                    'sales_tax' => 1636,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-06',
                    6 => '2021-09-07',
                    7 => '2021-09-08',
                    8 => '2021-09-09',
                    9 => '2021-09-10',
                    10 => '2021-09-11',
                    11 => '2021-09-12',
                    12 => '2021-09-13',
                    13 => '2021-09-14',
                    14 => '2021-09-15',
                    15 => '2021-09-16',
                    16 => '2021-09-17',
                    17 => '2021-09-18',
                    18 => '2021-09-19',
                    19 => '2021-09-21',
                    20 => '2021-09-22',
                    21 => '2021-09-24',
                    22 => '2021-09-25',
                    23 => '2021-09-26',
                    24 => '2021-09-27',
                    25 => '2021-09-28',
                    26 => '2021-09-29',
                    27 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            6 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770007',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 11637,
                    'sales_tax' => 1163,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-06',
                    6 => '2021-09-07',
                    7 => '2021-09-08',
                    8 => '2021-09-09',
                    9 => '2021-09-10',
                    10 => '2021-09-11',
                    11 => '2021-09-12',
                    12 => '2021-09-13',
                    13 => '2021-09-14',
                    14 => '2021-09-15',
                    15 => '2021-09-16',
                    16 => '2021-09-17',
                    17 => '2021-09-18',
                    18 => '2021-09-19',
                    19 => '2021-09-20',
                    20 => '2021-09-21',
                    21 => '2021-09-22',
                    22 => '2021-09-23',
                    23 => '2021-09-25',
                    24 => '2021-09-26',
                    25 => '2021-09-27',
                    26 => '2021-09-28',
                    27 => '2021-09-29',
                    28 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
            7 =>
            array(
                'type' => 'regular',
                'linkage_plan_id' => '03262776330140770008',
                'basis_reception_start_at' => '2021-06-01 00:00:00',
                'basis_reception_end_at' => '2021-06-30 23:50:00',
                'basis_play_date_start' => '2021-09-01',
                'basis_play_date_end' => '2021-09-30',
                'basis_content' =>
                array(
                    'name' => '[早期予約]P付SPコンペ/キャディ付※3組以上*',
                    'base_price' => 18364,
                    'sales_tax' => 1836,
                    'golf_usage_tax' => 450,
                    'other_tax' => 0,
                    '2b_additional_fee' => 2200,
                    '3b_additional_fee' => 550,
                    'additional_fee' => 0,
                ),
                'hybrid_contents' => NULL,
                'allowed_day_set' =>
                array(
                    0 => 'sunday',
                    1 => 'monday',
                    2 => 'tuesday',
                    3 => 'wednesday',
                    4 => 'thursday',
                    5 => 'friday',
                    6 => 'saturday',
                    7 => 'holiday',
                ),
                'excluded_play_dates' =>
                array(
                    0 => '2021-09-01',
                    1 => '2021-09-02',
                    2 => '2021-09-03',
                    3 => '2021-09-04',
                    4 => '2021-09-05',
                    5 => '2021-09-06',
                    6 => '2021-09-07',
                    7 => '2021-09-08',
                    8 => '2021-09-09',
                    9 => '2021-09-10',
                    10 => '2021-09-11',
                    11 => '2021-09-12',
                    12 => '2021-09-13',
                    13 => '2021-09-14',
                    14 => '2021-09-15',
                    15 => '2021-09-16',
                    16 => '2021-09-17',
                    17 => '2021-09-18',
                    18 => '2021-09-19',
                    19 => '2021-09-20',
                    20 => '2021-09-21',
                    21 => '2021-09-22',
                    22 => '2021-09-23',
                    23 => '2021-09-24',
                    24 => '2021-09-27',
                    25 => '2021-09-28',
                    26 => '2021-09-29',
                    27 => '2021-09-30',
                ),
                'is_searchable' => true,
                'searchable_start_at' => '2021-06-01 00:00:00',
                'searchable_end_at' => '2021-06-30 23:50:00',
                'play_styles' =>
                array(
                    0 => 'with_lunch',
                    1 => 'compe',
                    2 => 'with_caddie',
                    3 => 'with_4passengers_cart',
                    4 => '1_0_rounds',
                ),
                'player_amount_min' => 3,
                'player_amount_max' => 4,
                'has_2b_assortment' => true,
                'has_3b_assortment' => false,
                'is_recommended' => true,
                'accept_request_reception' => true,
                'has_waitlist' => true,
                'has_cancel_auto_revive' => false,
                'days_before_reception_deadline' => NULL,
                'reception_deadline_time' => NULL,
                'days_before_request_deadline' => NULL,
                'request_deadline_time' => NULL,
                'has_point_grant' => false,
                'allow_point_use' => true,
                'remarks' => '----------------------------------------------------------
          フェアウェイへのカート乗り入れ通年実施中！
          乗用カート全台に【ＧＰＳナビ】搭載
          ※天候、コースコンディション等により乗入れできない場合がございます。
          ----------------------------------------------------------
          【プレースタイル】
          ・GPSナビ付乗用カート/キャディ付
          ・キャディ数に限りがございます。
          　
          【組合せ登録】
          ・組合せがお決まりになりましたら、フルネームでご同伴者登録
          　をお願いいたします。
          ・プレー日の14日前までに組数の確定をお願いいたします。
          　組合せ登録はプレー日の7日前までに必ずお願いいたします。
          ・人数に対し最小組数でのお組合せをお願いしております。
          　※4組は13名より、5組は17名からのご案内となります。
          
          【ご案内】
          ・他の優待券、割引券との併用は出来ません。
          ・スタート30分前迄にチェックインをお済ませ下さい。
          ・遅れた場合はスタート時間・コースを変更させて頂く場合がございます。
          ・プレー進行はハーフ2時間15分以内でお願い致します。
          ・当クラブでは襟付きのシャツを着用し、ジーンズ、Tシャツ
          　作業着、サンダル、下駄等でのご来場はご遠慮下さい。',
                'compes' =>
                array(
                    0 =>
                    array(
                        'group_amount' => '3',
                        'player_amount_min' => '9',
                        'benefits' => '【プラン内容】
          1：昼食付（全メニューよりオーダー可・追加料金なし・但し大盛の注文は追加料金が掛かります）
          2：昼食時、指定1ドリンク付
          指定1ドリンク付には昼食時のグラスビール、焼酎（水割・お湯割）、レモンサワー、ハイボール、各種ソフトドリンク（10種）のいずれか1杯が含まれます。
          3：パーティ料理4品+ウーロン茶ピッチャー1組に1本
          4：ゴルフ場より「ボール1ダース」提供',
                        'options' => '◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆
          
          コンペ賞品手配承ります お気軽にご相談下さい
          
          ◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆◇◆',
                    ),
                ),
                'tee_time_auto_link_settings' =>
                array(
                    0 =>
                    array(
                        'course_index' => 1,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                    1 =>
                    array(
                        'course_index' => 2,
                        'start_time_start' => '07:00',
                        'start_time_end' => '10:59',
                    ),
                ),
            ),
        ),
    );
}
