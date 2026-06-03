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
        this.Text = "Хрестики-нулики";
        this.Size = new Size(320, 380);
        this.MinimumSize = this.Size;
        this.MaximumSize = this.Size;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(212, 208, 200);
        this.Font = new Font("Tahoma", 8f);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Title banner
        var banner = new Panel
        {
            Left = 0,
            Top = 0,
            Width = 320,
            Height = 90,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = Color.FromArgb(58, 110, 165),
        };
        banner.Controls.Add(new Label
        {
            Text = "Хрестики-нулики",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Tahoma", 16f, FontStyle.Bold),
            BackColor = Color.Transparent,
        });
        banner.Controls.Add(new Label
        {
            Text = "Нескінченне поле",
            Dock = DockStyle.Bottom,
            Height = 22,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(180, 210, 240),
            Font = new Font("Tahoma", 9f, FontStyle.Italic),
            BackColor = Color.Transparent,
        });
        this.Controls.Add(banner);

        // Buttons — positioned below the banner
        int btnX = 60, btnW = 200, btnH = 36, gap = 48;
        int startY = 110;

        Button Make(string text, int top) => new Button
        {
            Text = text,
            Left = btnX,
            Top = top,
            Width = btnW,
            Height = btnH,
            FlatStyle = FlatStyle.System,
            Font = new Font("Tahoma", 10f),
        };

        var btnPlay = Make("▶  Грати", startY);
        var btnSettings = Make("⚙  Налаштування", startY + gap);
        var btnHelp = Make("?  Довідка", startY + gap * 2);
        var btnExit = Make("✕  Вихід", startY + gap * 3);

        btnPlay.Click += (_, _) => OpenGame();
        btnSettings.Click += (_, _) => new SettingsForm().ShowDialog(this);
        btnHelp.Click += (_, _) => new HelpForm().ShowDialog(this);
        btnExit.Click += (_, _) => Application.Exit();

        this.Controls.AddRange(new Control[] { btnPlay, btnSettings, btnHelp, btnExit });
    }

    private void OpenGame()
    {
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