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
Yr君がCellFactoryを使ったやり方で実現していたので拝借した。  
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
</i:Interaction.Triggers>
```

---

## FlexGridの描画をリフレッシュするサンプル

``` XML : 画面をリフレッシュするサンプル
<i:Interaction.Triggers>
    <l:InteractionMessageTrigger MessageKey="FlexGridInvalidateAction" Messenger="{Binding Messenger}">
        <i:CallMethodAction MethodName="InvalidateVisual" />
    </l:InteractionMessageTrigger>
</i:Interaction.Triggers>
```

``` C#
    Messenger.Raise(new InteractionMessage("FlexGridInvalidateAction"));
```

上記2つでやっていることは下記トリガーアクションと同じ  

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

## チェックボックスをFlexGrid由来のものを使おうとしてCustomFlexGridの中で頑張ろうとしたやつら

なぜかthis[0,0]でそのセルのデータを取ろうとしたときに、値が入ってなかったので、バインドできても値が吹っ飛んだ。  
なのでできなかったけど、それさえ解決できれば済む問題ではあるので、備忘録として残しておく。  

CheckBoxHelper
CellFactory

いまセルテン破棄してるから作り直さないといけないのかねぇ。

チェックボックスをFlexGridのものを使えばすべて解決する。
セルテンで色々やろうとするからややこしくなってる。
これ、普通にビヘイビアで制御できる。
もしValidFlagがFalseならReadOnly Trueにして背景色を灰色にすればいいだけ。

セルテンプレートでやるにしても影響範囲でかすぎる。
やるにしても色々考えないといけない。

``` XML
<c1:Column.CellTemplate>
    <DataTemplate>
        <Grid
            HorizontalAlignment="Stretch"
            VerticalAlignment="Stretch"
            IsEnabled="{Binding IsEnableCancel, Mode=OneWay}">
            <Grid.Style>
                <Style TargetType="Grid">
                    <Style.Triggers>
                        <DataTrigger Binding="{Binding IsDiffPartyCount, Mode=OneWay}" Value="True">
                            <Setter Property="Background">
                                <Setter.Value>
                                    <SolidColorBrush Opacity="0.5" Color="Red" />
                                </Setter.Value>
                            </Setter>
                        </DataTrigger>
                    </Style.Triggers>
                </Style>
            </Grid.Style>
            <CheckBox
                HorizontalAlignment="Center"
                VerticalAlignment="Center"
                IsChecked="{Binding IsSelectedCancel, Mode=TwoWay}"
                IsTabStop="False">
                <CheckBox.Style>
                    <Style BasedOn="{StaticResource {x:Type CheckBox}}" TargetType="{x:Type CheckBox}">
                        <Setter Property="LayoutTransform">
                            <Setter.Value>
                                <ScaleTransform ScaleX="0.8" ScaleY="0.8" />
                            </Setter.Value>
                        </Setter>
                        <Style.Triggers>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter Property="metro:CheckBoxHelper.CheckBackgroundFillUncheckedDisabled" Value="LightGray" />
                            </Trigger>
                        </Style.Triggers>
                    </Style>
                </CheckBox.Style>
            </CheckBox>
        </Grid>
    </DataTemplate>
</c1:Column.CellTemplate>

<c1:Column.CellTemplate>
    <DataTemplate>
        <Grid
            Width="70"
            HorizontalAlignment="Stretch"
            VerticalAlignment="Stretch"
            Background="Red"
            IsEnabled="{Binding IsEnableCancel, Mode=OneWay}">
            <CheckBox
                HorizontalAlignment="Center"
                VerticalAlignment="Center"
                HorizontalContentAlignment="Center"
                VerticalContentAlignment="Center"
                IsChecked="{Binding IsSelectedCancel, Mode=TwoWay}">
                <CheckBox.Style>
                    <Style BasedOn="{StaticResource {x:Type CheckBox}}" TargetType="{x:Type CheckBox}">
                        <Setter Property="LayoutTransform">
                            <Setter.Value>
                                <ScaleTransform ScaleX="0.8" ScaleY="0.8" />
                            </Setter.Value>
                        </Setter>
                        <Style.Triggers>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter Property="metro:CheckBoxHelper.CheckBackgroundFillUncheckedDisabled" Value="LightGray" />
                            </Trigger>
                        </Style.Triggers>
                    </Style>
                </CheckBox.Style>
            </CheckBox>
        </Grid>
    </DataTemplate>
</c1:Column.CellTemplate>

    <!--<Trigger Property="IsChecked" Value="True">
        <Setter Property="metro:CheckBoxHelper.CheckGlyphForegroundChecked" Value="Red" />
        <Setter Property="metro:CheckBoxHelper.CheckGlyphForegroundCheckedMouseOver" Value="Red" />
    </Trigger>-->
