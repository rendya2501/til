# PHP_DIサンプル

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
```
