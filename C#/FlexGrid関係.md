# FlexGridに関して色々

## いつぞやのサンプル

```C#:XAML
                                    <c1:Column
                                        Width="290"
                                        HorizontalAlignment="Center"
                                        VerticalAlignment="Center"
                                        AllowMerging="True"
                                        ColumnName="Product"
                                        Foreground="Black"
                                        Header="商品"
                                        HeaderHorizontalAlignment="Center"
                                        HeaderVerticalAlignment="Center">
                                        <c1:Column.CellTemplate>
                                            <!--<DataTemplate DataType="{x:Type entity:SetProductView}">
                                                <StackPanel Orientation="Horizontal">
                                                    <ctrl:CustomTextBox
                                                        Width="70"
                                                        Height="22"
                                                        BorderThickness="0"
                                                        Foreground="Black"
                                                        Style="{StaticResource CustomTextBoxImeOff}"
                                                        Text="{Binding ProductCD, Mode=TwoWay}" />
                                                    <ctrl:SearchBox
                                                        Width="220"
                                                        Height="22"
                                                        VerticalContentAlignment="Center"
                                                        BorderThickness="0"
                                                        DropDownWidth="750"
                                                        Foreground="Black"
                                                        IsBusy="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType=Window}, Path=DataContext.IsBusyProductList, Mode=OneWay}"
                                                        IsTabStop="False"
                                                        ItemsSource="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType=Window}, Path=DataContext.SimpleProductList, Mode=OneWay}"
                                                        SearchCommand="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType=Window}, Path=DataContext.ProductSearchCommand, Mode=OneWay}"
                                                        SearchText="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType=Window}, Path=DataContext.ProductSearchText, Mode=TwoWay}"
                                                        SelectedItem="{Binding SelectedProduct, Mode=TwoWay}"
                                                        Text="{Binding ProductName, Mode=TwoWay}" />
                                                </StackPanel>
                                            </DataTemplate>-->
                                            <DataTemplate>
                                                <StackPanel Orientation="Horizontal">
                                                    <ctrl:CustomTextBox
                                                        Width="70"
                                                        Height="22"
                                                        BorderThickness="0"
                                                        Foreground="Black"
                                                        Style="{StaticResource CustomTextBoxImeOff}"
                                                        Text="{Binding ProductCD, Mode=TwoWay}" />
                                                    <im:GcTextBox
                                                        Width="200"
                                                        Height="23"
                                                        Padding="3,0,0,0"
                                                        VerticalAlignment="Center"
                                                        HorizontalContentAlignment="Left"
                                                        Background="{StaticResource IsReadOnlyBackGroundColor}"
                                                        BorderThickness="0"
                                                        Foreground="Black"
                                                        IsEnabled="False"
                                                        IsReadOnly="True"
                                                        Text="{Binding ProductName}" />
                                                    <Button
                                                        Width="23"
                                                        Height="23"
                                                        HorizontalContentAlignment="Center"
                                                        VerticalContentAlignment="Center"
                                                        Background="Transparent"
                                                        BorderThickness="0"
                                                        Command="{Binding ShowProductListCommand, Mode=OneWay}">
                                                        <Image Source="{StaticResource Black_Search_16}" Stretch="None" />
                                                    </Button>
                                                </StackPanel>
                                            </DataTemplate>
                                        </c1:Column.CellTemplate>
                                    </c1:Column>
                                    <!--<c1:Column
                                        Width="220"
                                        HorizontalAlignment="Left"
                                        VerticalAlignment="Center"
                                        AllowMerging="True"
                                        Binding="{Binding ProductCD, Mode=TwoWay}"
                                        ColumnName="ProductList"
                                        Header="商品"
                                        HeaderHorizontalAlignment="Center"
                                        HeaderVerticalAlignment="Center" />-->
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
CellFactoryを使ったやり方で萬君が実現していたので、拝借した。  
そのほかにも、チェックアウトで疑似的に実現していたのでそれも拝借する。  

``` XML : Front.CheckOut.Views.SelfCheckOutWindow.xaml
    <c1:C1FlexGrid
        CellFactory="{StaticResource RowHeaderNumberingCellFactory}">
        <c1:C1FlexGrid.Columns>
            <c1:Column>
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
```