```

``` C#
// chk.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
//chk.SetValue(CheckBox.BackgroundProperty, (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"]);

    //チェックボックスの見た目変更
    foreach (var col in Columns
       .Where(w => w.CellTemplate != null && (w.DataType == typeof(bool) || w.DataType == typeof(bool?)))
       .ToList())
    {
       var template = new DataTemplate();
       var checkBox = new FrameworkElementFactory(typeof(CheckBox));
       //this[0,0]
       var aa = col.CellTemplate.VisualTree;
       checkBox.SetBinding(ToggleButton.IsCheckedProperty, col.Binding);
       template.VisualTree = new FrameworkElementFactory(typeof(Grid));
       template.VisualTree.AppendChild(checkBox);
       template.Seal();
       col.CellTemplate = template;
    }

    //チェックボックスの見た目変更
    foreach (var col in Columns.Where(w => w.CellTemplate != null).ToList())
    {
       var template = new DataTemplate();
       var checkBox = new FrameworkElementFactory(typeof(CheckBox));
       checkBox.SetBinding(ToggleButton.IsCheckedProperty, col.Binding);
       template.VisualTree = new FrameworkElementFactory(typeof(Grid));
       template.VisualTree.AppendChild(checkBox);
       template.Seal();
       col.CellTemplate = template;
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

---

``` XML
<ctrl:CustomFlexGrid
    x:Name="PaymentMethodGrid"
    Grid.Row="0"
    Grid.Column="1"
    Margin="10"
    AllowAddNew="False"
    AllowDragging="None"
    AllowFreezing="None"
    AllowResizing="None"
    AllowSorting="False"
    AutoGenerateColumns="False"
    GridLinesVisibility="All"
    HeadersVisibility="Column"
    IsReadOnly="{Binding CanEditPayment, Mode=OneWay, Converter={StaticResource NotConverter}}"
    ItemsSource="{Binding DepositTypeList, Mode=OneWay}"
    MenuVisibility="Collapsed"
    MinRowHeight="30"
    SelectionBackground="Transparent"
    SelectionMode="Cell">
    <i:Interaction.Behaviors>
        <behavior:ResetFlexGridForegroundBehavior />
    </i:Interaction.Behaviors>
    <ctrl:CustomFlexGrid.LayoutTransform>
        <ScaleTransform ScaleX="{Binding Magnification, Mode=TwoWay}" ScaleY="{Binding Magnification, Mode=TwoWay}" />
    </ctrl:CustomFlexGrid.LayoutTransform>
    <ctrl:CustomFlexGrid.Columns>
        <c1:Column
            Width="150"
            HorizontalAlignment="Stretch"
            VerticalAlignment="Stretch"
            Binding="{Binding DepositTypeName, Mode=OneWay}"
            ColumnName="DepositTypeName"
            Header="精算方法"
            HeaderHorizontalAlignment="Center"
            HeaderVerticalAlignment="Center"
            IsReadOnly="true">
            <c1:Column.CellTemplate>
                <DataTemplate>
                    <DockPanel HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
                        <DockPanel.Style>
                            <Style TargetType="DockPanel">
                                <Setter Property="Background" Value="{StaticResource MahApps.Brushes.Accent4}" />
                            </Style>
                        </DockPanel.Style>
                        <Button
                            Padding="0"
                            HorizontalAlignment="Stretch"
                            VerticalAlignment="Stretch"
                            HorizontalContentAlignment="Stretch"
                            VerticalContentAlignment="Stretch"
                            Command="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.SelectDepositTypeCommand, Mode=OneWay}"
                            CommandParameter="{Binding Mode=OneWay}"
                            IsTabStop="False"
                            Style="{StaticResource MahApps.Styles.Button.Flat}">
                            <DockPanel VerticalAlignment="Center">
                                <!-- <Label
                                    Height="Auto"
                                    HorizontalContentAlignment="Center"
                                    VerticalContentAlignment="Center"
                                    Content="{Binding DepositTypeName, Mode=OneWay}"
                                    Focusable="False">
                                    <Label.Style>
                                        <Style TargetType="Label">
                                            <Style.Triggers>
                                                <DataTrigger Binding="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.FormStatus, Mode=OneWay}" Value="Mode_New">
                                                    <Setter Property="Foreground" Value="{DynamicResource MahApps.Brushes.Gray2}" />
                                                </DataTrigger>
                                                <DataTrigger Binding="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.FormStatus, Mode=OneWay}" Value="Mode_Edit">
                                                    <Setter Property="Foreground" Value="Black" />
                                                </DataTrigger>
                                            </Style.Triggers>
                                        </Style>
                                    </Label.Style>
                                </Label> -->
                                <Label
                                    Margin="4,0,0,0"
                                    VerticalAlignment="Center"
                                    Content="{Binding DepositTypeName, Mode=OneWay}"
                                    DockPanel.Dock="Left" />
                                <Label
                                    Margin="0,0,8,0"
                                    HorizontalAlignment="Right"
                                    VerticalAlignment="Center"
                                    Content="▼"
                                    DockPanel.Dock="Right"
                                    Focusable="False"
                                    Visibility="{Binding DetailDisplayFlag, Mode=OneWay, Converter={StaticResource VisibleConverter}}" />
                            </DockPanel>
                        </Button>
                    </DockPanel>
                </DataTemplate>
            </c1:Column.CellTemplate>
        </c1:Column>
        <c1:Column
            Width="*"
            HorizontalAlignment="Right"
            VerticalAlignment="Center"
            Binding="{Binding Amount}"
            ColumnName="Amount"
            Header="精算金額"
            HeaderHorizontalAlignment="Center"
            HeaderVerticalAlignment="Center"
            IsReadOnly="True">
            <c1:Column.CellTemplate>
                <DataTemplate>
                    <im:GcNumber
                        x:Name="AmountTextBox"
                        Width="200"
                        Height="35"
                        HorizontalAlignment="Stretch"
                        VerticalAlignment="Stretch"
                        HorizontalContentAlignment="Right"
                        VerticalContentAlignment="Center"
                        AllowSpin="False"
                        Background="Transparent"
                        BorderThickness="0"
                        DisabledBackground="Transparent"
                        DisabledForeground="Black"
                        DisplayFieldSet="###,###,##0,,,-,"
                        FieldSet="###,###,##0,,,-,"
                        Focusable="True"
                        IsEnabled="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.CanEditPayment, Mode=OneWay}"
                        IsReadOnly="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.CanEditPayment, Mode=OneWay, Converter={StaticResource NotConverter}}"
                        IsTabStop="True"
                        SpinButtonVisibility="NotShown"
                        Value="{Binding Amount, Mode=TwoWay}" />
                </DataTemplate>
            </c1:Column.CellTemplate>
        </c1:Column>
    </ctrl:CustomFlexGrid.Columns>
</ctrl:CustomFlexGrid>
```

``` XML : ItemsControlにした
<ItemsControl
    x:Name="PaymentMethodGrid"
    Grid.Row="0"
    Grid.Column="1"
    MinWidth="120"
    Margin="10"
    BorderBrush="{StaticResource MahApps.Brushes.Gray5}"
    Focusable="False"
    ItemsSource="{Binding DepositTypeList, Mode=OneWay}">
    <ItemsControl.LayoutTransform>
        <ScaleTransform ScaleX="{Binding Magnification, Mode=TwoWay}" ScaleY="{Binding Magnification, Mode=TwoWay}" />
    </ItemsControl.LayoutTransform>
    <ItemsControl.Template>
        <ControlTemplate TargetType="{x:Type ItemsControl}">
            <Border BorderBrush="{StaticResource MahApps.Brushes.Accent}" BorderThickness="1">
                <ScrollViewer
                    Focusable="False"
                    HorizontalScrollBarVisibility="Auto"
                    VerticalScrollBarVisibility="Auto">
                    <i:Interaction.Triggers>
                        <l:InteractionMessageTrigger MessageKey="ResetPayMethodScrollAction" Messenger="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.Messenger, Mode=OneWay}">
                            <action:ReserScrollViewerAction />
                        </l:InteractionMessageTrigger>
                    </i:Interaction.Triggers>
                    <Grid>
                        <ItemsPresenter Margin="0,26,0,0" />
                        <Border VerticalAlignment="Top" IsHitTestVisible="False">
                            <Grid Background="White">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="3*" MinWidth="140" />
                                    <ColumnDefinition Width="4*" MinWidth="100" />
                                </Grid.ColumnDefinitions>
                                <Label
                                    Grid.Column="0"
                                    Height="Auto"
                                    Margin="0"
                                    HorizontalContentAlignment="Center"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="0,0,1,1"
                                    Content="精算方法"
                                    Style="{StaticResource AccentLabel}" />
                                <Label
                                    Grid.Column="1"
                                    Height="Auto"
                                    Margin="0"
                                    HorizontalContentAlignment="Center"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="0,0,0,1"
                                    Content="精算金額"
                                    Style="{StaticResource AccentLabel}" />
                            </Grid>
                        </Border>
                    </Grid>
                </ScrollViewer>
            </Border>
        </ControlTemplate>
    </ItemsControl.Template>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ItemsControl}}, Path=BorderBrush, Mode=OneWay}" BorderThickness="0,0,1,1">
                <Grid Height="Auto">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="3*" MinWidth="140" />
                        <ColumnDefinition Width="4*" MinWidth="100" />
                    </Grid.ColumnDefinitions>
                    <Button
                        HorizontalAlignment="Stretch"
                        VerticalAlignment="Stretch"
                        HorizontalContentAlignment="Stretch"
                        VerticalContentAlignment="Stretch"
                        Command="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.SelectDepositTypeCommand, Mode=OneWay}"
                        CommandParameter="{Binding Mode=OneWay}"
                        Focusable="False"
                        Style="{StaticResource MahApps.Styles.Button.Flat}">
                        <DockPanel VerticalAlignment="Center">
                            <TextBlock DockPanel.Dock="Left" Text="{Binding DepositTypeName, Mode=OneWay}" />
                            <TextBlock
                                Margin="0,0,8,0"
                                HorizontalAlignment="Right"
                                DockPanel.Dock="Right"
                                Text="▼"
                                Visibility="{Binding DetailDisplayFlag, Mode=OneWay, Converter={StaticResource VisibleConverter}}" />
                        </DockPanel>
                    </Button>
                    <im:GcNumber
                        Grid.Column="1"
                        HorizontalAlignment="Stretch"
                        HorizontalContentAlignment="Right"
                        AllowSpin="False"
                        BorderThickness="0"
                        DisabledBackground="Transparent"
                        DisabledForeground="Black"
                        DisplayFieldSet="###,###,##0,,,-,"
                        FieldSet="###,###,##0,,,-,"
                        IsReadOnly="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.CanEditPayment, Mode=OneWay, Converter={StaticResource NotConverter}}"
                        SpinButtonVisibility="NotShown"
                        Value="{Binding Amount, Mode=TwoWay}">
                        <im:GcNumber.Style>
                            <Style BasedOn="{StaticResource {x:Type im:GcNumber}}" TargetType="{x:Type im:GcNumber}">
                                <Setter Property="Background" Value="Transparent" />
                                <Style.Triggers>
                                    <Trigger Property="IsReadOnly" Value="True">
                                        <Setter Property="HighlightText" Value="False" />
                                        <Setter Property="Background" Value="Transparent" />
                                    </Trigger>
                                    <Trigger Property="IsActive" Value="True">
                                        <Setter Property="Background" Value="{StaticResource MahApps.Brushes.Accent4}" />
                                    </Trigger>
                                </Style.Triggers>
                            </Style>
                        </im:GcNumber.Style>
                    </im:GcNumber>
                </Grid>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

``` C#
    for (int i = 1; i <= 4; i++)
    {
        var playerCell = AssociatedObject.Columns[$"Player{i}Name"].CellTemplate.LoadContent().FindVisualChildren<TextBlock>();
        if (playerCell != null)
        {
            foreach (var textBlock in playerCell)
            {
                //if (AssociatedObject.Selection.Row == i && AssociatedObject.Selection.Column == AssociatedObject.Columns[$"Player{i}"])
                //{
                //    textBlock.Foreground = AssociatedObject.Foreground;
                //}
                //else if (AssociatedObject.Selection.Row == i)
                //{
                //    textBlock.Foreground = AssociatedObject.SelectionForeground;
                //}
                //else
                //{
                    textBlock.Foreground = AssociatedObject.Foreground;
                //}
            }
        }
    }
```

---

## 予約検索でFlexGridのCellTemplateを使うと文字が白くなる現象について

CellTemplateを使ったセルだけスクロールした時に白くなる。  
結果的にCellTemplateではなく、CellFactoryで同じようにコンポーネントをCreateしてChildにセットすることで白くなる現象を解決できた。  
CellTemplateでもCellFactoryでもやっていることは同じなのに、CellTemplateを使うと発生する模様。  
念のためデフォルトのC1FlexGridでもやってみたがもっとひどかった。  
これはFlexGridの仕様としてとらえたほうがいいかもしれない。  
多少の違いはあれど、やらせていることは同じ。不思議である。  
これだからFlexGridは嫌いなんだ。  

### XMLで書いていたことをセルファクトリーで記述できたのでまとめ

[方法: グリッド要素を作成する](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-create-a-grid-element?view=netframeworkdesktop-4.8)
[方法: ToolTip を配置する](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-position-a-tooltip?view=netframeworkdesktop-4.8)

``` XML
<!-- 一番上の要素でリソース定義 -->
<!-- こうしないと拡大率のバインドができなかった -->
<DockPanel.Resources>
    <Style x:Key="ToolTipTextBlock" TargetType="TextBlock">
        <Setter Property="LayoutTransform">
            <Setter.Value>
                <ScaleTransform ScaleX="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" ScaleY="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" />
            </Setter.Value>
        </Setter>
    </Style>
</DockPanel.Resources>

<c1:Column
    Width="100"
    VerticalAlignment="Center"
    Binding="{Binding Player2Name, Mode=OneWay}"
    ColumnName="Player2Name"
    Header="Player2"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center"
    IsReadOnly="True"
    TextWrapping="true">
    <c1:Column.CellTemplate>
        <DataTemplate>
            <Grid ToolTipService.IsEnabled="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}}">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                <ToolTipService.ToolTip>
                    <TextBlock Style="{DynamicResource ToolTipTextBlock}" Text="{Binding Player1Remarks, Mode=OneWay}" />
                </ToolTipService.ToolTip>
                <TextBlock
                    Grid.Column="0"
                    VerticalAlignment="Center"
                    Focusable="False"
                    Text="{Binding Player1Name, Mode=OneWay}"
                    TextWrapping="Wrap" />
                <TextBlock
                    Grid.Column="1"
                    Margin="0,0,5,0"
                    VerticalAlignment="Center"
                    Focusable="False"
                    Text="※"
                    Visibility="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource StringToVisibilityConverter}}" />
            </Grid>
        </DataTemplate>
    </c1:Column.CellTemplate>
</c1:Column>
```

``` C#
    /// <summary>
    /// セルコンテンツ作成
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="bdr"></param>
    /// <param name="rng"></param>
    public override void CreateCellContent(C1FlexGrid grid, Border bdr, CellRange rng)
    {
        // コンテンツを作成します
        base.CreateCellContent(grid, bdr, rng);

        // プレーヤー1~4までループ
        for (int i = 1; i <= 4; i++)
        {
            string nameColumnName = $"Player{i}Name";
            string remarksColumnName = $"Player{i}Remarks";
            // 指定したカラム名かどうか判定
            if (grid.Columns[rng.Column].ColumnName == nameColumnName)
            {
                 // 備考を取得
                string contents = (string)grid[rng.Row, remarksColumnName];

                // そのセルに備考が存在したらツールチップを設定
                if (!string.IsNullOrEmpty(contents))
                {
                    // タグにTextBlockおいて、そのTextにバインドして内容取れないかやってみたけど駄目だった。
                    var mnnn = (grid.Columns[nameColumnName].Tag is TextBlock ts) ? ts.Text : string.Empty;

                    // ツールチップ用テキストブロック定義
                    TextBlock tooltipBlock = new TextBlock
                    {
                        LayoutTransform = new ScaleTransform()
                        {
                            ScaleX = (double)grid.LayoutTransform.GetValue(ScaleTransform.ScaleXProperty),
                            ScaleY = (double)grid.LayoutTransform.GetValue(ScaleTransform.ScaleYProperty),
                        },
                        Text = contents
                    };
                    // セルにツールチップを設定
                    ToolTipService.SetToolTip(bdr, tooltipBlock);

                    // プレーヤー名テキストブロック定義
                    TextBlock nameBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Focusable = false,
                        TextWrapping = TextWrapping.Wrap,
                        Text = (string)grid[rng.Row, nameColumnName],
                    };
                    // ※マーク用テキストブロック定義
                    TextBlock noticeBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "※",
                        Margin = new Thickness(0, 0, 5, 0),
                        Focusable = false,
                    };
                    // テキストブロック格納Grid定義
                    Grid childGrid = new Grid();
                    childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });
                    childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    Grid.SetColumn(nameBlock, 0);
                    Grid.SetColumn(noticeBlock, 1);
                    _ = childGrid.Children.Add(nameBlock);
                    _ = childGrid.Children.Add(noticeBlock);
                    
                    // これをやるとGridにToolTipServiceを設定できる。
                    //ToolTipService.SetIsEnabled(childGrid, true);
                    //ToolTipService.SetToolTip(childGrid, tooltipBlock);

                    // 定義したGridをセル要素に割り当てる
                    bdr.Child = childGrid;
                }
            }
        }
    }
