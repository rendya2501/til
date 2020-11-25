

	■Laravel

●->get()->delete()

Eloquent
->get()->delete()はエラーになる。
->get()->each(
	function($item){
		$item->delete();
	}
);
ってやらないといけない。


●->each null

$test = $this->getOpenPlanGDO(553, 111, 111);
$test->each(
    function ($item) {
        \Log::debug($item);
    }
);
$test->eachでnullのエラーになる。
->eachはnull判定が必要。

if(!is_null($test)) $test->each(
    function ($item) {
        \Log::debug($item);
    }
);


●Laravelでコレクションをfilterするとインデックスが連続でなくなる
->filter()や->reject()すると連番が振られてしまう問題。
小一時間悩んだが、->values()するだけで済む問題だった。
しかし、知らなければ永遠に調べ続けることになるのだ。
https://yoshinorin.net/2018/05/26/laravel-filterd-items-key/
