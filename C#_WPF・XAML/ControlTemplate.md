# ControlTemplate

---

## 概要

>WPFのコントロールは、見た目を完全にカスタマイズする方法が提供されています。  
コントロールは、TemplateというプロパティにControlTemplateを設定することで、見た目を100%カスタマイズすることが出来るようになっています。  
>[WPF4.5入門 その52 「コントロールテンプレート」](https://blog.okazuki.jp/entry/2014/09/07/195335)  

→  
ItemsControlの時にTemplateプロパティの直下に配置したあれ。  
Templateプロパティ自体、ControlTemplateクラスしか許可していなかったので、そういうものなんだなとしか理解していなかった。  

---

## クリック可能なラベルを作成する例

WPFのLabelコントロールは、Windows Formと異なりClickイベントが提供されていない。  
Click可能なLabelの実現のために、Buttonコントロールの見た目をLabelにする。  

``` XML
<Button Content="ラベル" Click="Button_Click">
    <Button.Template>
        <ControlTemplate TargetType="{x:Type Button}">
            <Label Content="{TemplateBinding Content}" />
        </ControlTemplate>
    </Button.Template>
</Button>
```

ControlTemplateは、TargetTypeにテンプレートを適用するコントロールの型を指定する。  
ControlTemplateの中に、コントロールの見た目を定義する。  
TemplateBindingという特殊なBindingを使うことで、コントロールのプロパティをバインドすることが出来る。  
上記の例ではButtonのContentに設定された値をLabelのContentにBindingしています。  

---

## 必要な構成要素があるコントロールを上書きする場合

>コントロールには、そのコントロールが動作するために必要な構成要素がある場合があります。  
スクロールバーのバーやバーを左右に移動するためのボタンなど、見た目だけでなく、操作することが出来る要素がそれにあたります。  
このようなコントロールは、ControlTemplate内に、コントロールごとに決められた名前で定義する必要があります。  
どのように定義しているかは、MSDNにある、デフォルトのコントロールテンプレートの例を参照してください。  
[コントロールのスタイルとテンプレート](http://msdn.microsoft.com/ja-jp/library/aa970773(v=vs.110).aspx)  

→  
チェックボックスの例を見たときに、やたら複雑に書かれていたのはこういう仕組みがあったからか。  
スタイルを適応するときも、元のコントロールと同じように再定義しないといけないのもこの仕組みのためだろうか。  

---

## 一部のコントロールだけ修正可能か？

基本的に無理っぽい。  
もっと探せばあるかもしれないが、そこまでする余裕がない。  

基本的に一部の修正は無理で、やるならテンプレートの全てのXAMLコードを元からコピーした上で、希望する部分を周成する必要がある模様。  
マイクロソフト公式やオープンソース系は問題なくできるが、ライセンス形式のサードパーティー制はそれはできない。  

遭遇した事象としては日付を和暦と西暦で表示を切り替えたいが、サードパーティ製のカレンダーコントロールは西暦にしか対応しておらず、それを表示しているテンプレートも秘匿されている。  
なので、年度部分だけをいじることが出来ない。  

「wpf controltemplate 一部変更」  
[WPFのデフォルトテンプレートの一部を置き換えます](https://www.web-dev-qa-db-ja.com/ja/c%23/wpf%E3%81%AE%E3%83%87%E3%83%95%E3%82%A9%E3%83%AB%E3%83%88%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88%E3%81%AE%E4%B8%80%E9%83%A8%E3%82%92%E7%BD%AE%E3%81%8D%E6%8F%9B%E3%81%88%E3%81%BE%E3%81%99/968443947/)  
[How to Embed Arbitrary Content in a WPF Control](https://www.codeproject.com/Articles/82464/How-to-Embed-Arbitrary-Content-in-a-WPF-Control)  
[【WPF】Templateを標準の実装からちょっとだけ変更したい](https://threeshark3.com/xaml-designer-template-copy/)  
[[WPF] xaml画面の構成要素の見た目のいじりかた(Style/Templateの使いどき)あれこれ](https://qiita.com/tera1707/items/4a2acd893f6098737987)  