```

#### ボツ案

TagのTextBlockにあるバインド情報からリフレクション使って取得しようとしたり、  
CellTemplateのTextBlock要素だけを抽出して取得しようとしたがうまく取得できなかった。  
リフレクションに関しては取得の型が違うとエラーになり、要素の抽出に関しては結局空白しか抽出できなかった。  
備忘録としてまとめておく。  

``` C#
    // これは駄目だった。
    if ((bdr.Child is TextBlock t22s))
    {
        var cd = t22s.DataContext.GetType().GetProperty(remarksColumnName);
        var value = cd.GetValue(typeof(string),null);
    }
    // FindVisualChildrenはutilのメソッド。TextBlockまでは取れた
    var aa = grid.Columns[nameColumnName].CellTemplate.LoadContent().FindVisualChildren<TextBlock>();
    // 取得したTextBlockのTextには何も入っていなかった。
    if (grid.Columns[nameColumnName].CellTemplate.LoadContent() is TextBlock sss)
    {
        var bb = sss.Text;
    }
```

### 案2

最初はスクロールした時も色を変更するようにしたら行けると思っていたのだが、スクロールした端の部分までは変わってくれなかった。  
試しに再描画命令を記述したら変わるようになったが、体感できるレベルで重くなった。  
スクロールが若干かくつく感じ。  
スクロールの度に2重ループして文字色を再設定した後、すべてのセルを再描画するので当然といえば当然。  
これでもできるといえばできるが、なんか無理やり感が否めなかったので、

``` C#
using C1.WPF.FlexGrid;
using Microsoft.Xaml.Behaviors;
using RN3.Wpf.Common.Util.Extension;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RN3.Wpf.Reservation.ReservationSearch.Behaivor
{
    /// <summary>
    /// FlexGridのカスタムセル内にあるTextBlockのForegroundを再設定するビヘイビア
    /// </summary>
    public class ResetFlexGridForegroundBehavior : Behavior<C1FlexGrid>
    {
        /// <summary>
        /// イベント登録
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject is C1FlexGrid fg)
            {
                fg.SelectionChanged -= FlexGrid_CellRangeEventArgs;
                fg.SelectionChanged += FlexGrid_CellRangeEventArgs;
                fg.ScrollPositionChanged -= FlexGrid_EventArgs;
                fg.ScrollPositionChanged += FlexGrid_EventArgs;
            }
        }

        /// <summary>
        /// CellRangeEventArgsイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlexGrid_CellRangeEventArgs(object sender, CellRangeEventArgs e) => ReserForeground();
        /// <summary>
        /// EventArgsイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlexGrid_EventArgs(object sender, EventArgs e) => ReserForeground();

        /// <summary>
        /// TextBlockの文字色を再設定する
        /// </summary>
        private void ReserForeground()
        {
            // カスタムテンプレート内のTextBlockの文字色がおかしくなるので、強制的に再設定
            for (int i = 0; i < AssociatedObject.Rows.Count; i++)
            {
                for (int j = 0; j < AssociatedObject.Columns.Count; j++)
                {
                    var cell = AssociatedObject.Cells.GetCellElement(new CellRange(i, j));
                    // 子孫要素からTextBlockを探索、選択箇所に応じてForegroundを再設定
                    var textBlockList = cell?.FindVisualChildren<TextBlock>();
                    foreach (var textBlock in textBlockList ?? Enumerable.Empty<TextBlock>())
                    {
                        if (AssociatedObject.Selection.Row == i && AssociatedObject.Selection.Column == j)
                        {
                            textBlock.Foreground = AssociatedObject.Foreground;
                        }
                        else if (AssociatedObject.Selection.Row == i)
                        {
                            textBlock.Foreground = AssociatedObject.SelectionForeground;
                        }
                        else
                        {
                            textBlock.Foreground = AssociatedObject.Foreground;
                        }
                    }
                }
            }
            // 再描画命令を記述したら解決するけど重くなる。
            AssociatedObject.Invalidate();
        }
    }
}
```

### 最終案

最終的にこれで行けた。  
まずセルテンプレートにTextBlockを配置して備考をバインドさせてセルファクトリーで取得。  
CellFactory内部で改めてテンプレートを作ってセルテンプレートに上書きすることで備考がある場合とない場合の表示ができた。  

一番最初は備考カラムを作って、そのセルの内容を取得してたけど、エクスポートした時に備考まで出力されてしまう欠点があった。  
備考程度出力されても別にいいといえばいいけど、一応影響を受けないように作ることにした。  

次にTagにTextBlockをバインドさせて同じようにCellFactoryで取得しようとしたけどだめだった。  
どういうわけか空白にしかならない。  
中々いい案だと思っただけに残念だった。  
TagもBindに対応してくれればいいのだが。  

この考えを応用してCellTemplateで同じようにしたらいけた。  

``` XML
<c1:Column
    Width="100"
    VerticalAlignment="Center"
    Binding="{Binding Player1Name, Mode=OneWay}"
    ColumnName="Player1Name"
    Header="Player1"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center"
    IsReadOnly="True"
    TextWrapping="true">
    <c1:Column.CellTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Player1Remarks, Mode=OneWay}" />
        </DataTemplate>
    </c1:Column.CellTemplate>
    <!-- タグ案は駄目だった。 -->
    <!-- 記述自体に問題はなさそうなのだが、CellFactory側でTextが空白のままで何も取得できなかった。 -->
    <c1:Column.Tag>
        <TextBlock Text="{Binding Player1Remarks, Mode=OneWay}" />
    </c1:Column.Tag>
