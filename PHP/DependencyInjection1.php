<?php
    /**
     * undocumented class
     *
     * @package default
     * @author `g:snips_author`
     */
    interface ICurrentSlipWorkerService
    {
        // これでも動くけど、明示的にして損はない
        public function formatSlipData($data);
    }
    
    class CurrentSlipController
    {
        protected $currentslip;
        
        public function __construct(ICurrentSlipWorkerService $currentslip)
        {
            $this->currentslip = $currentslip;
        }
        
        public function formatSlipData($data)
        {
            $this->currentslip->formatSlipData($data, true);
        }
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
    
    $obj = new CurrentSlipController(new CurrentSlipWorkerService());
    $obj->formatSlipData(1);
?>
