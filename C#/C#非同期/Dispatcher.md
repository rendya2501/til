# Dispatcher

<https://hilapon.hatenadiary.org/entry/20130225/1361779314>  
<https://araramistudio.jimdo.com/2017/05/02/c-%E3%81%A7%E5%88%A5%E3%82%B9%E3%83%AC%E3%83%83%E3%83%89%E3%81%8B%E3%82%89%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC%E3%83%AB%E3%82%92%E6%93%8D%E4%BD%9C%E3%81%99%E3%82%8B/>  
<https://www.it-swarm-ja.com/ja/c%23/dispatcherinvoke%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E9%9D%9E%E3%83%A1%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%AC%E3%83%83%E3%83%89%E3%81%8B%E3%82%89wpf%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC%E3%83%AB%E3%82%92%E5%A4%89%E6%9B%B4%E3%81%99%E3%82%8B/968851135/>  

`Application.Current.Dispatcher.Invoke(() =>`謎の呪文。後で調べたい。  
→  
どうやら、並列処理における概念らしい。  
async,awaitなどでスレッドを分けた場合、他のスレッドで動作しているコントロールをいじれなくなってしまう。  
でも、場合によってはそのコントロールを操作したい時があるわけで、  
そういう場合に、そのコントロールを操作する仕組みがDispatcherっぽい。  
Dispatcherはキューを管理するクラス。  
操作したいコントロールを所有しているスレッドのキュー管理に対して、指定したデリゲートの実行をお願いするイメージ。  
とりあえず、並列処理とかasync,awaitとかの時に使えばいいみたいね。  

[Dispatcher.InvokeとDispatcher.BeginInvoke、Dispatcher.InvokeAsyncの違い](https://redwarrior.hateblo.jp/entry/2021/03/29/090000)  
この記事も面白い。  

[await/async等の別スレッドからコントロールのプロパティを変更する](https://todosoft.net/blog/?p=363)  
改めて見てみたけど、非同期中に他のコントロールのプロパティ変更したりするときに使う命令みたいね。  

``` C#
/*
await/async等の別スレッドからコントロールのプロパティを変更しようとすると、
コントロールの描画を行っているスレッドと異なる為、エラーとなる。
InvokeRequired で異なるスレッドからの呼び出しかを判定し、異なる場合は Invoke で再呼び出しをすることで解決する。
*/
private async void button1_Click(object sender, EventArgs e)
{
    await Task.Run(() =>
    {
        Invoke((MethodInvoker)(() =>
        {
            // 非同期中に他のコントロールのプロパティを変更する
            button1.Text = "OK";
        }));
    });
}
// WPFのVMでは上記Invokeが使用できない為、下記のようになる
System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
    // 同期処理
}));
```
