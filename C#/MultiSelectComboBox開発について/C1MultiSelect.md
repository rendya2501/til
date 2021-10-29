# C1MultiSelect

作ったはいいけど、結局XceedのCheckComboBoxを使うことにしたので、完成形だけ拝借して本番から消す。
受け手はIListにしてOfTypeで変換して使うか、ObjservableCollectionにしてCollectionChangedを観測するしか変更通知を受け取ることができない。  
この記事に関しては、「DependencyProperyのSetterの値がNullになってしまう問題」としてまとめたのそちらを参照されたし。

## コード

``` C# : 完成版
using C1.WPF.Input;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RN3.Wpf.Common.Control
{
    /// <summary>
    /// C1MultiSelect拡張クラス
    /// </summary>
    public class CustomMultiSelect : C1MultiSelect
    {
        #region プロパティ
        #region 依存関係プロパティ
        /// <summary>
        /// SelectedItems依存関係プロパティ登録
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(IList),
                typeof(CustomMultiSelect),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged))
            );
        /// <summary>
        /// 現在選択されている行に対応するデータ項目のコレクションです。
        /// </summary>
        public new IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        #endregion

        /// <summary>
        /// OnSelectionChangedイベント抑制フラグ
        /// </summary>
        /// <remarks>
        /// overrideしたイベントは登録と解除ができないのでフラグで制御する
        /// </remarks>
        private bool OnSelectionChangedSuppressionFlag { get; set; } = false;
        #endregion


        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomMultiSelect() { }
        #endregion


        #region 内部処理
        /// <summary>
        /// SelectedItems変更イベント
        /// ViewModelからCustomMultiSelectに対応する処理です。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomMultiSelect)d;
            if (control?.ListBox == null || control?.SelectedItems == null)
            {
                return;
            }
            // OnSelectionChangedの抑制
            control.OnSelectionChangedSuppressionFlag = true;
            // 衝突を避けるため内部の選択状態をクリアする
            control.ListBox.SelectedItems.Clear();
            // フロントから代入された選択項目
            foreach (var frontItem in control.SelectedItems)
            {
                // CustomMultiSelectが保持している項目
                foreach (var sourceItem in control.ListBox.ItemsSource)
                {
                    // 保持しているアイテムと同じ選択項目をコントロール内部の選択項目に反映させる
                    if (sourceItem == frontItem)
                    {
                        // 内部の選択状態はC1MultiSelectのListBoxを操作することで反映される。
                        control.ListBox.SelectedItems.Add(frontItem);
                        break;
                    }
                }
            }
            // OnSelectionChangedの抑制解除
            control.OnSelectionChangedSuppressionFlag = false;
        }

        /// <summary>
        /// コンボボックスが選択された時に発動します。
        /// CustomMultiSelectからViewModelに対応する処理です。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnSelectionChangedSuppressionFlag)
            {
                return;
            }
            // シャローコピー。
            var items = new List<object>();
            foreach (var item in SelectedItems ?? new List<object>())
            {
                items.Add(item);
            }
            // 衝突の危険性があるので先に削除する
            foreach (var deleteItem in e.RemovedItems ?? new List<object>())
            {
                items.Remove(deleteItem);
            }
            // 追加は削除の後に実行
            foreach (var addItem in e.AddedItems ?? new List<object>())
            {
                items.Add(addItem);
            }
            SelectedItems = items;

            base.OnSelectionChanged(sender, e);
        }
        #endregion
    }
}
```

