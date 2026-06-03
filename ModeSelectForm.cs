namespace TicTacToe;

public class ModeSelectForm : Form
{
    public GameMode SelectedMode { get; private set; } = GameMode.VsComputer;

    public ModeSelectForm()
    {
        this.Text            = "Вибір режиму";
        this.Size            = new Size(280, 170);
        this.MinimumSize     = this.Size;
        this.MaximumSize     = this.Size;
        this.StartPosition   = FormStartPosition.CenterParent;
        this.BackColor       = Color.FromArgb(212, 208, 200);
        this.Font            = new Font("Tahoma", 8f);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox     = false;

        var lbl = new Label
        {
            Text     = "Оберіть режим гри:",
            Left = 12, Top = 12, Width = 240, Height = 20,
            Font = new Font("Tahoma", 8f, FontStyle.Bold),
        };

        var rbVsPC = new RadioButton
        {
            Text    = "Гравець проти комп'ютера",
            Left = 16, Top = 36, Width = 230, Height = 22,
            Checked = true,
        };
        var rbVsP2 = new RadioButton
        {
            Text    = "Два гравці",
            Left = 16, Top = 60, Width = 230, Height = 22,
        };

        var btnOk = new Button
        {
            Text         = "OK",
            Left = 80, Top = 92, Width = 75, Height = 26,
            FlatStyle    = FlatStyle.System,
            DialogResult = DialogResult.OK,
        };
        var btnCancel = new Button
        {
            Text         = "Скасувати",
            Left = 162, Top = 92, Width = 90, Height = 26,
            FlatStyle    = FlatStyle.System,
            DialogResult = DialogResult.Cancel,
        };

        btnOk.Click += (_, _) =>
        {
            SelectedMode = rbVsPC.Checked ? GameMode.VsComputer : GameMode.TwoPlayers;
        };

        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
        this.Controls.AddRange(new Control[] { lbl, rbVsPC, rbVsP2, btnOk, btnCancel });
    }
}
