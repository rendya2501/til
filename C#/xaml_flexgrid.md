# XAML_Sample

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