``` XML : Style
    <!--#region C1MultiSelect-->
    <Style x:Key="C1MultiSelectStyle1" TargetType="{x:Type c1:C1MultiSelect}">
        <Setter Property="Background" Value="{StaticResource InputBackgroundBrush}" />
        <Setter Property="Foreground" Value="{StaticResource ForegroundTextBoxBrushColor}" />
        <Setter Property="MouseOverBrush" Value="{StaticResource MouseOverBrush}" />
        <Setter Property="SelectedBackground" Value="{StaticResource SelectedBackground}" />
        <Setter Property="BorderBrush" Value="{StaticResource WpfInputBorderBrush}" />
        <Setter Property="BorderThickness" Value="{StaticResource GeneralThickness}" />
        <Setter Property="HighlightColor" Value="{StaticResource ForegroundTextBoxBrushColor}" />
        <Setter Property="SelectAllCaption" Value="{Binding CheckList_SelectAllCaption, Source={StaticResource InputResources}}" />
        <Setter Property="UnSelectAllCaption" Value="{Binding CheckList_UnSelectAllCaption, Source={StaticResource InputResources}}" />
        <Setter Property="ShowCheckBoxes" Value="true" />
        <Setter Property="AutoCompleteMode" Value="Suggest" />
        <Setter Property="ShowSelectAll" Value="false" />
        <Setter Property="HighlightFontWeight" Value="Bold" />
        <Setter Property="CaptionStyle" Value="{StaticResource DefaultCaptionStyle}" />
        <Setter Property="HorizontalContentAlignment" Value="Left" />
        <Setter Property="VerticalContentAlignment" Value="Top" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type c1:C1MultiSelect}">
                    <Grid>
                        <c1:C1DropDown
                            x:Name="DropDown"
                            Padding="0"
                            AutoClose="true"
                            AutoSizeMode="GrowAndShrink"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            DisabledCuesVisibility="Visible"
                            DropDownHeight="{TemplateBinding DropdownHeight}"
                            DropDownWidth="{TemplateBinding DropdownWidth}"
                            HeaderPadding="0"
                            IsDropDownOpen="{Binding IsDroppedDown, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                            IsEnabled="{TemplateBinding IsEnabled}"
                            MaxDropDownHeight="{TemplateBinding MaxDropDownHeight}"
                            MaxDropDownWidth="{TemplateBinding MaxDropDownWidth}"
                            MinDropDownHeight="{TemplateBinding MinDropDownHeight}"
                            MinDropDownWidth="{TemplateBinding MinDropDownWidth}"
                            MouseOverBrush="{TemplateBinding MouseOverBrush}"
                            PressedBrush="{TemplateBinding PressedBrush}"
                            ShowButton="{TemplateBinding ShowDropDownButton}">
                            <c1:C1DropDown.Header>
                                <Grid>
                                    <c1:C1TagEditor
                                        x:Name="Editor"
                                        HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                                        VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                                        Background="{TemplateBinding Background}"
                                        BorderThickness="0"
                                        DisplayMode="{TemplateBinding DisplayMode}"
                                        Foreground="{TemplateBinding Foreground}"
                                        IsEditable="{TemplateBinding IsEditable}"
                                        IsTagEditable="{TemplateBinding IsTagEditable}"
                                        PlaceHolder="{TemplateBinding PlaceHolder}"
                                        Separator="{TemplateBinding Separator}"
                                        TagStyle="{TemplateBinding TagStyle}" />
                                    <ContentControl
                                        x:Name="HeaderFormat"
                                        Margin="{TemplateBinding Padding}"
                                        HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                        VerticalAlignment="{TemplateBinding VerticalContentAlignment}"
                                        Background="{TemplateBinding Background}"
                                        FontFamily="{TemplateBinding FontFamily}"
                                        FontSize="{TemplateBinding FontSize}"
                                        Foreground="{TemplateBinding Foreground}"
                                        IsHitTestVisible="false"
                                        IsTabStop="false"
                                        Visibility="Collapsed" />
                                </Grid>
                            </c1:C1DropDown.Header>
                            <Grid>
                                <c1:C1CheckList
                                    x:Name="CheckList"
                                    HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                                    VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                                    Background="{TemplateBinding Background}"
                                    BindingGroup="{TemplateBinding BindingGroup}"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="{TemplateBinding BorderThickness}"
                                    CaptionStyle="{TemplateBinding CaptionStyle}"
                                    CheckedMemberPath="{TemplateBinding CheckedMemberPath}"
                                    DisabledMemberPath="{TemplateBinding DisabledMemberPath}"
                                    DisplayMemberPath="{TemplateBinding DisplayMemberPath}"
                                    Foreground="{TemplateBinding Foreground}"
                                    IsTextSearchCaseSensitive="{TemplateBinding IsTextSearchCaseSensitive}"
                                    ItemBindingGroup="{TemplateBinding ItemBindingGroup}"
                                    ItemContainerStyle="{TemplateBinding ItemContainerStyle}"
                                    ItemTemplate="{TemplateBinding ItemTemplate}"
                                    ItemTemplateSelector="{TemplateBinding ItemTemplateSelector}"
                                    ItemsSource="{TemplateBinding ItemsSource}"
                                    MouseOverBrush="{TemplateBinding MouseOverBrush}"
                                    SelectAllCaption="{TemplateBinding SelectAllCaption}"
                                    SelectedBackground="{TemplateBinding SelectedBackground}"
                                    SelectedIndex="{Binding SelectedIndex, Mode=TwoWay, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                                    SelectedItem="{Binding SelectedItem, Mode=TwoWay, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                                    SelectedValue="{Binding SelectedValue, Mode=TwoWay, RelativeSource={RelativeSource Mode=TemplatedParent}}"
                                    SelectedValuePath="{TemplateBinding SelectedValuePath}"
                                    SelectionMode="{TemplateBinding SelectionMode}"
                                    ShowCheckBoxes="{TemplateBinding ShowCheckBoxes}"
                                    ShowSelectAll="{TemplateBinding ShowSelectAll}"
                                    UnSelectAllCaption="{TemplateBinding UnSelectAllCaption}" />
                                <c1:C1CheckList
                                    x:Name="SuggestList"
                                    HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                                    VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                                    Background="{TemplateBinding Background}"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="{TemplateBinding BorderThickness}"
                                    Foreground="{TemplateBinding Foreground}"
                                    ItemStringFormat="{TemplateBinding ItemStringFormat}"
                                    MouseOverBrush="{TemplateBinding MouseOverBrush}"
                                    SelectedBackground="{TemplateBinding SelectedBackground}"
                                    SelectionMode="Single"
                                    ShowCheckBoxes="false"
                                    Visibility="Collapsed" />
                            </Grid>
                        </c1:C1DropDown>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    <!--#endregion-->

```

