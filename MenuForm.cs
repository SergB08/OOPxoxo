namespace TicTacToe;

public class MenuForm : Form
{
    public static int WinLength { get; private set; } = 5;

    public MenuForm()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        this.Text            = "Хрестики-нулики";
        this.Size            = new Size(320, 340);
        this.MinimumSize     = this.Size;
        this.MaximumSize     = this.Size;
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = Color.FromArgb(212, 208, 200);
        this.Font            = new Font("Tahoma", 8f);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox     = false;

        // Title banner
        var banner = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 90,
            BackColor = Color.FromArgb(58, 110, 165),
        };
        var titleLabel = new Label
        {
            Text      = "Хрестики-нулики",
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font      = new Font("Tahoma", 16f, FontStyle.Bold),
            BackColor = Color.Transparent,
        };
        var subtitleLabel = new Label
        {
            Text      = "Нескінченне поле",
            Dock      = DockStyle.Bottom,
            Height    = 22,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(180, 210, 240),
            Font      = new Font("Tahoma", 9f, FontStyle.Italic),
            BackColor = Color.Transparent,
        };
        banner.Controls.Add(titleLabel);
        banner.Controls.Add(subtitleLabel);
        this.Controls.Add(banner);

        // Button container
        var btnPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(60, 20, 60, 20),
        };

        var btnPlay     = MenuButton("▶  Грати",       0);
        var btnHelp     = MenuButton("?  Довідка",     44);
        var btnSettings = MenuButton("⚙  Налаштування",88);

        btnPlay.Click     += (_, _) => OpenGame();
        btnHelp.Click     += (_, _) => new HelpForm().ShowDialog(this);
        btnSettings.Click += (_, _) => { new SettingsForm().ShowDialog(this); };

        btnPanel.Controls.AddRange(new Control[] { btnPlay, btnHelp, btnSettings });
        this.Controls.Add(btnPanel);
        banner.BringToFront();
    }

    private static Button MenuButton(string text, int topOffset)
    {
        return new Button
        {
            Text      = text,
            Left      = 0, Top  = topOffset,
            Width     = 200, Height = 36,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            FlatStyle = FlatStyle.System,
            Font      = new Font("Tahoma", 10f),
        };
    }

    private void OpenGame()
    {
        // Ask for mode via a small dialog
        using var modeForm = new ModeSelectForm();
        if (modeForm.ShowDialog(this) != DialogResult.OK) return;

        var gameForm = new MainForm(modeForm.SelectedMode, WinLength);
        gameForm.ShowDialog(this);
    }

    internal static void ApplySettings(int winLength)
    {
        WinLength = winLength;
    }
}
