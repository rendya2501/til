# Publish/Subscribe

---

## 概要

- アプリケーションの非同期通信を容易にするため設計されたメッセージングパターン。  
- リンクすることが困難なコンポーネント間でメッセージをやり取りするといった問題を解決するのに役立つ。  
- Prism EventAggregatorで使用されているデザインパターン。  
- Prism のプロジェクトでは、EventAggregator は、２つ以上の ViewModel の間や、お互いに参照を持たないサービスの間でメッセージを送受信するためによく利用される。  

- Observerパターンが派生したものとみなされている。  
- イベント駆動型の言語でよく利用される。  

- Publisher(発行者)が発行するイベントをSubscriber(購読者)が購読して、イベントが発行されたら、登録されたコールバック(Subscriber)関数が実行されるイメージ。  

- Publisher = Subject  
- Subscriber = Observer  

- このパターンの一番大きな特徴はイベント名を動的に定義しているところ。  
- 「Observerパターン」を使って同じことを達成するためには、Subjectを複数個作成しなければならない。  
- 「Publish/Subscribe パターン」ではイベントがあらかじめ決められていないため、後から自由にいくらでも追加していくことができる。  
- PublishとSubscribeを完全に分離することで、疎結合化することができる。  

---

## 実装1

イメージ的にはこんな感じ。

``` txt
msgid | func  |
------+-------+-------
0     | func0 |
1     | func1 | func2
3     | func3 |
4     | func4 |
```

キー1のPublisherに対して関数を2つ登録して、Publishすると登録した2つの関数が実行される。  
言葉にするとただのイベントでしかないな・・・。

``` php
<?php
/**
 * 登録解除クラス
 */
class Disposable
{
    private  $pubSub,$callback;

    function __construct(PubSub $pubSub, callable $callback)
    {
        $this->pubSub = $pubSub;
        $this->callback = $callback;
    }

    /**
     * コールバックを通知リストから削除する
     *
     * @return void
     */
    public function dispose()
    {
        $this->pubSub->unsubscribe($this->callback);
    }
}

/**
 * Publish/Subscribeクラス
 */
class PubSub
{
    private $topics = [];

    /**
     * イベント名とコールバックを登録する
     *
     * @param [type] $topic
     * @param callable $callback
     * @return Disposable
     */
    function subscribe($topic, callable $callback): Disposable
    {
        // キーがなければ配列生成
        if (!isset($this->topics[$topic])) {
            $this->topics[$topic] = [];
        }
        // キーに対してコールバックを登録する
        array_push($this->topics[$topic], $callback);

        return new Disposable($this, $callback);
    }

    /**
     * イベント名に対するコールバックを解除する
     *
     * @param callable $callback
     * @return void
     */
    function unsubscribe(callable $callback)
    {
        foreach ($this->topics as $i => $topic) {
            foreach ($topic as $j => $_callback) {
                if ($callback === $_callback) {
                    array_splice($this->topics[$i], $j, 1);
                    return $callback;
                }
            }
        }
        return false;
    }

    /**
     * 発行
     *
     * @param [type] $topic
     * @return void
     */
    public function publish($topic)
    {
        if (!isset($this->topics[$topic])) {
            return false;
        }

        // 関数に渡された引数を取得 例:[0]sms,[1]time
        $args = func_get_args();
        // 配列の先頭から要素を一つ取り出す
        array_shift($args);
        foreach ($this->topics[$topic] as $callback) {
            // コールバック実行
            call_user_func_array($callback, $args);
        }

        return $this;
    }
}


// ------------------メイン処理-----------------------

// subscriber1
$message_to_girlfriend = function ($time) {
    echo "今帰った。晩御飯は何にする？($time)\n";
};

// subscriber2
$message_to_mum = function ($time) {
    echo "遊びに来る?($time)\n";
};

// subscriber3
$message_to_friend = function ($time) {
    echo "今暇？？($time)\n";
};

$pubSub = new PubSub();

// message_to_girlfriendをdisposableとして受け取る
$disposable = $pubSub->subscribe('sms', $message_to_girlfriend);
// smsに2つ目を登録する
$pubSub->subscribe('sms', $message_to_mum);
$pubSub->subscribe('line', $message_to_friend);

// smsを発行→2つのイベントが実行される
$pubSub->publish('sms', time());
echo "\n";
$pubSub->publish('line', time() + 3600);
echo "\n";

// $message_to_girlfriend を通知リストから削除する
$disposable->dispose();
// smsを発行→1つのイベントが実行される
$pubSub->publish('sms', time());
```

---

## 参考

[Prism EventAggregator をなぜ使うべきか](https://shikaku-sh.hatenablog.com/entry/wpf-prism-why-should-use-eventaggregator)  
[【PHPで学ぶデザインパターン入門】第6回 Observerパターン](https://liginc.co.jp/web/programming/php/149799)  
[Pub/Subメッセージングモデルの業務利用における難しさ](https://qiita.com/TakaakiOtomo/items/badba239ade07c4ea59f)  
[PythonにおけるPub/Subパターンの実装](https://webty.jp/staffblog/production/post-3328/)  
[C# Channels - Publish / Subscribe Workflows](https://deniskyashif.com/2019/12/08/csharp-channels-part-1/)  
[Python で publish-subscribe パターン](https://zenn.dev/miwarin/articles/df7998929dc955)  
