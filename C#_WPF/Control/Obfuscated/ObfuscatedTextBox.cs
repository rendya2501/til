using System.Windows;
using System.Windows.Controls;
using System;

namespace Obfuscated;

public class ObfuscatedTextBox : TextBox
{
    public static readonly DependencyProperty IsObfuscatedProperty =
        DependencyProperty.Register(
            nameof(IsObfuscated),
            typeof(bool),
            typeof(ObfuscatedTextBox),
            new PropertyMetadata(false, OnIsObfuscatedChanged)
        );

    public bool IsObfuscated
    {
        get => (bool)GetValue(IsObfuscatedProperty);
        set => SetValue(IsObfuscatedProperty, value);
    }

    //public static readonly DependencyProperty DisplayTextProperty =
    //    DependencyProperty.RegisterReadOnly(
    //        nameof(DisplayText),
    //        typeof(string),
    //        typeof(ObfuscatedTextBox),
    //        new PropertyMetadata(null)
    //    ).DependencyProperty;

    public static readonly DependencyProperty DisplayTextProperty =
          DependencyProperty.Register(
              nameof(DisplayText),
              typeof(string),
              typeof(ObfuscatedTextBox),
              new PropertyMetadata(null)
          );

    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    private static void OnIsObfuscatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var textBox = d as ObfuscatedTextBox;
        textBox?.UpdateDisplayText();
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (IsObfuscated)
        {
            int length = Text.Length;
            DisplayText = length <= 3
                ? new string('*', length)
                : string.Concat(Text.AsSpan(0, 1), new string('*', length - 2), Text.AsSpan(length - 1, 1));
        }
        else
        {
            DisplayText = Text;
        }
    }
}



//private ObfuscationAdorner _obfuscationAdorner;

//public static readonly DependencyProperty IsObfuscatedProperty = DependencyProperty.Register(nameof(IsObfuscated), typeof(bool), typeof(CustomTextBox), new PropertyMetadata(false, OnIsObfuscatedChanged));

//public bool IsObfuscated
//{
//    get => (bool)GetValue(IsObfuscatedProperty);
//    set => SetValue(IsObfuscatedProperty, value);
//}

//public CustomTextBox()
//{
//    Loaded += CustomTextBox_Loaded;
//    this.TextChanged += CustomTextBox_TextChanged;
//}

//private void CustomTextBox_Loaded(object sender, RoutedEventArgs e)
//{
//    AttachObfuscationAdorner();
//}

//private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e)
//{
//    if (_obfuscationAdorner != null && IsObfuscated)
//    {
//        _obfuscationAdorner.InvalidateVisual();
//    }
//}

//private static void OnIsObfuscatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//{
//    var customTextBox = d as CustomTextBox;
//    customTextBox?.UpdateObfuscation();
//}

//private void AttachObfuscationAdorner()
//{
//    if (_obfuscationAdorner == null)
//    {
//        _obfuscationAdorner = new ObfuscationAdorner(this);
//        AdornerLayer.GetAdornerLayer(this).Add(_obfuscationAdorner);
//        UpdateObfuscation();
//    }
//}

//private void UpdateObfuscation()
//{
//    if (_obfuscationAdorner != null)
//    {
//        _obfuscationAdorner.Visibility = IsObfuscated ? Visibility.Visible : Visibility.Collapsed;
//    }
//}

//protected override void OnRender(DrawingContext drawingContext)
//{
//    if (!IsObfuscated)
//    {
//        base.OnRender(drawingContext);
//    }
//}

//private class ObfuscationAdorner : Adorner
//{
//    private readonly CustomTextBox _textBox;

//    public ObfuscationAdorner(CustomTextBox textBox) : base(textBox)
//    {
//        _textBox = textBox;
//        IsHitTestVisible = false;
//    }

//    protected override void OnRender(DrawingContext drawingContext)
//    {
//        base.OnRender(drawingContext);

