<?php
// Your code here!
$te = array (
  '2020-08-01' => 
  array (
    1 => 
    array (
      0 => 
      array (
        'reception_start_at' => '2020-06-01 00:00:00',
        'reception_end_at' => '2020-06-30 00:50:00',
        'play_date_start' => '2020-08-01',
        'play_date_end' => '2020-08-01',
        'course_index' => '1',
        'start_time_start' => '07:00',
        'start_time_end' => '10:30',
        'content' => 
        array (
          'name' => '早割☆平日セルフ',
          'is_limited_period' => false,
          'is_auto_link' => true,
          'base_price' => 8900,
          'sales_tax' => 890.0,
          'golf_usage_tax' => 950,
          'other_tax' => 60,
          '2b_additional_fee' => 1500,
          '3b_additional_fee' => 0,
        ),
      ),
      1 => 
      array (
          'reception_start_at' => '2020-06-01 00:00:00',
          'reception_end_at' => '2020-06-30 00:50:00',
          'play_date_start' => '2020-08-01',
          'play_date_end' => '2020-08-01',
          'course_index' => '1',
          'start_time_start' => '10:00',
          'start_time_end' => '12:30',
          'content' => 
          array (
            'name' => '早割☆平日セルフ',
            'is_limited_period' => false,
            'is_auto_link' => true,
            'base_price' => 8900,
            'sales_tax' => 890.0,
            'golf_usage_tax' => 950,
            'other_tax' => 60,
            '2b_additional_fee' => 1500,
            '3b_additional_fee' => 0,
          ),
      ),
    ),
    2 => 
    array (
      0 => 
      array (
        'reception_start_at' => '2020-06-01 00:00:00',
        'reception_end_at' => '2020-06-30 00:50:00',
        'play_date_start' => '2020-08-01',
        'play_date_end' => '2020-08-01',
        'course_index' => '2',
        'start_time_start' => '07:00',
        'start_time_end' => '10:30',
        'content' => 
        array (
          'name' => '早割☆平日セルフ',
          'is_limited_period' => false,
          'is_auto_link' => true,
          'base_price' => 8900,
          'sales_tax' => 890.0,
          'golf_usage_tax' => 950,
          'other_tax' => 60,
          '2b_additional_fee' => 1500,
          '3b_additional_fee' => 0,
        ),
      ),
    ),
    'FeeInfo' => 
    array (
      '4BPrice' => 8900,
      'GolfUseTax' => 950,
      'OtherTax' => 60,
      'Tax' => 890.0,
    ),
  ),
  '2020-08-02' => 
  array (
    1 => 
    array (
      0 => 
      array (
        'reception_start_at' => '2020-06-01 00:00:00',
        'reception_end_at' => '2020-06-30 00:50:00',
        'play_date_start' => '2020-08-02',
        'play_date_end' => '2020-08-02',
        'course_index' => '1',
        'start_time_start' => '07:00',
        'start_time_end' => '10:30',
        'content' => 
        array (
          'name' => '早割☆平日セルフ',
          'is_limited_period' => false,
          'is_auto_link' => true,
          'base_price' => 8900,
          'sales_tax' => 890.0,
          'golf_usage_tax' => 950,
          'other_tax' => 60,
          '2b_additional_fee' => 1500,
          '3b_additional_fee' => 0,
        ),
      ),
    ),
    2 => 
    array (
      0 => 
      array (
        'reception_start_at' => '2020-06-01 00:00:00',
        'reception_end_at' => '2020-06-30 00:50:00',
        'play_date_start' => '2020-08-02',
        'play_date_end' => '2020-08-02',
        'course_index' => '2',
        'start_time_start' => '07:00',
        'start_time_end' => '10:30',
        'content' => 
        array (
          'name' => '早割☆平日セルフ',
          'is_limited_period' => false,
          'is_auto_link' => true,
          'base_price' => 8900,
          'sales_tax' => 890.0,
          'golf_usage_tax' => 950,
          'other_tax' => 60,
          '2b_additional_fee' => 1500,
          '3b_additional_fee' => 0,
        ),
      ),
    ),
    'FeeInfo' => 
    array (
      '4BPrice' => 8900,
      'GolfUseTax' => 950,
      'OtherTax' => 60,
      'Tax' => 890.0,
    ),
  ),
  );
  $te['2020-08-01'][1] = null;
  $te['2020-08-01'][2] = null;
  var_dump($te);
?>

array(2) {
  ["2020-08-01"]=>
  array(3) {
    [1]=>
    NULL
    [2]=>
    NULL
    ["FeeInfo"]=>
    array(4) {
      ["4BPrice"]=>
      int(8900)
      ["GolfUseTax"]=>
      int(950)
      ["OtherTax"]=>
      int(60)
      ["Tax"]=>
      float(890)
    }
  }
  ["2020-08-02"]=>
  array(3) {
    [1]=>
    array(1) {
      [0]=>
      array(8) {
        ["reception_start_at"]=>
        string(19) "2020-06-01 00:00:00"
        ["reception_end_at"]=>
        string(19) "2020-06-30 00:50:00"
        ["play_date_start"]=>
        string(10) "2020-08-02"
        ["play_date_end"]=>
        string(10) "2020-08-02"
        ["course_index"]=>
        string(1) "1"
        ["start_time_start"]=>
        string(5) "07:00"
        ["start_time_end"]=>
        string(5) "10:30"
        ["content"]=>
        array(9) {
          ["name"]=>
          string(24) "早割☆平日セルフ"
          ["is_limited_period"]=>
          bool(false)
          ["is_auto_link"]=>
          bool(true)
          ["base_price"]=>
          int(8900)
          ["sales_tax"]=>
          float(890)
          ["golf_usage_tax"]=>
          int(950)
          ["other_tax"]=>
          int(60)
          ["2b_additional_fee"]=>
          int(1500)
          ["3b_additional_fee"]=>
          int(0)
        }
      }
    }
    [2]=>
    array(1) {
      [0]=>
      array(8) {
        ["reception_start_at"]=>
        string(19) "2020-06-01 00:00:00"
        ["reception_end_at"]=>
        string(19) "2020-06-30 00:50:00"
        ["play_date_start"]=>
        string(10) "2020-08-02"
        ["play_date_end"]=>
        string(10) "2020-08-02"
        ["course_index"]=>
        string(1) "2"
        ["start_time_start"]=>
        string(5) "07:00"
        ["start_time_end"]=>
        string(5) "10:30"
        ["content"]=>
        array(9) {
          ["name"]=>
          string(24) "早割☆平日セルフ"
          ["is_limited_period"]=>
          bool(false)
          ["is_auto_link"]=>
          bool(true)
          ["base_price"]=>
          int(8900)
          ["sales_tax"]=>
          float(890)
          ["golf_usage_tax"]=>
          int(950)
          ["other_tax"]=>
          int(60)
          ["2b_additional_fee"]=>
          int(1500)
          ["3b_additional_fee"]=>
          int(0)
        }
      }
    }
    ["FeeInfo"]=>
    array(4) {
      ["4BPrice"]=>
      int(8900)
      ["GolfUseTax"]=>
      int(950)
      ["OtherTax"]=>
      int(60)
      ["Tax"]=>
      float(890)
    }
  }
}