</c1:Column>
```

``` C#
    /// <summary>
    /// セルコンテンツ作成
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="bdr"></param>
    /// <param name="rng"></param>
    public override void CreateCellContent(C1FlexGrid grid, Border bdr, CellRange rng)
    {
        // コンテンツを作成します
        base.CreateCellContent(grid, bdr, rng);

        // プレーヤー1~4までループ
        for (int i = 1; i <= 4; i++)
        {
            string nameColumnName = $"Player{i}Name";
            // string remarksColumnName = $"Player{i}Remarks";
            // 指定したカラム名かどうか判定
            if (grid.Columns[rng.Column].ColumnName == nameColumnName)
            {
                // プレーヤー名テキストブロック定義
                TextBlock nameBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Focusable = false,
                    TextWrapping = TextWrapping.Wrap,
                    Text = (string)grid[rng.Row, nameColumnName],
                };
                // 備考マーク用テキストブロック定義
                TextBlock noticeBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "※",
                    Margin = new Thickness(0, 0, 5, 0),
                    Focusable = false,
                };
                // テキストブロック格納Grid定義
                Grid childGrid = new Grid();
                childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });
                childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                Grid.SetColumn(nameBlock, 0);
                _ = childGrid.Children.Add(nameBlock);

                // セルテンプレートの備考を取得
                var contents = (bdr.Child is TextBlock ts) ? ts.Text : string.Empty;
                // 備考が存在したらそのセルにツールチップを設定
                if (!string.IsNullOrEmpty(contents))
                {
                    // ツールチップ用テキストブロック定義
                    TextBlock tooltipBlock = new TextBlock
                    {
                        LayoutTransform = new ScaleTransform()
                        {
                            ScaleX = (double)grid.LayoutTransform.GetValue(ScaleTransform.ScaleXProperty),
                            ScaleY = (double)grid.LayoutTransform.GetValue(ScaleTransform.ScaleYProperty),
                        },
                        Text = contents
                    };
                    // プレーヤーiのセルにツールチップを設定
                    ToolTipService.SetToolTip(bdr, tooltipBlock);
                    // 備考マークテキストブロックをgridに追加
                    Grid.SetColumn(noticeBlock, 1);
                    _ = childGrid.Children.Add(noticeBlock);
                }

                // 定義したGridをセル要素に割り当てる
                bdr.Child = childGrid;
            }
        }
    }
