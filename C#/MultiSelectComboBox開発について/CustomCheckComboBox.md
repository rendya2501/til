# CustomCheckComboBox

``` C# : CustomCheckComboBox.xaml
using System.Collections;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace namespace
{
    /// <summary>
    /// 拡張チェックコンボボックス
    /// </summary>
    public class CustomCheckComboBox : CheckComboBox
    {
        #region プロパティ
        #region MaxHeaderItemsProperty
        /// <summary>
        /// MaxHeaderItemsProperty依存関係プロパティ登録
        /// </summary>
        public static readonly DependencyProperty MaxHeaderItemsProperty =
            DependencyProperty.Register(
                "MaxHeaderItems",
                typeof(int),
                typeof(CustomCheckComboBox),
                new PropertyMetadata(0)
            );
        /// <summary>
        /// コントロールヘッダーに表示する項目の最大数を取得または設定します。
        /// </summary>
        public int MaxHeaderItems
        {
            get { return (int)GetValue(MaxHeaderItemsProperty); }
            set { SetValue(MaxHeaderItemsProperty, value); }
        }
        #endregion

        #region HeaderFormatProperty
        /// <summary>
        /// MaxHeaderItemsProperty依存関係プロパティ登録
        /// </summary>
        public static readonly DependencyProperty HeaderFormatProperty =
            DependencyProperty.Register(
                "HeaderFormat",
                typeof(string),
                typeof(CustomCheckComboBox),
                new PropertyMetadata("items selected")
            );
        /// <summary>
        /// コントロール内の選択項目が maxHeaderItems より多いときに、ヘッダーコンテンツの作成に使用される書式文字列を取得または設定します。
        /// </summary>
        public string HeaderFormat
        {
            get { return (string)GetValue(HeaderFormatProperty); }
            set { SetValue(HeaderFormatProperty, value); }
        }
        #endregion
        #endregion


        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomCheckComboBox() { }
        #endregion


        #region 内部処理
        /// <summary>
        /// SelectedItemsOverrideを変更した時に呼び出されます。
        /// ViewModelからCustomCheckComboBoxに対応する処理です。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnSelectedItemsOverrideChanged(IList oldValue, IList newValue)
        {
            // 1件もない場合は全解除
            if (newValue.Count <= 0)
            {
                UnSelectAll();
                base.OnSelectedItemsOverrideChanged(oldValue, newValue);
            }
            else
            {
                // SelectedItemsOverrideの元の処理を実行することで、選択状態を反映させる。
                base.OnSelectedItemsOverrideChanged(oldValue, newValue);
                // 「全て選択」ボタンが有効な場合
                if (IsSelectAllActive)
                {
                    // 全選択ボタン有効無効設定依存プロパティに紐づくイベントを発火することで、全選択ボタンをnullに設定する。
                    // 開発元のソースを見たとき、この方法が一番副作用がなくて、全選択ボタンをnullにすることができるのでこうしている。
                    base.OnIsSelectAllActiveChanged(false, true);
                }
            }
        }

        /// <summary>
        /// テキストが変更されたときに呼び出されます。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnTextChanged(string oldValue, string newValue)
        {
            // 最大数が設定されているときだけ文言を変更する
            if (MaxHeaderItems <= 0)
            {
                return;
            }
            // 選択件数が設定した最大表示件数を超えた場合、設定した文言に変更する。
            // そうでなければ、選択したアイテム名を羅列して表示する
            Text = SelectedItems.Count > MaxHeaderItems
                ? $"{SelectedItems.Count}{HeaderFormat}"
                : newValue;
        }
        #endregion
    }
}
```

