# XAML プロパティ

---

## XAML プロパティの設定方法

XAMLには主に2種類のプロパティ設定方法がある。  

---

## プロパティ属性構文(Attribute Syntax)

要素の属性にテキストを使用して設定する。  
値を文字列で指定できる（文字列そのもの or 文字列から直接変換可能な型）プロパティの場合はこの構文を使うと便利。  

``` XML
<TextBox
    Width = "100" FontSize = "30" Text = "text 1"
    Background = "White" Foreground = "Blue" />
```

---

## プロパティ要素構文(Property Element Syntax)

要素のinnerText・innerXMLを使用してプロパティを設定する。  
複雑な型を持つプロパティの場合に有効。  
XML 要素の子要素としてプロパティの値を設定する構文。  

``` XML
<TextBox>
  <TextBox.Width>100</TextBox.Width>
  <TextBox.FontSize>30</TextBox.FontSize>
  <TextBox.Background>White</TextBox.Background>
  <TextBox.Foreground>Blue</TextBox.Foreground>
  <TextBox.Text>text 1</TextBox.Text>
</TextBox>
```

---

[XAML の基本構造（WPF）](https://ufcpp.net/study/dotnet/wpf_xamlbasic.html)  
[XAMLの書き方（１）](https://techinfoofmicrosofttech.osscons.jp/index.php?XAML%E3%81%AE%E6%9B%B8%E3%81%8D%E6%96%B9%EF%BC%88%EF%BC%91%EF%BC%89#ra4d77a1)  
