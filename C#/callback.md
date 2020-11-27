
# コールバックサンプル

## Form側

``` C#
private void EditFormModel_Inquire(string message, Action yesAction, Action noAction)
{
   var dialogResult = MessageBox.Show(
       message,
       "確認",
       MessageBoxButtons.YesNo, MessageBoxIcon.Question
    );
   switch (dialogResult)
   {
       case DialogResult.Yes:
           yesAction();
           break;
       case DialogResult.No:
           noAction();
           break;
       case DialogResult.None:
       case DialogResult.Abort:
       case DialogResult.Retry:
       case DialogResult.Ignore:
       case DialogResult.OK:
       case DialogResult.Cancel:
       default:
           break;
   }
}
```

## Model側

``` C#
this.OnInquire(
   "保存処理を実行しますか？",
   () =>
   {
       try
       {
           var result = new ServiceClient.EditServiceClient().Save(this._Data);
       }
       catch (Exception ee)
       {
           MessageBox.Show(ee.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           this.OnStatusMessage("保存に失敗しました。");
           return;
       }

       //オプションで編集が有効の場合、編集モードにする
       var tmpOptionMode = true;
       if (tmpOptionMode)
       {
           //編集モード
           this.SerachBankBranch();
       }
       else
       {
           //初期化モード
           this.Initialize();
       }
       this.OnStatusMessage("データを保存しました。");
   },
   () => {}
);
```