``` XML : DesignResourceDictionary.xaml
    <!--#region xceed CheckComboBox-->
    <xctk:InverseBoolConverter x:Key="InverseBoolConverter" />
    <!--  ドロップダウンしたときの各アイテムのスタイル  -->
    <Style BasedOn="{StaticResource {x:Type xctk:SelectorItem}}" TargetType="{x:Type xctk:SelectorItem}">
        <Setter Property="Background" Value="{DynamicResource MahApps.Brushes.ThemeBackground}" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Padding" Value="2" />
        <Setter Property="RenderOptions.ClearTypeHint" Value="Enabled" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type xctk:SelectorItem}">
                    <Grid Background="{TemplateBinding Background}">
                        <Border
                            x:Name="_background"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}">
                            <CheckBox
                                x:Name="_checkBox"
                                Margin="0.5,0"
                                Padding="{TemplateBinding Padding}"
                                VerticalAlignment="{TemplateBinding VerticalContentAlignment}"
                                HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                                IsChecked="{Binding IsSelected, RelativeSource={RelativeSource TemplatedParent}}">
                                <CheckBox.Content>
                                    <ContentControl
                                        Content="{TemplateBinding Content}"
                                        ContentTemplate="{TemplateBinding ContentTemplate}"
                                        ContentTemplateSelector="{TemplateBinding ContentTemplateSelector}"
                                        Focusable="False"
                                        Foreground="{TemplateBinding Foreground}" />
                                </CheckBox.Content>
                                <CheckBox.LayoutTransform>
                                    <TransformGroup>
                                        <ScaleTransform ScaleX="0.9" ScaleY="0.9" />
                                    </TransformGroup>
                                </CheckBox.LayoutTransform>
                            </CheckBox>
                        </Border>
                    </Grid>
                    <ControlTemplate.Triggers>
                        <MultiTrigger>
                            <MultiTrigger.Conditions>
                                <Condition Property="IsSelected" Value="true" />
                                <Condition Property="IsMouseOver" Value="false" />
                            </MultiTrigger.Conditions>
                            <Setter TargetName="_background" Property="Background" Value="{DynamicResource MahApps.Brushes.Accent4}" />
                        </MultiTrigger>
                        <MultiTrigger>
                            <MultiTrigger.Conditions>
                                <Condition Property="IsSelected" Value="false" />
                                <Condition Property="IsMouseOver" Value="true" />
                            </MultiTrigger.Conditions>
                            <Setter TargetName="_background" Property="Background" Value="{DynamicResource MahApps.Brushes.Accent4}" />
                        </MultiTrigger>
                        <Trigger Property="IsSelected" Value="true">
                            <Setter TargetName="_background" Property="Background" Value="{DynamicResource MahApps.Brushes.Accent}" />
                            <Setter Property="Foreground" Value="{DynamicResource MahApps.Brushes.Selected.Foreground}" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    <!--  全選択ボタンのスタイル  -->
    <Style
        x:Key="SelectAllSelectorItem"
        BasedOn="{StaticResource {x:Type xctk:SelectorItem}}"
        TargetType="{x:Type xctk:SelectAllSelectorItem}" />
    <!--  選択したアイテムの表示テキストとドロップダウンボタンのスタイル  -->
    <Style x:Key="CheckComboBoxToggleButton" TargetType="{x:Type ToggleButton}">
        <Setter Property="OverridesDefaultStyle" Value="true" />
        <Setter Property="IsTabStop" Value="false" />
        <Setter Property="Focusable" Value="false" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type ToggleButton}">
                    <Grid x:Name="ToggleButtonRootGrid" Background="{TemplateBinding Background}">
                        <Border
                            x:Name="Background"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" />
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="0" MinWidth="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight, Mode=OneWay}" />
                            </Grid.ColumnDefinitions>
                            <!--  選択した要素を表示するテキスト部分  -->
                            <TextBox
                                x:Name="PART_ClearText"
                                Width="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualWidth, Mode=OneWay}"
                                Height="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}, Path=ActualHeight, Mode=OneWay}"
                                HorizontalAlignment="{TemplateBinding HorizontalAlignment}"
                                VerticalAlignment="{TemplateBinding VerticalAlignment}"
                                VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                                Background="Transparent"
                                BorderThickness="0"
                                Cursor="Arrow"
                                Focusable="False"
                                HorizontalScrollBarVisibility="Hidden"
                                IsReadOnly="True"
                                SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                                Text="{TemplateBinding Content}"
                                VerticalScrollBarVisibility="Hidden" />
                            <!--  ▼部分  -->
                            <Grid
                                x:Name="ArrowBackground"
                                Grid.Column="1"
                                Background="Transparent">
                                <Path
                                    x:Name="Arrow"
                                    Width="8"
                                    Height="4"
                                    HorizontalAlignment="Center"
                                    VerticalAlignment="Center"
                                    Data="F1 M 301.14,-189.041L 311.57,-189.041L 306.355,-182.942L 301.14,-189.041 Z "
                                    Fill="{DynamicResource MahApps.Brushes.Gray1}"
                                    IsHitTestVisible="false"
                                    SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                                    Stretch="Uniform" />
                            </Grid>
                        </Grid>
                    </Grid>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="Background" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray8}" />
                            <Setter TargetName="ArrowBackground" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray8}" />
                        </Trigger>
                        <Trigger SourceName="ArrowBackground" Property="IsMouseOver" Value="True">
                            <Setter TargetName="ArrowBackground" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray5}" />
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="false">
                            <Setter TargetName="Arrow" Property="Fill" Value="{DynamicResource MahApps.Brushes.Gray7}" />
                        </Trigger>
                        <DataTrigger Binding="{Binding IsDropDownOpen, Mode=OneWay, RelativeSource={RelativeSource TemplatedParent}}" Value="True">
                            <Setter TargetName="PART_ClearText" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray8}" />
                            <Setter TargetName="ArrowBackground" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray8}" />
                            <Setter TargetName="ToggleButtonRootGrid" Property="Background" Value="{DynamicResource MahApps.Brushes.Gray7}" />
                        </DataTrigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    <!--  チェックボックス本体  -->
    <Style BasedOn="{StaticResource {x:Type xctk:CheckComboBox}}" TargetType="{x:Type ctrl:CustomCheckComboBox}">
        <Setter Property="MinHeight" Value="26" />
        <Setter Property="Background" Value="{DynamicResource MahApps.Brushes.Control.Background}" />
        <Setter Property="BorderBrush" Value="{DynamicResource MahApps.Brushes.TextBox.Border}" />
        <Setter Property="BorderThickness" Value="{DynamicResource ComboBoxBorderThemeThickness}" />
        <Setter Property="Foreground" Value="{DynamicResource MahApps.Brushes.Text}" />
        <Setter Property="Padding" Value="1,5,0,0" />
        <Setter Property="Stylus.IsFlicksEnabled" Value="False" />
        <Setter Property="HorizontalContentAlignment" Value="Left" />
        <Setter Property="VerticalContentAlignment" Value="Center" />
        <Setter Property="ScrollViewer.HorizontalScrollBarVisibility" Value="Auto" />
        <Setter Property="ScrollViewer.VerticalScrollBarVisibility" Value="Auto" />
        <Setter Property="ScrollViewer.PanningMode" Value="Both" />
        <Setter Property="SnapsToDevicePixels" Value="True" />
        <Setter Property="Validation.ErrorTemplate" Value="{DynamicResource MahApps.Templates.ValidationError}" />
        <Setter Property="ScrollViewer.CanContentScroll" Value="False" />
        <Setter Property="metro:ControlsHelper.FocusBorderBrush" Value="{DynamicResource MahApps.Brushes.ComboBox.Border.Focus}" />
        <Setter Property="metro:ControlsHelper.MouseOverBorderBrush" Value="{DynamicResource MahApps.Brushes.ComboBox.Border.MouseOver}" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type ctrl:CustomCheckComboBox}">
                    <Grid x:Name="MainGrid" SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}">
                        <Popup
                            x:Name="PART_Popup"
                            AllowsTransparency="true"
                            Focusable="False"
                            IsOpen="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                            Placement="Bottom"
                            PopupAnimation="{DynamicResource {x:Static SystemParameters.ComboBoxPopupAnimationKey}}"
                            StaysOpen="False">

                            <Grid MinWidth="{Binding ActualWidth, ElementName=MainGrid}" MaxHeight="{Binding MaxDropDownHeight, RelativeSource={RelativeSource TemplatedParent}}">
                                <Border
                                    x:Name="DropDownBorder"
                                    Height="Auto"
                                    Background="{DynamicResource MahApps.Brushes.ThemeBackground}"
                                    BorderBrush="{DynamicResource MahApps.Brushes.ComboBox.PopupBorder}"
                                    BorderThickness="1"
                                    Effect="{DynamicResource DropShadowBrush}">
                                    <ScrollViewer x:Name="DropDownScrollViewer">
                                        <Grid RenderOptions.ClearTypeHint="Enabled">
                                            <Canvas
                                                Width="0"
                                                Height="0"
                                                HorizontalAlignment="Left"
                                                VerticalAlignment="Top">
                                                <Rectangle
                                                    x:Name="OpaqueRect"
                                                    Width="{Binding ActualWidth, ElementName=DropDownBorder}"
                                                    Height="{Binding ActualHeight, ElementName=DropDownBorder}"
                                                    Fill="{Binding Background, ElementName=DropDownBorder}" />
                                            </Canvas>
                                            <StackPanel>
                                                <!--  全てボタン  -->
                                                <xctk:SelectAllSelectorItem
                                                    x:Name="PART_SelectAllSelectorItem"
                                                    Content="{TemplateBinding SelectAllContent}"
                                                    SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                                                    Style="{DynamicResource SelectAllSelectorItem}"
                                                    Visibility="{Binding IsSelectAllActive, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource InverseBoolConverter}}" />
                                                <!--  各アイテム  -->
                                                <ItemsPresenter
                                                    x:Name="PART_ItemsPresenter"
                                                    KeyboardNavigation.DirectionalNavigation="Contained"
                                                    SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" />
                                            </StackPanel>
                                        </Grid>
                                    </ScrollViewer>
                                </Border>
                            </Grid>
                        </Popup>

                        <Border
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" />
                        <!--  平常時の見た目  -->
                        <ToggleButton
                            x:Name="PART_DropDownButton"
                            Margin="0"
                            Padding="{TemplateBinding Padding}"
                            VerticalAlignment="Stretch"
                            HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                            VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            Content="{TemplateBinding Text}"
                            Focusable="False"
                            Foreground="{TemplateBinding Foreground}"
                            IsChecked="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                            IsHitTestVisible="{Binding IsDropDownOpen, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource InverseBoolConverter}}"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                            Style="{DynamicResource CheckComboBoxToggleButton}" />
                        <!--  フォーカスが当たった時の見た目  -->
                        <Border
                            x:Name="FocusBorder"
                            Background="{x:Null}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                            Visibility="Collapsed" />
                        <!--  EnabledがFalseの時の見た目  -->
                        <Border
                            x:Name="DisabledVisualElement"
                            Background="{DynamicResource MahApps.Brushes.Control.Disabled}"
                            BorderBrush="{DynamicResource MahApps.Brushes.Control.Disabled}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            IsHitTestVisible="False"
                            Opacity="0.6"
                            SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                            Visibility="Collapsed" />
                    </Grid>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="FocusBorder" Property="Visibility" Value="Visible" />
                            <Setter TargetName="FocusBorder" Property="BorderBrush" Value="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=(metro:ControlsHelper.MouseOverBorderBrush)}" />
                        </Trigger>
                        <Trigger Property="IsFocused" Value="True">
                            <Setter TargetName="FocusBorder" Property="Visibility" Value="Visible" />
                            <Setter TargetName="FocusBorder" Property="BorderBrush" Value="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=(metro:ControlsHelper.FocusBorderBrush)}" />
                        </Trigger>
                        <Trigger Property="IsKeyboardFocusWithin" Value="True">
                            <Setter TargetName="FocusBorder" Property="Visibility" Value="Visible" />
                            <Setter TargetName="FocusBorder" Property="BorderBrush" Value="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=(metro:ControlsHelper.FocusBorderBrush)}" />
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter TargetName="DisabledVisualElement" Property="Visibility" Value="Visible" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    <!--#endregion-->
```

