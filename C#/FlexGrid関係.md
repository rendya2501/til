# FlexGridに関して色々

## FlexGridの縦横の大きさを動的に変動する方法

[WPF ScrollViewer with Grid inside and dynamic size](https://stackoverflow.com/questions/48529723/wpf-scrollviewer-with-grid-inside-and-dynamic-size)  

RN3のどのプログラムを見ても、FlexGridの大きさが画面の大きさに連動して変わる奴がなかったので、何とかならないか探してみたらそれっぽいものがあった。  
やっぱりGrid先輩が強すぎる。  
とりあえずGridで囲んでおけみたいなところあるな。  

``` XML
<Window x:Class="WpfTest.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Width="200"
        Height="300">

  <!-- Layout Root -->
  <Grid>
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <!-- Header Panel -->
    <Border Grid.Row="0" Background="#CCCCCC" Padding="11">
      <!-- Replace this TextBlock with your header content. -->
      <TextBlock Text="Header Content" TextAlignment="Center" />
    </Border>

    <!-- Body Panel -->
    <Grid Grid.Row="1" Background="#CCCCFF">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
      </Grid.RowDefinitions>

      <Border Grid.Row="0" Background="#FFCCCC" Padding="11">
        <!-- Replace this TextBlock with your upper body content. -->
        <TextBlock Text="Upper Body Content" TextAlignment="Center" />
      </Border>

      <ScrollViewer Grid.Row="1" Padding="11"
                    VerticalScrollBarVisibility="Auto">

        <!-- Replace this Border with your scrollable content. -->
        <Border MinHeight="200">
          <Border.Background>
            <RadialGradientBrush RadiusY="1" RadiusX="1" Center="0.5,0.5">
              <GradientStop Color="White" Offset="0" />
              <GradientStop Color="Black" Offset="1" />
            </RadialGradientBrush>
          </Border.Background>
        </Border>

      </ScrollViewer>
    </Grid>

    <!-- Footer Panel -->
    <Border Grid.Row="2" Background="#CCFFCC" Padding="11">
      <!-- Replace this TextBlock with your footer content. -->
      <TextBlock Text="Footer Content" TextAlignment="Center" />
    </Border>
  </Grid>

</Window>
```

---

## キーボードフォーカスを移動させるコードビハインド

``` C#
private void PaymentMethodFlexGrid_SelectedItemChanged(object sender, System.EventArgs e)
{
    if (sender is C1FlexGrid grid)
    {
        var cell = grid.Cells.GetCellElement(grid.Selection);
        if (cell != null)
        {
            var input = cell.FindVisualChild<TextBox>();
            if (input != null)
            {
                input.Focus();
                Keyboard.Focus(input);
            }
        }
    }
}
```

---

## FlexGridのRowHeaderに番号を付ける方法とCellFactoryサンプル

FlexGridのRowHeaderに番号を付ける方法を聞かれたがわからなかった。  
萬君がCellFactoryを使ったやり方で実現していたので拝借した。  
そのほかにも、チェックアウトで疑似的に実現していたのでそれも拝借する。  

``` XML : Front.CheckOut.Views.SelfCheckOutWindow.xaml
    <!-- xmlns:factory="clr-namespace:RN3.Wpf.Common.Util.C1CellFactory;assembly=RN3.Wpf.Common" -->
    <metro:MetroWindow.Resources>
        <ResourceDictionary>
            <factory:RowHeaderNumberingCellFactory x:Key="RowHeaderNumberingCellFactory" />
        </ResourceDictionary>
    </metro:MetroWindow.Resources>

    <!-- HeadersVisibility="Column"ついていると出ない。なくすと出る。 -->
    <c1:C1FlexGrid
        CellFactory="{StaticResource RowHeaderNumberingCellFactory}">
        <c1:C1FlexGrid.Columns>
            <c1:Column>
                <!-- 色々 -->
            </c1:Column>
        </c1:C1FlexGrid.Columns>
    </c1:C1FlexGrid
```

``` C# : Common.Util.C1CellFactory.RowHeaderNumberingCellFactory.cs
/// <summary>
/// RowHeaderに番号を付けるCellFactory
/// </summary>
public class RowHeaderNumberingCellFactory : CellFactory
{
    /// <summary>
    /// CreateRowHeaderContent
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="bdr"></param>
    /// <param name="rng"></param>
    public override void CreateRowHeaderContent(C1.WPF.FlexGrid.C1FlexGrid grid, Border bdr, CellRange rng)
    {
        var tb = new TextBlock();
        tb.HorizontalAlignment = HorizontalAlignment.Center;
        tb.VerticalAlignment = VerticalAlignment.Center;
        tb.Foreground = grid.RowHeaderForeground;
        tb.Text = (rng.Row + 1).ToString();
        bdr.Child = tb;
    }
}
```

``` XML : Front.CheckOut.Views.CheckOutWindow.xaml
    <!-- CellFactoryを使わないで実装するパターン -->
    <ctrl:CustomFlexGrid>
        <ctrl:CustomFlexGrid.Columns>
            <c1:Column
                Width="28"
                HorizontalAlignment="Right"
                VerticalAlignment="Center"
                Background="{StaticResource MahApps.Brushes.Accent}"
                Binding="{Binding CheckoutOrder}"
                ColumnName="CheckoutOrder"
                Foreground="{StaticResource MahApps.Brushes.IdealForeground}"
                Format="N0"
                Header=" "
                HeaderHorizontalAlignment="Center"
                HeaderVerticalAlignment="Center" />
        </ctrl:CustomFlexGrid.Columns>
    </ctrl:CustomFlexGrid>
```

---

## 右クリックでセルを選択、右クリックで何行目かのダイアログを出すサンプル

チェックアウトはサンプルの宝庫だな。  

``` XML
    <ctrl:CustomFlexGrid>
        <i:Interaction.Behaviors>
            <behavior:C1FlexGridMouseRightButtonDownToSelectRowBehavior />
        </i:Interaction.Behaviors>
        <!-- チェックアウトで実装されていたバージョン -->
        <ctrl:CustomFlexGrid.ContextMenu>
            <ContextMenu>
                <MenuItem Command="{Binding ShowPlayerDialogCommand}" Header="プレーヤー詳細を表示" />
                <MenuItem
                    Command="{Binding DeleteSettlementDetailCommand}"
                    CommandParameter="{Binding SelectedSettlementDetail}"
                    Header="{Binding SelectedSettlementDetail.CheckoutOrder, Mode=OneWay, TargetNullValue={x:Static sys:String.Empty}}"
                    HeaderStringFormat="{}{0:N0} 行目を削除" />
            </ContextMenu>
        </ctrl:CustomFlexGrid.ContextMenu>
        <!-- ConverterParameterを指定することでXAML上で1行目から始めることができる -->
        <ctrl:CustomFlexGrid.ContextMenu>
            <ContextMenu>
                <MenuItem
                    Command="{Binding DeleteSelectedSlipCommand}"
                    CommandParameter="{Binding SelectedItem}"
                    Header="{Binding SelectedIndex, Mode=OneWay, TargetNullValue={x:Static sys:String.Empty}, Converter={StaticResource AdditionConverter}, ConverterParameter=1}"
                    HeaderStringFormat="{}{0:N0} 行目を削除" />
            </ContextMenu>
        </ctrl:CustomFlexGrid.ContextMenu>
        <!-- 親をたどってSelectedIndexの値を使うタイプ -->
        <!-- 行けたと思ったんだけど、起動する時にエラーになる。どうやら親が生成されていない状況が発生してしまう模様。 -->
        <ctrl:CustomFlexGrid.ContextMenu>
            <ContextMenu>
                <MenuItem
                    Command="{Binding DeleteSettlementSettingPlayerCommand}"
                    CommandParameter="{Binding SelectedSettlementDetail}"
                    Header="{Binding SelectedIndex, RelativeSource={RelativeSource FindAncester, AncestorType={x:Type ctrl:CustomFlexGrid}}, Mode=OneWay, TargetNullValue={x:Static sys:String.Empty}, Converter={StaticResource AdditionConverter}, ConverterParameter=1}"
                    HeaderStringFormat="{}{0:N0} 行目を削除" />
            </ContextMenu>
        </ctrl:CustomFlexGrid.ContextMenu>

```

---

## XAML上でソートをクリアする命令を実行する方法

2021/10/06 Fri  
最近、ViewModelからコントロールのメソッドを実行する方法を見つけたので、  
「伝票一括入力でやってるソートをクリアするだけのトリガーアクションあるじゃん。あれ、これで実現できないか？」と思ってやったら行けたのでまとめる。  

``` XML : Front.SlipBulkInput.Views.EditWindow.xaml
 <i:Interaction.Triggers>
    <l:InteractionMessageTrigger MessageKey="FlexGridClearSortAction" Messenger="{Binding Messenger}">
        <!-- CallMethodActionの存在を知らなかった時は、わざわざTriggerActionを定義してそれを叩いていた。しかも1行しかない。 -->
        <!--<localaction:C1FlexGridClearSortAction />-->

        <!-- TargetObjectを適切に指定することで、そのオブジェクトに属するメソッドを呼び出すことができる。 -->
        <!-- TriggerActionに記述していた1行だけの命令はこれで実行できる。 -->
        <i:CallMethodAction MethodName="Clear" TargetObject="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ctrl:CustomFlexGrid}}, Path=CollectionView.SortDescriptions}" />
    </l:InteractionMessageTrigger>

    <!-- 画面をリフレッシュするだけならこれでよい。 -->
    <l:InteractionMessageTrigger MessageKey="FlexGridInvalidateAction" Messenger="{Binding Messenger}">
        <i:CallMethodAction MethodName="Invalidate" />
    </l:InteractionMessageTrigger>
</i:Interaction.Triggers>
```

``` C# : Front.SlipBulkInput.TriggerAction.C1FlexGridClearSortAction.cs
    /// <summary>
    /// C1FlexGridのソートを解除するトリガーアクション
    /// </summary>
    public class C1FlexGridClearSortAction : TriggerAction<C1FlexGrid>
    {
        /// <summary>
        ///  FlexGridのソートを解除します。
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter) => AssociatedObject.CollectionView.SortDescriptions.Clear();
    }
```

//Messenger.Raise(new InteractionMessage("FlexGridInvalidateAction"));

---

## FlexGridのセルを選択する時に色々あったのでまとめ

個人売上ムーブで新しく伝票を追加したら、追加した伝票にフォーカスが当たってほしいっていう修正をした時の話。  
この修正をしたときは、セルで編集する機能がバグにより実現不可能な状態だったので、セル選択をやめて行選択にしてもよかったんだけど、  
それだとフォーカスが当たった部分の色が、黒に寒色がかぶって見えにくくなって、違和感がすごかったので、  
セル選択のままにして、何とか数量列を指定できないか色々やる方向にした。  
まぁ、色々あったので、その時のまとめ。  

### CallMethodActionでFlexGridのSelectメソッドを呼び出したい

前にCallMethodActionでViewから直接メソッドを実行できることを発見した。  
今回も使えないかと思って調べた。  

FlexGridのSelectメソッドは行と列の2つの引数を必要とする。  
CommandParameterとかで複数の引数を渡すことができるのか？  
→できない。  
渡すとしてもObject型になるので、受け取る側もObject型でなければいけない。  
受け取った後に、内部でキャストして順次取り出す感じ。  

CommandParameterに複数のパラメーターを指定したければ、MultiBindingとMultiConverterを使う必要がある。  
だけど、これだけのためにMultiConverter作るのは面倒くさい。  
さらに、項目が追加されたら毎回、最終行の3列目を指定したいので、毎回MessengerをRaiseすることになる。  
となれば、ビヘイビアにしたほうがいいのでは？となった。  
普通にビヘイビアで解決した。  

### FlexGridの項目の追加・削除を観測するイベント

→`FlexGrid.Rows.CollectionChangedイベント`  
ItemsSourceChanging、ItemsSourceChangedではない。  
バインド先がItemsSourceなので、なんでできないのか小一時間悩んでしまった。  
正解はRowsでした。  

---

### Mah チェックボックスの色変える

``` C#
        // これでMahのチェックつけたときの青色が取れる。
        private static readonly SolidColorBrush _ForeGround = (SolidColorBrush)Application.Current.Resources["MahApps.Brushes.Accent"];


                        var textBlockList = cell.FindVisualChildren<CheckBox>();
                        if (textBlockList != null)
                        {
                            foreach (var textBlock in textBlockList)
                            {
                                if (AssociatedObject.Selection.Row == i && AssociatedObject.Selection.Column == j)
                                {
                                    // これで色変えられる。
                                    // CheckGlyphForegroundCheckedはXAMLで指定するときのあれ。
                                    // mahapp:CheckGlyphForegroundChecked()
                                    CheckBoxHelper.SetCheckGlyphForegroundChecked(textBlock, _ForeGround);
                                    CheckBoxHelper.SetCheckGlyphForegroundCheckedMouseOver(textBlock, _ForeGround);



                // 値を表示/編集するために、チェックボックスを作成します
                CheckBox chk = new CheckBox
                {
                    IsChecked = (bool?)grid.Cells[rng.Row, rng.Column],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                _ = chk.SetBinding(ToggleButton.IsCheckedProperty, grid.Columns["Cancel"].Binding);
                // 精算済みのレコードは操作不能にして色を変える
                if (!(bool)grid.Cells[rng.Row, "ValidFlag"])
                {
                    chk.IsEnabled = false;
                    grid.Columns["Cancel"].IsReadOnly = true;
                    // ここで色変える
                    CheckBoxHelper.SetCheckBackgroundFillUncheckedDisabled(
                        chk,
                        (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"]
                    );
                }
                // チェックボックスをセル要素に割り当てます
                bdr.Child = chk;
```

---

## セルファクトリーでチェックボックスの制御

``` C#
using C1.WPF.FlexGrid;
using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RN3.Wpf.Front.AccountsReceivableSlip.CellFactory
{
    /// <summary>
    /// 売掛伝票入力セルファクトリー
    /// </summary>
    /// <remarks>
    /// ソートを実装したらチェックボタンの制御を色々やらないといけなくて、
    /// セルファクトリーでしか実現できなかったので実装した。
    /// ここ参考にした。
    /// <https://docs.grapecity.com/help/c1/xaml/xaml_flexgrid/#Displayingcheckbox.html>
    /// </remarks>
    public class AccountsReceivableSlipCellFactory : C1.WPF.FlexGrid.CellFactory
    {
        /// <summary>
        /// CreateRowHeaderContent
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="bdr"></param>
        /// <param name="rng"></param>
        public override void CreateRowHeaderContent(C1FlexGrid grid, Border bdr, CellRange rng)
        {
            var row = grid.Rows[rng.Row];
            if (!(row is GroupRow))
            {
                var tb = new TextBlock();
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Foreground = grid.RowHeaderForeground;
                tb.Text = (rng.Row + 1 - grid.Rows.Where(w => w.Index < rng.Row && w is GroupRow).Count()).ToString();
                bdr.Child = tb;
            }
        }

        public override void CreateCellContent(C1FlexGrid grid, Border bdr, CellRange rng)
        {
            if (grid.Cells[rng.Row, rng.Column] is bool)
            {
                // 値を表示/編集するために、チェックボックスを作成します
                CheckBox chk = new CheckBox
                {
                    IsChecked = (bool?)grid.Cells[rng.Row, rng.Column],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                _ = chk.SetBinding(ToggleButton.IsCheckedProperty, grid.Columns["Cancel"].Binding);
                // 精算済みのレコードは操作不能にして色を変える
                if (!(bool)grid.Cells[rng.Row, "ValidFlag"])
                {
                    chk.IsEnabled = false;
                    grid.Columns["Cancel"].IsReadOnly = true;
                    CheckBoxHelper.SetCheckBackgroundFillUncheckedDisabled(
                        chk,
                        (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"]
                    );
                }
                // チェックボックスをセル要素に割り当てます
                bdr.Child = chk;
            }
            else
            {
                // ブール値ではない場合、デフォルト動作を実現
                base.CreateCellContent(grid, bdr, rng);
            }
        }

        /// <summary>
        /// セルが廃却された場合、セルの境界のコンテンツも削除します
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cellType"></param>
        /// <param name="cell"></param>
        public override void DisposeCell(C1FlexGrid grid, CellType cellType, FrameworkElement cell)
        {
            Border bdr = (Border)cell;
            bdr.Child = null;
        }
    }
}
```

---

### aaa

``` C#
using C1.WPF.FlexGrid;
using Microsoft.Xaml.Behaviors;
using RN3.Wpf.Front.DutchTreat.Resource;
using System.Windows;
using System.Windows.Media;

namespace RN3.Wpf.Front.DutchTreat.TriggerAction
{
    /// <summary>
    /// 割り勘FlexGridを読み取りモードにするトリガーアクション
    /// </summary>
    public class DutchTreatGridReadOnlyModeAction : TriggerAction<C1FlexGrid>
    {
        /// <summary>
        /// 読み取り専用背景色
        /// </summary>
        private static readonly Brush ReadOnlyBackGroundColor = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];

        /// <summary>
        /// 読み取りモードにします。
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            // 割り勘金額列
            var dutchTreatAmount = AssociatedObject.Columns[DutchTreatGridColumnName.DutchTreatAmount];
            dutchTreatAmount.IsReadOnly = true;
            dutchTreatAmount.Background = ReadOnlyBackGroundColor;
            // 計算対象外列
            var isNoTargetCalc = AssociatedObject.Columns[DutchTreatGridColumnName.IsNoTargetCalc];
            isNoTargetCalc.IsReadOnly = true;
            isNoTargetCalc.Background = ReadOnlyBackGroundColor;
            // FlexGridを再描画
            AssociatedObject.Invalidate();
        }
    }
}
```

---

## セルファクトリーでツールチップを表示する

<https://docs.grapecity.com/help/c1/xaml/xaml_flexgrid/#Displayingtooltip.html>  

それはそうとして、どちらかというとカラム名の指定とそのカラムだけにツールチップを表示するようにする制御のほうがメインかもしれん。

``` C#
using C1.WPF.FlexGrid;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using C1.WPF;

namespace RN3.Wpf.Reservation.ReservationSearch.CellFactory
{
    /// <summary>
    /// 予約検索セルファクトリー
    /// </summary>
    public class ReservationSearchCellFactory : C1.WPF.FlexGrid.CellFactory
    {
        /// <summary>
        /// セルコンテンツ作成
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="bdr"></param>
        /// <param name="rng"></param>
        public override void CreateCellContent(C1FlexGrid grid, Border bdr, CellRange rng)
        {
            //// コンテンツを作成します
            base.CreateCellContent(grid, bdr, rng);
            // 指定したカラム名かどうか判定
            // nameofでもいける
            if (grid.Columns[rng.Column].ColumnName == "Player1Name")
            {
                // これでもなんか取れる
                var aaa = grid.Columns["Player1Name"];
                // そのセルに備考が存在したらツールチップを設定
                // セルの指定の仕方は grid[rng.Row, grid.Columns["Player1Remarks"]
                if (!string.IsNullOrEmpty(grid[rng.Row, grid.Columns["Player1Remarks"].Index]?.ToString()))
                {
                    var contents = $"備考:{ Environment.NewLine }{grid[rng.Row, grid.Columns["Player1Remarks"].Index]}";
                    // ツールチップ設定の呪文。
                    // 第2引数に設定したい内容を設定
                    ToolTipService.SetToolTip(bdr, contents);
                }
            }
        }

        /// <summary>
        /// セルが廃却された場合、セルの境界のコンテンツも削除します
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cellType"></param>
        /// <param name="cell"></param>
        public override void DisposeCell(C1FlexGrid grid, CellType cellType, FrameworkElement cell)
        {
            Border bdr = (Border)cell;
            bdr.Child = null;
        }
    }
}
```
