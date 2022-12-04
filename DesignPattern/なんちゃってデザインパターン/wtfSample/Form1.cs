namespace wtfSample;

public partial class Form1 : Form
{
    public Form1() => InitializeComponent();
    private void ButtonNormal_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Normal);
    private void ButtonWarning_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Warning);
    private void ButtonError_Click(object sender, EventArgs e) => this.ChangeTextBox(StateFactory.Error);
    private void ChangeTextBox(State state) => state.ChangeTextBox(this.textBox1);
}

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

internal static class StateFactory
{
    private static readonly NormalState normal = new NormalState();
    private static readonly WarningState warning = new WarningState();
    private static readonly ErrorState error = new ErrorState();

    //NormalStateクラスを返す
    internal static State Normal => normal;
    //WarningStateクラスを返す
    internal static State Warning => warning;
    //ErrorStateクラスを返す
    internal static State Error => error;
}