``` XML : xceed
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:chrome="clr-namespace:Xceed.Wpf.Toolkit.Chromes"
    xmlns:conv="clr-namespace:Xceed.Wpf.Toolkit.Core.Converters"
    xmlns:local="clr-namespace:Xceed.Wpf.Toolkit"
    xmlns:prim="clr-namespace:Xceed.Wpf.Toolkit.Primitives">

    <conv:InverseBoolConverter x:Key="InverseBoolConverter" />
    <BooleanToVisibilityConverter x:Key="BoolToVisibilityConverter" />

    <SolidColorBrush x:Key="ButtonNormalBorder" Color="#FF707070" />

    <Geometry x:Key="DownArrowGeometry">M 0,1 C0,1 0,0 0,0 0,0 3,0 3,0 3,0 3,1 3,1 3,1 4,1 4,1 4,1 4,0 4,0 4,0 7,0 7,0 7,0 7,1 7,1 7,1 6,1 6,1 6,1 6,2 6,2 6,2 5,2 5,2 5,2 5,3 5,3 5,3 4,3 4,3 4,3 4,4 4,4 4,4 3,4 3,4 3,4 3,3 3,3 3,3 2,3 2,3 2,3 2,2 2,2 2,2 1,2 1,2 1,2 1,1 1,1 1,1 0,1 0,1 z</Geometry>

    <Style x:Key="ComboBoxToggleButton" TargetType="{x:Type ToggleButton}">
        <Setter Property="OverridesDefaultStyle" Value="true" />
        <Setter Property="IsTabStop" Value="false" />
        <Setter Property="Focusable" Value="false" />
        <Setter Property="Padding" Value="2" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type ToggleButton}">
                    <chrome:ButtonChrome
                        x:Name="Chrome"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        CornerRadius="0"
                        RenderEnabled="{TemplateBinding IsEnabled}"
                        RenderMouseOver="{Binding IsMouseOver, ElementName=PART_DropDownButton}"
                        RenderNormal="False"
                        RenderPressed="{Binding IsPressed, ElementName=PART_DropDownButton}"
                        SnapsToDevicePixels="true">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="0" MinWidth="{DynamicResource {x:Static SystemParameters.VerticalScrollBarWidthKey}}" />
                            </Grid.ColumnDefinitions>

                            <TextBox
                                x:Name="TextBox"
                                Margin="{TemplateBinding Padding}"
                                HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                VerticalAlignment="{TemplateBinding VerticalContentAlignment}"
                                Background="Transparent"
                                BorderThickness="0"
                                Cursor="Arrow"
                                Focusable="False"
                                Foreground="{TemplateBinding Foreground}"
                                HorizontalScrollBarVisibility="Hidden"
                                IsReadOnly="True"
                                IsTabStop="{TemplateBinding IsTabStop}"
                                SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                                Text="{Binding Content, RelativeSource={RelativeSource TemplatedParent}, Mode=TwoWay}"
                                VerticalScrollBarVisibility="Hidden" />
                            <Grid
                                Grid.Column="1"
                                Width="{DynamicResource {x:Static SystemParameters.VerticalScrollBarWidthKey}}"
                                HorizontalAlignment="Right">
                                <Path
                                    x:Name="Arrow"
                                    Margin="3,0,3,0"
                                    HorizontalAlignment="Center"
                                    VerticalAlignment="Center"
                                    Data="{StaticResource DownArrowGeometry}"
                                    Fill="Black" />
                            </Grid>
                        </Grid>
                    </chrome:ButtonChrome>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsChecked" Value="true">
                            <Setter TargetName="Chrome" Property="RenderPressed" Value="true" />
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="false">
                            <Setter TargetName="Arrow" Property="Fill" Value="#AFAFAF" />
                        </Trigger>
                        <DataTrigger Binding="{Binding IsEditable, RelativeSource={RelativeSource AncestorType={x:Type local:CheckComboBox}}}" Value="True">
                            <Setter TargetName="TextBox" Property="IsReadOnly" Value="False" />
                            <Setter TargetName="TextBox" Property="Focusable" Value="True" />
                            <Setter TargetName="TextBox" Property="Cursor" Value="{x:Null}" />
                            <Setter Property="IsTabStop" Value="True" />
                        </DataTrigger>
                        <MultiDataTrigger>
                            <MultiDataTrigger.Conditions>
                                <Condition Binding="{Binding IsEditable, RelativeSource={RelativeSource AncestorType={x:Type local:CheckComboBox}}}" Value="True" />
                                <Condition Binding="{Binding IsFocused, RelativeSource={RelativeSource AncestorType={x:Type local:CheckComboBox}}}" Value="True" />
                            </MultiDataTrigger.Conditions>
                            <Setter TargetName="TextBox" Property="FocusManager.FocusedElement" Value="{Binding ElementName=TextBox}" />
                        </MultiDataTrigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style TargetType="{x:Type local:CheckComboBox}">
        <Setter Property="Foreground" Value="{DynamicResource {x:Static SystemColors.WindowTextBrushKey}}" />
        <Setter Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
        <Setter Property="BorderBrush" Value="{StaticResource ButtonNormalBorder}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="ScrollViewer.HorizontalScrollBarVisibility" Value="Auto" />
        <Setter Property="ScrollViewer.VerticalScrollBarVisibility" Value="Auto" />
        <Setter Property="Padding" Value="2" />
        <Setter Property="ScrollViewer.CanContentScroll" Value="true" />
        <Setter Property="ScrollViewer.PanningMode" Value="Both" />
        <Setter Property="Stylus.IsFlicksEnabled" Value="False" />
        <Setter Property="VerticalContentAlignment" Value="Center" />
        <Setter Property="HorizontalContentAlignment" Value="Stretch" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type local:CheckComboBox}">
                    <Grid x:Name="MainGrid" SnapsToDevicePixels="true">
                        <Popup
                            x:Name="PART_Popup"
                            Margin="1"
                            AllowsTransparency="true"
                            IsOpen="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                            Placement="Bottom"
                            PopupAnimation="{DynamicResource {x:Static SystemParameters.ComboBoxPopupAnimationKey}}"
                            StaysOpen="False">
                            <Grid MinWidth="{Binding ActualWidth, ElementName=MainGrid}">
                                <Border
                                    x:Name="DropDownBorder"
                                    MaxHeight="{Binding MaxDropDownHeight, RelativeSource={RelativeSource TemplatedParent}}"
                                    Background="{DynamicResource {x:Static SystemColors.WindowBrushKey}}"
                                    BorderBrush="{DynamicResource {x:Static SystemColors.WindowFrameBrushKey}}"
                                    BorderThickness="1">
                                    <ScrollViewer x:Name="DropDownScrollViewer">
                                        <Grid RenderOptions.ClearTypeHint="Enabled">
                                            <Canvas
                                                Width="0"
                                                Height="0"
                                                HorizontalAlignment="Left"
                                                VerticalAlignment="Top">
                                                <Rectangle
                                                    x:Name="OpaqueRect"
                                                    Width="{Binding ActualWidth, ElementName=DropDownBorder}"
                                                    Height="{Binding ActualHeight, ElementName=DropDownBorder}"
                                                    Fill="{Binding Background, ElementName=DropDownBorder}" />
                                            </Canvas>
                                            <StackPanel>
                                                <prim:SelectAllSelectorItem
                                                    x:Name="PART_SelectAllSelectorItem"
                                                    Content="{TemplateBinding SelectAllContent}"
                                                    Visibility="{Binding IsSelectAllActive, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource BoolToVisibilityConverter}}" />
                                                <ItemsPresenter
                                                    x:Name="PART_ItemsPresenter"
                                                    KeyboardNavigation.DirectionalNavigation="Contained"
                                                    SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" />
                                            </StackPanel>
                                        </Grid>
                                    </ScrollViewer>
                                </Border>
                            </Grid>
                        </Popup>

                        <Border
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}" />
                        <ToggleButton
                            x:Name="PART_DropDownButton"
                            Padding="{TemplateBinding Padding}"
                            HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                            VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            Content="{Binding Text, RelativeSource={RelativeSource TemplatedParent}, Mode=TwoWay}"
                            Focusable="False"
                            IsChecked="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                            IsHitTestVisible="{Binding IsDropDownOpen, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource InverseBoolConverter}}"
                            Style="{StaticResource ComboBoxToggleButton}" />
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
        <Style.Triggers>
            <Trigger Property="IsEditable" Value="True">
                <Setter Property="IsTabStop" Value="False" />
            </Trigger>
        </Style.Triggers>
    </Style>

</ResourceDictionary>
```