//        if (_textBox.IsObfuscated)
//        {
//            var text = _textBox.Text;
//            var obfuscatedText = GetObfuscatedText(text);

//            var formattedText = new FormattedText(
//                obfuscatedText,
//                CultureInfo.CurrentCulture,
//                FlowDirection.LeftToRight,
//                new Typeface(_textBox.FontFamily, _textBox.FontStyle, _textBox.FontWeight, _textBox.FontStretch),
//                _textBox.FontSize,
//                _textBox.Foreground.CloneCurrentValue());

//            drawingContext.DrawText(formattedText, new Point(2, 0));
//        }
//    }


//    private string GetObfuscatedText(string text)
//    {
//        int length = text.Length;

//        if (length <= 3)
//        {
//            return new string('*', length);
//        }
//        else
//        {
//            return text.Substring(0, 1) + new string('*', length - 2) + text.Substring(length - 1, 1);
//        }
//    }
//}



//public class CustomTextBox : TextBox
//{
//    private ObfuscationAdorner _obfuscationAdorner;

//    public static readonly DependencyProperty IsObfuscatedProperty = DependencyProperty.Register(nameof(IsObfuscated), typeof(bool), typeof(CustomTextBox), new PropertyMetadata(false, OnIsObfuscatedChanged));

//    public bool IsObfuscated
//    {
//        get => (bool)GetValue(IsObfuscatedProperty);
//        set => SetValue(IsObfuscatedProperty, value);
//    }

//    public CustomTextBox()
//    {
//        Loaded += CustomTextBox_Loaded;
//        this.TextChanged += CustomTextBox_TextChanged;
//    }

//    private void CustomTextBox_Loaded(object sender, RoutedEventArgs e)
//    {
//        AttachObfuscationAdorner();
//    }

//    private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e)
//    {
//        if (_obfuscationAdorner != null && IsObfuscated)
//        {
//            _obfuscationAdorner.InvalidateVisual();
//        }
//    }

//    private static void OnIsObfuscatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        var customTextBox = d as CustomTextBox;
//        customTextBox?.UpdateObfuscation();
//    }

//    private void AttachObfuscationAdorner()
//    {
//        if (_obfuscationAdorner == null)
//        {
//            _obfuscationAdorner = new ObfuscationAdorner(this);
//            AdornerLayer.GetAdornerLayer(this).Add(_obfuscationAdorner);
//            UpdateObfuscation();
//        }
//    }

//    private void UpdateObfuscation()
//    {
//        if (_obfuscationAdorner != null)
//        {
//            _obfuscationAdorner.Visibility = IsObfuscated ? Visibility.Visible : Visibility.Collapsed;
//        }
//    }

//    private class ObfuscationAdorner : Adorner
//    {
//        private readonly CustomTextBox _textBox;

//        public ObfuscationAdorner(CustomTextBox textBox) : base(textBox)
//        {
//            _textBox = textBox;
//            IsHitTestVisible = false;
//        }

//        protected override void OnRender(DrawingContext drawingContext)
//        {
//            base.OnRender(drawingContext);

//            if (_textBox.IsObfuscated)
//            {
//                var text = _textBox.Text;
//                var obfuscatedText = GetObfuscatedText(text);

//                var formattedText = new FormattedText(
//                    obfuscatedText,
//                    CultureInfo.CurrentCulture,
//                    FlowDirection.LeftToRight,
//                    new Typeface(_textBox.FontFamily, _textBox.FontStyle, _textBox.FontWeight, _textBox.FontStretch),
//                    _textBox.FontSize,
//                    _textBox.Foreground);

//                drawingContext.DrawText(formattedText, new Point(2, 0));
//            }
//        }

//        private string GetObfuscatedText(string text)
//        {
//            int length = text.Length;

//            if (length <= 3)
//            {
//                return new string('*', length);
//            }
//            else
//            {
//                return text.Substring(0, 1) + new string('*', length - 2) + text.Substring(length - 1, 1);
//            }
//        }
//    }
//}
