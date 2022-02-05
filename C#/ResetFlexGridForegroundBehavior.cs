using C1.WPF.FlexGrid;
using Microsoft.Xaml.Behaviors;
using RN3.Wpf.Common.Util.Extension;
using System.Windows.Controls;

namespace RN3.Wpf.Common.Behavior
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
            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= FlexGrid_SelectionChanged;
                AssociatedObject.SelectionChanged += FlexGrid_SelectionChanged;
            }
        }

        /// <summary>
        /// SelectionChangedイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlexGrid_SelectionChanged(object sender, CellRangeEventArgs e)
        {
            // カスタムテンプレート内のTextBlockの文字色がおかしくなるので、強制的に再設定
            for (int i = 0; i < AssociatedObject.Rows.Count; i++)
            {
                for (int j = 0; j < AssociatedObject.Columns.Count; j++)
                {
                    var cell = AssociatedObject.Cells.GetCellElement(new CellRange(i, j));
                    if (cell != null)
                    {
                        // 子孫要素からTextBlockを探索、選択箇所に応じてForegroundを再設定
                        var textBlockList = cell.FindVisualChildren<TextBlock>();
                        if (textBlockList != null)
                        {
                            foreach (var textBlock in textBlockList)
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
                }
            }
        }
    }
}