``` C# : ViewModel
        /// <summary>
        /// 科目大区分選択項目一覧
        /// IList型で受け取る必要がある。
        /// </summary>
        public IList SubjectLargeTypeSelectedItemList
        {
            get { return _SubjectLargeTypeSelectedItemList; }
            set { SetProperty(ref _SubjectLargeTypeSelectedItemList, value); }
        }
        private IList _SubjectLargeTypeSelectedItemList
            = new List<SubjectLargeTypeWithSubjectCDList>();

        private void SubjectLargeTypeSelectedChanged()
        {
            // 選択した科目大区分の伝票のみを抽出して金額を反映させる。
            var subjectCDList = SubjectLargeTypeList
                .Where(w =>
                    SubjectLargeTypeSelectedItemList
                        // 使うときはOfTypeで変換して使用する。
                        .OfType<SubjectLargeTypeWithSubjectCDList>()
                        .Select(s => s.SubjectLargeTypeCD)
                        .Contains(w.SubjectLargeTypeCD)
                )
                .SelectMany(s => s.SubjectCDList)
                .ToList();
        }
```

---

## 試行錯誤版

``` C# : 最初のバージョン
    /// <summary>
    /// C1MultiSelect拡張コントロール
    /// </summary>
    public class CustomMultiSelect : C1MultiSelect
    {
        /// <summary>
        /// SelectedItemsプロパティ登録
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(IList),
                typeof(CustomMultiSelect),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged))
            );
        /// <summary>
        /// 現在選択されている行に対応するデータ項目のコレクションです。
        /// </summary>
        public new IList SelectedItems
        {
            // newをつけると、元のプロパティを隠ぺいする。
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        /// <summary>
        /// OnSelectionChangedイベント抑制フラグ
        /// </summary>
        /// <remarks>
        /// overrideしたイベントは登録と解除ができないのでフラグで制御する
        /// </remarks>
        private bool OnSelectionChangedSuppressionFlag { get; set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomMultiSelect() { }

        /// <summary>
        /// SelectedItems変更イベント
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomMultiSelect)d;
            if (control.SelectedItems is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged -= control.OnCollectionChanged;
                notify.CollectionChanged += control.OnCollectionChanged;
            }

            // 試行錯誤の後
            //if (d != null)
            //{
            //    //fg.SetValue(
            //    //    SelectedItemsProperty,
            //    //    () => SelectedItems.Add(e.NewValue)
            //    //);
            //    if (control.ListBox != null)
            //    {
            //        control.ListBox.SelectedItems.Add(e.NewValue);
            //    }
            //}

            //if (d is CustomMultiSelect multiselect)
            //{
            //    multiselect.SelectionChanged += (s, eventArgs) =>
            //    {
            //        multiselect.SelectedItems.Add(s);
            //        //var items = (List)multiselect.SelectedItems;
            //        //if (items == null)
            //        //{
            //        //    return;
            //        //}
            //        //foreach (var item in items.Where(x => x.IsSelected == true))
            //        //{
            //        //    multiselect.SelectedItems.Add(item);
            //        //}
            //    };
            //    //// 削除
            //    //foreach (var deleteItem in args.RemovedItems.OfType<SubjectLargeTypeWithSubjectCDList>())
            //    //{
            //    //    multiselect.Remove(deleteItem);
            //    //}
            //    //// 追加
            //    //foreach (var addItem in args.AddedItems.OfType<SubjectLargeTypeWithSubjectCDList>())
            //    //{
            //    //    multiselect.Add(addItem);
            //    //}
            //}
        }

        /// <summary>
        /// SelectedItemsコレクション変更イベント
        /// OneWayに対応する処理です。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// 主にViewModel側から選択状態を反映させるときに発動する。
        /// 選択項目の反映はC1MultiSelectのプロパティであるListBoxのSelectedItemsを操作することで反映されることが分かった。
        /// </remarks>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // SelectionChanged抑制開始
            OnSelectionChangedSuppressionFlag = true;
            // 要素の追加、削除、クリアに応じた処理を実行します。
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: foreach (var item in e.NewItems) ListBox.SelectedItems.Add(item); break;
                case NotifyCollectionChangedAction.Remove: foreach (var item in e.OldItems) ListBox.SelectedItems.Remove(item); break;
                case NotifyCollectionChangedAction.Reset: ListBox.SelectedItems.Clear(); break;
                default: break;
            }
            // SelectionChanged抑制終了
            OnSelectionChangedSuppressionFlag = false;
        }

        /// <summary>
        /// コンボボックスが選択された時に発動します。
        /// ToSourceに対応する処理です。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // CollectionChangedが発動しているときは処理をしない
            if (OnSelectionChangedSuppressionFlag) return;
            // コンボボックスの選択状態をSelectedItemsに反映させる
            if (SelectedItems is INotifyCollectionChanged notify)
            {
                // SelectedItemsのAddとRemoveを実行するのでCollectionChangedイベントを抑制する
                notify.CollectionChanged -= OnCollectionChanged;
                // 衝突の危険性があるので先に削除する
                foreach (var deleteItem in e.RemovedItems) SelectedItems.Remove(deleteItem);
                // 後から追加する
                foreach (var addItem in e.AddedItems) SelectedItems.Add(addItem);
                // イベント再登録
                notify.CollectionChanged += OnCollectionChanged;
            }
            base.OnSelectionChanged(sender, e);
        }
    }
```