```

---

## イベント順

[[C#/WPF] Windowクラスのイベント全部、発生時にログ取ってみる](https://zenn.dev/tera1707/articles/594f392077ea80)

FlexGridの「検索→セルを適当にクリック」した時のイベント発火の流れを記録したので残す。  

``` C#
    // ログをデスクトップに残すためのクラス
    static class LogOnDesktop
    {
        public static void WriteLogToDesktopLogFile(string line)
        {
            var logPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log.log";

            File.AppendAllText(logPath, DateTime.Now.ToString("hh:mm:ss.fff") + "  " + line);
            File.AppendAllText(logPath, Environment.NewLine);
        }
    }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditWindow()
        {
            InitializeComponent();

            FlexGrid.DraggedFrozenRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggedFrozenRow"); });
            FlexGrid.DraggedColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggedColumn"); });
            FlexGrid.DraggedRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggedRow"); });
            FlexGrid.DraggingColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggingColumn"); });
            FlexGrid.DraggingRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggingRow"); });
            FlexGrid.LoadingRows += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LoadingRows"); });
            FlexGrid.GroupCollapsedChanging += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GroupCollapsedChanging"); });
            FlexGrid.GroupCollapsedChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GroupCollapsedChanged"); });
            FlexGrid.PrepareCellForEdit += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PrepareCellForEdit"); });
            FlexGrid.CellEditEnding += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("CellEditEnding"); });
            FlexGrid.RowEditEnding += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("RowEditEnding"); });
            FlexGrid.DraggedFrozenColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggedFrozenColumn"); });
            FlexGrid.LoadedRows += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LoadedRows"); });
            FlexGrid.DraggingFrozenRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggingFrozenRow"); });
            FlexGrid.ItemsSourceChanging += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ItemsSourceChanging"); });
            FlexGrid.ResizedRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ResizedRow"); });
            FlexGrid.CustomAggregate += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("CustomAggregate"); });
            FlexGrid.ItemsSourceChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ItemsSourceChanged"); });
            FlexGrid.Click += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("Click"); });
            FlexGrid.DoubleClick += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DoubleClick"); });
            FlexGrid.ScrollPositionChanging += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ScrollPositionChanging"); });
            FlexGrid.ScrollPositionChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ScrollPositionChanged"); });
            FlexGrid.DraggingFrozenColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DraggingFrozenColumn"); });
            FlexGrid.ScrollingDeferred += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ScrollingDeferred"); });
            FlexGrid.SelectionChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("SelectionChanged"); });
            FlexGrid.SortingColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("SortingColumn"); });
            FlexGrid.SortedColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("SortedColumn"); });
            FlexGrid.ResizingColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ResizingColumn"); });
            FlexGrid.ResizingRow += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ResizingRow"); });
            FlexGrid.ResizedColumn += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ResizedColumn"); });
            FlexGrid.SelectionChanging += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("SelectionChanging"); });
            FlexGrid.SelectedItemChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("SelectedItemChanged"); });
            FlexGrid.RowEditEnded += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("RowEditEnded"); });
            FlexGrid.CellEditEnded += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("CellEditEnded"); });

            FlexGrid.KeyUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("KeyUp"); });
            FlexGrid.TouchMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TouchMove"); });
            FlexGrid.PreviewTouchMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewTouchMove"); });
            FlexGrid.TouchDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TouchDown"); });
            FlexGrid.PreviewTouchDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewTouchDown"); });
            FlexGrid.Drop += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("Drop"); });
            FlexGrid.PreviewDrop += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewDrop"); });
            FlexGrid.DragLeave += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DragLeave"); });
            FlexGrid.PreviewDragLeave += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewDragLeave"); });
            FlexGrid.DragOver += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DragOver"); });
            FlexGrid.PreviewDragOver += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewDragOver"); });
            FlexGrid.DragEnter += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("DragEnter"); });
            FlexGrid.PreviewDragEnter += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewDragEnter"); });
            FlexGrid.GiveFeedback += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GiveFeedback"); });
            FlexGrid.PreviewGiveFeedback += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewGiveFeedback"); });
            FlexGrid.QueryContinueDrag += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("QueryContinueDrag"); });
            FlexGrid.PreviewQueryContinueDrag += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewQueryContinueDrag"); });
            FlexGrid.TextInput += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TextInput"); });
            FlexGrid.PreviewTouchUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewTouchUp"); });
            FlexGrid.TouchUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TouchUp"); });
            FlexGrid.LostTouchCapture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LostTouchCapture"); });
            FlexGrid.PreviewTextInput += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewTextInput"); });
            FlexGrid.ManipulationInertiaStarting += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationInertiaStarting"); });
            FlexGrid.ManipulationDelta += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationDelta"); });
            FlexGrid.ManipulationStarted += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationStarted"); });
            FlexGrid.ManipulationStarting += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationStarting"); });
            FlexGrid.FocusableChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("FocusableChanged"); });
            FlexGrid.IsHitTestVisibleChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsHitTestVisibleChanged"); });
            //FlexGrid.IsEnabledChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsEnabledChanged"); });
            FlexGrid.LostFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LostFocus"); });
            FlexGrid.GotTouchCapture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GotTouchCapture"); });
            FlexGrid.GotFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GotFocus"); });
            FlexGrid.IsKeyboardFocusedChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsKeyboardFocusedChanged"); });
            FlexGrid.IsStylusCaptureWithinChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsStylusCaptureWithinChanged"); });
            FlexGrid.IsStylusDirectlyOverChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsStylusDirectlyOverChanged"); });
            //FlexGrid.IsMouseCaptureWithinChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsMouseCaptureWithinChanged"); });
            //FlexGrid.IsMouseCapturedChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsMouseCapturedChanged"); });
            FlexGrid.IsKeyboardFocusWithinChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsKeyboardFocusWithinChanged"); });
            //FlexGrid.IsMouseDirectlyOverChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsMouseDirectlyOverChanged"); });
            FlexGrid.TouchLeave += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TouchLeave"); });
            FlexGrid.TouchEnter += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("TouchEnter"); });
            FlexGrid.LostKeyboardFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LostKeyboardFocus"); });
            FlexGrid.PreviewLostKeyboardFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewLostKeyboardFocus"); });
            FlexGrid.GotKeyboardFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GotKeyboardFocus"); });
            FlexGrid.PreviewStylusMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusMove"); });
            FlexGrid.StylusMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusMove"); });
            FlexGrid.PreviewStylusInAirMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusInAirMove"); });
            FlexGrid.StylusInAirMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusInAirMove"); });
            FlexGrid.StylusEnter += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusEnter"); });
            FlexGrid.StylusLeave += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusLeave"); });
            FlexGrid.PreviewStylusInRange += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusInRange"); });
            FlexGrid.StylusInRange += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusInRange"); });
            FlexGrid.PreviewStylusOutOfRange += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusOutOfRange"); });
            FlexGrid.StylusOutOfRange += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusOutOfRange"); });
            FlexGrid.PreviewStylusSystemGesture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusSystemGesture"); });
            FlexGrid.StylusSystemGesture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusSystemGesture"); });
            FlexGrid.GotStylusCapture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("GotStylusCapture"); });
            FlexGrid.LostStylusCapture += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("LostStylusCapture"); });
            FlexGrid.StylusButtonDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusButtonDown"); });
            FlexGrid.StylusButtonUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusButtonUp"); });
            FlexGrid.PreviewStylusButtonDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusButtonDown"); });
            FlexGrid.PreviewStylusButtonUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusButtonUp"); });
            FlexGrid.PreviewKeyDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewKeyDown"); });
            FlexGrid.KeyDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("KeyDown"); });
            FlexGrid.PreviewKeyUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewKeyUp"); });
            FlexGrid.StylusUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusUp"); });
            FlexGrid.PreviewGotKeyboardFocus += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewGotKeyboardFocus"); });
            FlexGrid.PreviewStylusUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusUp"); });
            FlexGrid.PreviewStylusDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewStylusDown"); });
            //FlexGrid.PreviewMouseDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewMouseDown"); });
            //FlexGrid.MouseDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("MouseDown"); });
            //FlexGrid.PreviewMouseUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewMouseUp"); });
            //FlexGrid.MouseUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("MouseUp"); });
            //FlexGrid.PreviewMouseLeftButtonDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewMouseLeftButtonDown"); });
            //FlexGrid.MouseLeftButtonDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("MouseLeftButtonDown"); });
            //FlexGrid.PreviewMouseLeftButtonUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewMouseLeftButtonUp"); });
            //FlexGrid.MouseLeftButtonUp += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("MouseLeftButtonUp"); });
            //FlexGrid.PreviewMouseMove += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("PreviewMouseMove"); });
            //FlexGrid.QueryCursor += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("QueryCursor"); });
            FlexGrid.StylusDown += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("StylusDown"); });
            FlexGrid.IsStylusCapturedChanged += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("IsStylusCapturedChanged"); });
            FlexGrid.ManipulationCompleted += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationCompleted"); });
            FlexGrid.ManipulationBoundaryFeedback += ((obj, ev) => { LogOnDesktop.WriteLogToDesktopLogFile("ManipulationBoundaryFeedback"); });
        }
