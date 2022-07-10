# ControlTemplate

[WPF4.5入門 その52 「コントロールテンプレート」](https://blog.okazuki.jp/entry/2014/09/07/195335)  

WPFのコントロールは、見た目を完全にカスタマイズする方法が提供されています。  
コントロールは、TemplateというプロパティにControlTemplateを設定することで、見た目を100%カスタマイズすることが出来るようになっています。  

→  
ItemsControlの時にTemplateプロパティの直下に配置したあれ。  
Templateプロパティ自体、ControlTemplateクラスしか許可していなかったので、そういうものなんだなとしか理解していなかった。  

``` XML : コントロールのTemplateの差し替え例
<!-- WPFのLabelコントロールには、Windows Formと異なりClickイベントが提供されていません。 -->
<!-- ここではClick可能なLabelの実現のために、Buttonコントロールの見た目をLabelにします。 -->

<Button Content="ラベル" Click="Button_Click">
    <Button.Template>
        <ControlTemplate TargetType="{x:Type Button}">
            <Label Content="{TemplateBinding Content}" />
        </ControlTemplate>
    </Button.Template>
</Button>
```

ControlTemplateは、TargetTypeにテンプレートを適用するコントロールの型を指定します。  
そして、ControlTemplateの中に、コントロールの見た目を定義します。  

このとき、TemplateBindingという特殊なBindingを使うことで、コントロールのプロパティをバインドすることが出来ます。  
上記の例ではButtonのContentに設定された値をLabelのContentにBindingしています。  

## 必要な構成要素があるコントロールを上書きする場合

コントロールには、そのコントロールが動作するために必要な構成要素がある場合があります。  
スクロールバーのバーやバーを左右に移動するためのボタンなど、見た目だけでなく、操作することが出来る要素がそれにあたります。  
このようなコントロールは、ControlTemplate内に、コントロールごとに決められた名前で定義する必要があります。  
どのように定義しているかは、MSDNにある、デフォルトのコントロールテンプレートの例を参照してください。  

[コントロールのスタイルとテンプレート](http://msdn.microsoft.com/ja-jp/library/aa970773(v=vs.110).aspx)  

→  
チェックボックスの例を見たときに、やたら複雑に書かれていたのはこういう仕組みがあったからか。  
スタイルを適応するときも、元のコントロールと同じように再定義しないといけないのもこの仕組みのためだろうか。  
