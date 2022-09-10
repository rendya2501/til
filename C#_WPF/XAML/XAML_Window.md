# XAML_Window

---

## WindowをActiveにする

Window1 から Window2を開く。そのWindow2からWindow3を開く。  
Window3を閉じたとき、Window2がWindow1の前に来てほしいのだが、Window1が前に来てしまう。  
解決するために色々やってうまく出来たのでまとめる。  

Window1から指定したWindowをActiveにするTriggerActionを実行することで解決できた。  
これ以外にもEventAggregaterも考えたが、ViewModel間で頑張る必要もないし、親ウィンドウが子ウィンドウの存在を知っているのだから何かしら操作可能だろうということでTriggerActionにした。  

- MahApp:MetroWindow  
- Livet  

``` C# : ViewModel
Messenger.Raise(new Livet.Messaging.InteractionMessage("ActivateWindow"));
```

``` XML : View
<metro:MetroWindow
    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    xmlns:l="http://schemas.livet-mvvm.net/2011/wpf"
    xmlns:namespace="clr-namespace:namespace.Views;assembly=assembly">
    <i:Interaction.Triggers>
        <l:InteractionMessageTrigger MessageKey="ActivateWindow" Messenger="{Binding Messenger}">
            <action:ActiveWindowAction TargetWindow="{x:Type namespace:Window}" />
        </l:InteractionMessageTrigger>
    </i:Interaction.Triggers>
</metro:MetroWindow>
```

``` C# : TriggerAction
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;

namespace RN3.Wpf.Common.TriggerAction
{
    /// <summary>
    /// WindowをActiveにするトリガーアクション
    /// </summary>
    public class ActiveWindowAction : TriggerAction<MetroWindow>
    {
        #region 依存関係プロパティ
        /// <summary>
        /// 対象となるWindowのTypeの依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register(
                "TargetWindow",
                typeof(Type),
                typeof(ActiveWindowAction),
                new PropertyMetadata(null)
            );
        /// <summary>
        /// 対象となるWindowのType
        /// </summary>
        public Type TargetWindow
        {
            get => (Type)GetValue(TargetWindowProperty);
            set { SetValue(TargetWindowProperty, value); }
        }
        #endregion

        /// <summary>
        /// 指定WindowをActiveにする処理を実行します
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (TargetWindow == null)
            {
                return;
            }
            // 対象Windowが開かれているか探す
            MetroWindow window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault(f => f.ToString() == TargetWindow.FullName);
            if (window == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 最小化している場合はウィンドウサイズを元へ戻す
                if (window.WindowState == WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                }
                // 対象ウィンドウをアクティブにする
                window.Activate();
                // ウィンドウを最前面へ移動
                window.Topmost = true;
                window.Topmost = false;
            });
        }

        /// <summary>
        /// Detach前処理
        /// </summary>
        protected override void OnDetaching()
        {
            TargetWindow = null;
            base.OnDetaching();
        }
    }
}
```

[WPF の Window が開いていれば前面に表示する](https://blog.okazuki.jp/entry/20101215/1292384080)  
[C# WPFアプリ(Livet)の画面遷移を理解する](https://setonaikai1982.com/livet_screen-trans/)  

---

## WPF Window 最前面

[親フォームを子フォームの前面に表示出来るのでしょうか](http://bbs.wankuma.com/index.cgi?mode=al2&namber=54099&KLOG=90)  
→  
Ownerを設定した上だと無理っぽい。  
魔界の仮面弁士がそういっているなら無理なのだろう。  

[WPFでウインドウにフォーカスが当たらない](http://www.ria-lab.com/archives/2998)  
→  
Delayさせるなら問題はないが、安定性はなくなる。  

[qshinoの日記](https://qshino.hatenablog.com/entry/2017/03/27/023443)  
→  
Owner設定した場合、「4. 親は子を覆わない。」  
というわけで、親をクリックしたら親が全面に来るということは出来ない。  

[【.NET】ウインドウを一時的に最前面に表示しフォーカスを奪う](https://qiita.com/yaju/items/af308376f04ef2ff1325)  
→  
win32apiを使う方法も試してみたが、結局他でdelayされたら奪われる。  

``` C#
    internal static class WindowsHandles
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        // ウィンドウの最前面/解除
        public static void SetTopMostWindow(IntPtr handle, bool isTopMost)
        {
            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;
            const int SWP_SHOWWINDOW = 0x0040;
            const int HWND_TOPMOST = -1;
            const int HWND_NOTOPMOST = -2;

            if (isTopMost)
            {
                // 最前面
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            else
            {
                // 最前面解除
                SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        /// <summary>
        /// Windowフォームアクティブ化処理
        /// </summary>
        /// <param name="handle">フォームハンドル</param>
        /// <returns>true : 成功 / false : 失敗</returns>
        private static bool ForceActive(IntPtr handle)
        {
            const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
            const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
            const int SPIF_SENDCHANGE = 0x2;

            IntPtr dummy = IntPtr.Zero;
            IntPtr timeout = IntPtr.Zero;

            bool isSuccess = false;

            int processId;
            // フォアグラウンドウィンドウを作成したスレッドのIDを取得
            int foregroundID = GetWindowThreadProcessId(GetForegroundWindow(), out processId);
            // 目的のウィンドウを作成したスレッドのIDを取得
            int targetID = GetWindowThreadProcessId(handle, out processId);

            // スレッドのインプット状態を結び付ける
            AttachThreadInput(targetID, foregroundID, true);

            // 現在の設定を timeout に保存
            SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, timeout, 0);
            // ウィンドウの切り替え時間を 0ms にする
            SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, dummy, SPIF_SENDCHANGE);

            // ウィンドウをフォアグラウンドに持ってくる
            isSuccess = SetForegroundWindow(handle);

            // 設定を元に戻す
            SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, timeout, SPIF_SENDCHANGE);

            // スレッドのインプット状態を切り離す
            AttachThreadInput(targetID, foregroundID, false);

            return isSuccess;
        }

        /// <summary>
        /// ウィンドウを前面化する
        /// </summary>
        /// <param name="isTopMostOnly">最前面化設定</param>
        public static void SetActiveWindow(MetroWindow window)
        {
            // 自身をアクティブにする
            window.Activate();

            var helper = new System.Windows.Interop.WindowInteropHelper(window);
            // 表示の最初は最前面とする
            WindowsHandles.SetTopMostWindow(helper.Handle, true);
            // 最前面にした後に解除することで前面化させる
            WindowsHandles.SetTopMostWindow(helper.Handle, false);

            // 強制的にフォーカスを奪う
            WindowsHandles.ForceActive(helper.Handle);

            // 背面に隠れることがあるため、再度繰り返す
            for (int i = 0; i < 2; i++)
            {
                // 表示の最初は最前面とする
                WindowsHandles.SetTopMostWindow(helper.Handle, true);
                // 最前面にした後に解除することで前面化させる
                WindowsHandles.SetTopMostWindow(helper.Handle, false);
            }
        }
    }
```

一回だけならOwnerを設定して、開いた瞬間にOwnerをnullにすることで、全面に持ってくることができる模様。  
ただ、暫く画面を表示しなければいけない場合では、この方法は使えないだろう。  