```

``` log
11:13:07.514  ItemsSourceChanging
11:13:07.519  LoadingRows
11:13:07.522  SelectionChanging
11:13:07.525  SelectedItemChanged
11:13:07.588  PrepareCellForEdit
11:13:07.609  SelectionChanged
11:13:07.612  LoadedRows
11:13:07.639  SelectedItemChanged
11:13:07.641  CellEditEnding
11:13:07.644  CellEditEnded
11:13:07.648  SelectionChanging
11:13:07.718  SelectionChanged
11:13:07.722  SelectedItemChanged
11:13:07.725  RowEditEnding
11:13:07.728  RowEditEnded
11:13:07.746  SelectionChanging
11:13:07.819  PrepareCellForEdit
11:13:07.831  SelectionChanged
11:13:07.834  ItemsSourceChanged
11:13:08.116  PreviewGotKeyboardFocus
11:13:08.120  IsKeyboardFocusWithinChanged
11:13:08.123  IsKeyboardFocusedChanged
11:13:08.127  GotFocus
11:13:08.130  GotKeyboardFocus
11:13:15.680  CellEditEnding
11:13:15.684  CellEditEnded
11:13:15.693  RowEditEnding
11:13:15.696  RowEditEnded
11:13:15.701  SelectionChanging
11:13:15.704  SelectedItemChanged
11:13:15.724  PrepareCellForEdit
11:13:15.732  PreviewLostKeyboardFocus
11:13:15.735  PreviewGotKeyboardFocus
11:13:15.738  IsKeyboardFocusedChanged
11:13:15.742  LostKeyboardFocus
11:13:15.744  LostFocus
11:13:15.747  PreviewLostKeyboardFocus
11:13:15.750  PreviewGotKeyboardFocus
11:13:15.755  GotFocus
11:13:15.759  GotKeyboardFocus
11:13:15.763  SelectionChanged
11:13:15.765  SelectionChanging
11:13:15.770  PreviewLostKeyboardFocus
11:13:15.772  PreviewGotKeyboardFocus
11:13:15.776  LostKeyboardFocus
11:13:15.778  LostFocus
11:13:15.782  PreviewLostKeyboardFocus
11:13:15.784  PreviewGotKeyboardFocus
11:13:15.787  GotFocus
11:13:15.791  GotKeyboardFocus
11:13:15.794  PreviewLostKeyboardFocus
11:13:15.797  PreviewGotKeyboardFocus
11:13:15.800  LostKeyboardFocus
11:13:15.803  LostFocus
11:13:15.806  PreviewLostKeyboardFocus
11:13:15.809  PreviewGotKeyboardFocus
11:13:15.812  GotFocus
11:13:15.815  GotKeyboardFocus
11:13:15.822  CellEditEnding
11:13:15.825  CellEditEnded
11:13:15.837  PrepareCellForEdit
11:13:15.848  PreviewLostKeyboardFocus
11:13:15.851  PreviewGotKeyboardFocus
11:13:15.854  LostKeyboardFocus
11:13:15.857  LostFocus
11:13:15.859  PreviewLostKeyboardFocus
11:13:15.862  PreviewGotKeyboardFocus
11:13:15.865  GotFocus
11:13:15.869  GotKeyboardFocus
11:13:15.872  Click
11:13:15.876  PreviewLostKeyboardFocus
11:13:15.879  PreviewGotKeyboardFocus
11:13:15.882  IsKeyboardFocusedChanged
11:13:15.889  LostKeyboardFocus
11:13:15.892  LostFocus
11:13:15.895  GotFocus
11:13:15.898  GotKeyboardFocus
11:13:15.902  PreviewLostKeyboardFocus
11:13:15.905  PreviewGotKeyboardFocus
11:13:15.908  IsKeyboardFocusedChanged
11:13:15.911  LostKeyboardFocus
11:13:15.914  LostFocus
11:13:15.916  PreviewLostKeyboardFocus
11:13:15.919  PreviewGotKeyboardFocus
11:13:15.924  GotFocus
11:13:15.928  GotKeyboardFocus
11:13:15.931  PreviewLostKeyboardFocus
11:13:15.934  PreviewGotKeyboardFocus
11:13:15.937  LostKeyboardFocus
11:13:15.940  LostFocus
11:13:15.943  PreviewLostKeyboardFocus
11:13:15.945  PreviewGotKeyboardFocus
11:13:15.948  GotFocus
11:13:15.952  GotKeyboardFocus
11:13:18.236  CellEditEnding
11:13:18.240  CellEditEnded
11:13:18.249  RowEditEnding
11:13:18.253  RowEditEnded
11:13:18.258  LostFocus
11:13:18.272  IsKeyboardFocusWithinChanged
```

---

## クリックした場所がヘッダーかどうかを判定し、クリックした場所の情報を表示するサンプル

``` C#
    private void FlexGrid_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is C1FlexGrid fg)
        {
            var ht = fg.HitTest(e);
            // ヘッダー
            if (ht.CellType == CellType.ColumnHeader)
            {
                
            }
            // 普通のセル
            else
            {
                fg.StartEditing(true);
            }
            MessageBox.Show($"クリックされたセル{Environment.NewLine}行：{ ht.Row}{Environment.NewLine}列：{ht.Column}");
        }
    }
```
