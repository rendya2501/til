# なんちゃってデザインパターン

[なんちゃってデザインパターンで条件分岐をなくす](https://blog.jnito.com/entry/20100717/1279321664)

すんごい大昔にこのコードを見て衝撃を受けたのを覚えている。  
当時はこんな設計、一生思いつけないだろうなって思ってた。  
今でも思いつけないけど、思ったらまとめてなかったのでまとめることにした。  

---

## 概要

ボタンを押したら、ボタンに応じてメッセージと色を変えるサンプル。  
StateパターンとFactoryパターンらしきテクニックを使って実装。  
ボタンを押すとStateに応じて文字や色が変わる。  
条件分岐にあたる部分をStateFactoryクラス、Stateクラス、それとStateクラスのサブクラスで実現。  

![aa](https://cdn-ak.f.st-hatena.com/images/fotolife/J/JunichiIto/20100717/20100717145157.png)  

---

## 開発環境

- win 11  
- dotnet 7.0.100  

---

## デザインパターンを使わない場合のコード

``` cs
public partial class Form1 : Form
{
    public Form1() => InitializeComponent();

    private void buttonNormal_Click(object sender, EventArgs e) => this.ChangeTextBox("Normal");
    private void buttonWarning_Click(object sender, EventArgs e) => this.ChangeTextBox("Warning");
    private void buttonError_Click(object sender, EventArgs e) => this.ChangeTextBox("Error");

    private void ChangeTextBox(string state)
    {
        this.textBox1.Text = state;
        switch (state)
        {
            case "Normal":
                this.textBox1.BackColor = SystemColors.Window;
                this.textBox1.ForeColor = SystemColors.WindowText;
                break;
            case "Warning":
                this.textBox1.BackColor = Color.Yellow;
                this.textBox1.ForeColor = SystemColors.WindowText;
                break;
            case "Error":
                this.textBox1.BackColor = Color.Red;
                this.textBox1.ForeColor = Color.White;
                break;
            default:
                break;
        }
    }
}
```

---

## コード

``` cs
public partial class Form1 : Form
{
    public Form1() => InitializeComponent();

    //クラスではなくプロパティを選択。分岐もなし
    private void ButtonNormal_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Normal);
    private void ButtonWarning_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Warning);
    private void ButtonError_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Error);

    //stateの中身はNormalStateクラス、WarningStateクラス、ErrorStateクラスのいずれか
    //どのクラスが渡されるかは実行時に決まる(=ポリモーフィズム)
    private void ChangeTextBox(State state) => state.ChangeTextBox(this.textBox1);
}
```

Stateクラス  
各Stateの親クラスになるのと同時に、テキストボックスに文字や色を設定します。  
ただし、Stateに応じて変化する文字や色はサブクラスに返してもらうため、抽象メソッド(抽象プロパティ)になっています。  

``` cs
internal abstract class State
{
    internal void ChangeTextBox(TextBox textBox)
    {
        textBox.Text = this.Text;
        textBox.BackColor = this.BackColor;
        textBox.ForeColor = this.ForeColor;
    }

    protected abstract String Text { get; }
    protected abstract Color BackColor { get; }
    protected abstract Color ForeColor { get; }
}
```

Stateクラスのサブクラス  
抽象メソッドを実装し、Stateごとに変わる文字や色だけを返します。  

``` cs
internal class NormalState : State
{
    protected override string Text => "Normal";
    protected override Color BackColor => SystemColors.Window;
    protected override Color ForeColor => SystemColors.WindowText;
}

internal class WarningState : State
{
    protected override string Text => "Warning";
    protected override Color BackColor => Color.Yellow;
    protected override Color ForeColor => SystemColors.WindowText;
}

internal class ErrorState : State
{
    protected override string Text => "Error";
    protected override Color BackColor => Color.Red;
    protected override Color ForeColor => Color.White;
}
```

StateFactoryクラス  
どのサブクラスを利用すべきかはStateFactoryクラスにカプセル化しておく。  

``` cs
internal static class StateFactory
{
    private static readonly NormalState normal = new NormalState();
    private static readonly WarningState warning = new WarningState();
    private static readonly ErrorState error = new ErrorState();

    //戻り値の型を親クラス(Stateクラス)にしてあるのがポイント
    //戻り値の型を抽象化しておくことで、呼び出し側はどのサブクラスも受け入れ可能になる
    internal static State Normal => normal;
    internal static State Warning => warning;
    internal static State Error => error;
}
```
