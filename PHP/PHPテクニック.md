# PHPテクニック

## 最後の文字を消す

[PHP で文字列から最後の文字を削除する](https://www.delftstack.com/ja/howto/php/php-remove-last-character-from-string/)  
別にまとめる程のことでもないかもしれないが、色々方法があるので、一番手っ取り早い奴だけまとめておく  

``` php
// 対象文字列、スタート位置、長さ
// substr($string, $start, $length)
$mystring = "This is a PHP program.";
echo substr($mystring, 0, -1);
// 出力：This is a PHP program
```

---

## PHPのプロセスを止める方法

<https://flashbuilder-job.com/php/635.html>

`ps aux | grep php`  
これでプロセスID（左から２つ目の値）を調べる。  

ルートユーザー、またはルートになれるユーザーでコマンド実行  
`sudo kill -9 (プロセスID)`  

---

## DIサンプル

たしか、インターフェースにオプショナルを定義しなくてもエラーにならなかったのを確認したくて作ったサンプルだった気がする。  

``` PHP
<?php
    /**
     * インターフェース
     */
    interface ICurrentSlipWorkerService
    {
        // これでも動くけど、明示的にして損はない
        public function formatSlipData($data);
    }

    class CurrentSlipWorkerService implements ICurrentSlipWorkerService
    {
        // オプショナルだとインターフェースで定義していなくてもエラーにならない模様。
        // C#だとこれはエラーになる。
        public function formatSlipData($data, $mode = true)
        {
            var_dump($data,$mode);
        }
    }

    class CurrentSlipController
    {
        // 
        protected $currentslip;
        // コンストラクタ DIの注入
        public function __construct(ICurrentSlipWorkerService $currentslip)
        {
            $this->currentslip = $currentslip;
        }
        
        public function formatSlipData($data)
        {
            $this->currentslip->formatSlipData($data, true);
        }
    }
    
    $obj = new CurrentSlipController(new CurrentSlipWorkerService());
    $obj->formatSlipData(1);
?>

```
