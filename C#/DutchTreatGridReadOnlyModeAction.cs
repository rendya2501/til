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
        /// 読み取りモードにします。
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            // 割り勘金額行
            var dutchTreatAmount = AssociatedObject.Columns[DutchTreatGridColumnName.DutchTreatAmount];
            dutchTreatAmount.IsReadOnly = true;
            dutchTreatAmount.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
            // FlexGridを再描画
            AssociatedObject.Invalidate();
        }
    }
}
