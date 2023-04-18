using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ui_test;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool isDragging;
    private UIElement selectedElement;
    private Point clickPosition;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddRectangleButton_Click(object sender, RoutedEventArgs e)
    {
        var rectangle = new Rectangle
        {
            Width = 100,
            Height = 50,
            Fill = Brushes.Red,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        rectangle.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
        rectangle.MouseMove += Shape_MouseMove;
        rectangle.MouseLeftButtonUp += Shape_MouseLeftButtonUp;

        Canvas.SetLeft(rectangle, 0);
        Canvas.SetTop(rectangle, 0);

        MyCanvas.Children.Add(rectangle);
    }

    private void AddEllipseButton_Click(object sender, RoutedEventArgs e)
    {
        var ellipse = new Ellipse
        {
            Width = 100,
            Height = 50,
            Fill = Brushes.Blue,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        ellipse.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
        ellipse.MouseMove += Shape_MouseMove;
        ellipse.MouseLeftButtonUp += Shape_MouseLeftButtonUp;

        Canvas.SetLeft(ellipse, 0);
        Canvas.SetTop(ellipse, 0);

        MyCanvas.Children.Add(ellipse);
    }

    private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isDragging = true;
        selectedElement = sender as UIElement;
        clickPosition = e.GetPosition(MyCanvas);
        selectedElement.CaptureMouse();
    }

    private void Shape_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging && selectedElement != null)
        {
            var currentPosition = e.GetPosition(MyCanvas);
            var offsetX = currentPosition.X - clickPosition.X;
            var offsetY = currentPosition.Y - clickPosition.Y;
            Canvas.SetLeft(selectedElement, Canvas.GetLeft(selectedElement) + offsetX);
            Canvas.SetTop(selectedElement, Canvas.GetTop(selectedElement) + offsetY);
            clickPosition = currentPosition;
        }
    }

    private void Shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (isDragging)
        {
            selectedElement.ReleaseMouseCapture();
            isDragging = false;
        }
    }
}


public class ConnectionLine
{
    public UIElement StartElement { get; set; }
    public UIElement EndElement { get; set; }
    public Line Line { get; set; }

    public ConnectionLine()
    {
        Line = new Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
    }
}
