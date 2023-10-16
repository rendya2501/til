# ダイアログ

## ダイアログ開くときのアレコレ

ダイアログを開く。  
入力事項を記入する。  
OKかCancelを押してダイアログを閉じる。  
バリデーションに引っかかり、エラーメッセージがあれば警告ダイアログを出した後に再度、入力ダイアログを開きなおす。  

という動作を実現するために色々試行錯誤したのでまとめる。  

■ **1. ローカル関数の再帰+コールバック案**

ローカル関数の再帰とダイアログのコールバックで処理を記述している。  

キャンセルした場合、Taskのキャンセルとして`OperationCanceledException`をスローしなければいけないが、コールバックで処理をしていると、コールバックがエラー扱いとなって、うまく大本まで伝搬してくれない。  
なので`.ContinueWith(t => t.IsCanceled)`みたいな判定を噛ませて、無理やりエラーを観測して処理を中断している。  

でもって`feeCorrectContext`への再代入も行っているので、全体的に完成度が低い。  
それでも、要件は満たせているけどね。  

``` cs
/// <summary>
/// 変更理由入力ダイアログを表示する
/// </summary>
/// <returns></returns>
private async Task<CorrectReasonDialogContext> ShowCorrectReasonDialog()
{
    // 結果を待機するためのTask
    TaskCompletionSource<CorrectReasonDialogContext> callbackTask = new TaskCompletionSource<CorrectReasonDialogContext>();

    CorrectReasonDialogContext feeCorrectContext = new CorrectReasonDialogContext()
    {
        StaffCD = InternalControlType == InternalControlType.LoginStaff
            ? AuthenticationInfo.StaffCode
            : null
    };

    void show()
    {
        InteractionCorrectReasonDialogMessage msg = new InteractionCorrectReasonDialogMessage(
            "CorrectReasonDialog",
            "確認",
            "変更理由を入力してください。",
            async context => await callback(context).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    return;
                }
            }),
            feeCorrectContext,
            InternalControlType
        );

        async Task callback(CorrectReasonDialogContext context)
        {
            // 前回入力内容で再度開くために結果を再代入する
            feeCorrectContext = context ?? throw new OperationCanceledException();

            // 警告メッセージがあれば表示してダイアログを開き直す。とする
            if (!string.IsNullOrEmpty(feeCorrectContext.errMsg))
            {
                await MessageDialogUtil.ShowWarningAsync(Messenger,feeCorrectContext.errMsg);
                show();
                return;
            }

            // 最後まで来たらOK
            callbackTask.SetResult(feeCorrectContext);
        }

        // ダイアログを開く
        Messenger.Raise(msg);
    }

    show();
    // 結果を待機
    return await callbackTask.Task;
}
```

■ **2. 無限ループ+非同期コールバック待機案**

ローカル関数の再帰が必要なのは、ダイアログから戻ってきたときに開きなおさなければならないわけなので、ならばループで制御出来ないか考えた案。  

それに伴い、コールバック中で行っていた処理は、タスクとして待機して、OK,Calcelを押してから先に進むようにしてみた。  
こちらの方が、上から下への流れとなり、制御がわかりやすいと思った。  
でもって、コールバックではくなったので、Exceptionのスローが安定するようになった。  

しかし、無限ループという点がやはり不安事項となる。  
また、`feeCorrectContext`に再代入している点は変わっていない。  

``` cs
/// <summary>
/// 変更理由入力ダイアログを表示する
/// </summary>
/// <returns></returns>
private async Task<CorrectReasonDialogContext> ShowCorrectReasonDialog()
{
    CorrectReasonDialogContext feeCorrectContext = new CorrectReasonDialogContext()
    {
        StaffCD = InternalControlType == InternalControlType.LoginStaff
            ? AuthenticationInfo.StaffCode
            : null
    };

    while (true)
    {
        // コールバック待機タスク
        TaskCompletionSource<CorrectReasonDialogContext> callbackContext = new TaskCompletionSource<CorrectReasonDialogContext>();

        // ダイアログを開く
        Messenger.Raise(
            new InteractionCorrectReasonDialogMessage(
                "CorrectReasonDialog",
                "確認",
                "変更理由を入力してください。",
                resContext => callbackContext.SetResult(resContext),
                feeCorrectContext,
                InternalControlType
            )
        );

        // コールバック待機
        // context=nullはキャンセルなので処理終了例外をスローする
        // 前回入力内容で再度開くために結果を再代入する
        feeCorrectContext = await callbackContext.Task ?? throw new OperationCanceledException();

        // 警告メッセージがあれば表示してダイアログを開き直す。
        if (!string.IsNullOrEmpty(feeCorrectContext.errMsg))
        {
            await MessageDialogUtil.ShowWarningAsync(Messenger, feeCorrectContext.errMsg);
            // 次のループを開始
            continue;
        }

        // 一番最後まで来たらループ終了
        break;
    }

    return feeCorrectContext;
}
```

■ **3. 非同期ローカル関数の再帰+非同期コールバック待機案**

一番最初のローカル関数の再帰と無限ループの時に思いついたコールバック待機案を合わせた案。  
`feeCorrectContext`への再代入はローカル関数の引数とすることでなくすことが出来た。  

上から下への制御がわかりやすくなり、無限ループの心配もなくなったので一番安定している案だと思われる。  

``` cs
/// <summary>
/// 変更理由入力ダイアログを表示する
/// </summary>
/// <returns></returns>
private async Task<CorrectReasonDialogContext> ShowCorrectReasonDialog()
{
    // 結果を待機するためのTask
    TaskCompletionSource<CorrectReasonDialogContext> resultTask = new TaskCompletionSource<CorrectReasonDialogContext>();

    // ダイアログ表示のためのローカル関数
    async Task show(CorrectReasonDialogContext inputContext)
    {
        // コールバック待機タスク
        TaskCompletionSource<CorrectReasonDialogContext> callbackContext = new TaskCompletionSource<CorrectReasonDialogContext>();

        // ダイアログを開く
        Messenger.Raise(
            new InteractionCorrectReasonDialogMessage(
                "CorrectReasonDialog",
                "確認",
                "変更理由を入力してください。",
                resContext => callbackContext.SetResult(resContext),
                inputContext,
                InternalControlType
            )
        );

        // コールバック待機。ダイアログのOKかCancelが押されるまでここで待機する。
        // context=nullはキャンセルなので処理終了例外をスローする
        CorrectReasonDialogContext resultContext = await callbackContext.Task ?? throw new OperationCanceledException();

        // 警告メッセージがあれば表示してダイアログを開き直す。
        if (!string.IsNullOrEmpty(resultContext.errMsg))
        {
            await MessageDialogUtil.ShowWarningAsync(Messenger, resultContext.errMsg);
            // 再表示時、前回の入力内容で再度開くため、結果を引き渡す
            _ = show(resultContext);
            return;
        }

        // 最後まで来たらOK
        resultTask.SetResult(resultContext);
    }

    // 最初の表示を行う。
    // 以後の再表示は処理内での再帰呼び出しで行う。
    _ = show(
        new CorrectReasonDialogContext()
        {
            StaffCD = InternalControlType == InternalControlType.LoginStaff
                ? AuthenticationInfo.StaffCode
                : null
        }
    );
    // 結果を待機
    return await resultTask.Task;
}
```
