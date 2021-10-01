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
        <!-- 行けたと思ったんだけど、 -->
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
