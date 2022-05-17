using C1.WPF.FlexGrid;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RN3.Wpf.Common.Util.C1CellFactory
{
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
    }
}
