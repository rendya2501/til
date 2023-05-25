# MAUI Toast

## ダブルクリックでアプリ終了

`_DoubleBackToExitPressedOnce`がTrueなら処理終了。  
初期値はFalse。  
Falseなので処理は続行する。  
フラグを挙げる。  
Toastを表示しつつ、2秒経過したらフラグをFalseにするTaskを開始する。  
2秒以内に再度戻るボタンをタップした場合、フラグがTrueとなっているので処理が終わる。  

``` cs
public partial class BaseContentPage : ContentPage
{
    private bool _DoubleBackToExitPressedOnce = false;

    protected override bool OnBackButtonPressed()
    {
        if (_DoubleBackToExitPressedOnce)
        {
            return base.OnBackButtonPressed();
        }

        _DoubleBackToExitPressedOnce = true;

        CancellationTokenSource cancellationTokenSource = new();
        Toast.Make("終了する場合はもう一度「戻る操作」を行ってください", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
        Task.Delay(2000).ContinueWith(_ => _DoubleBackToExitPressedOnce = false);

        return true;
    }
}
```

[java - Clicking the back button twice to exit an activity - Stack Overflow](https://stackoverflow.com/questions/8430805/clicking-the-back-button-twice-to-exit-an-activity)  
[Doble Tab to Exit in Microsoft MAUI - Microsoft Q&A](https://learn.microsoft.com/en-us/answers/questions/992556/doble-tab-to-exit-in-microsoft-maui)  

---

## toastとsnackbarの違い

[snackbarとtoastの違いまとめ｜ドリーム・アーツ Designers](https://note.com/dreamarts_design/n/na830aedec855)  
