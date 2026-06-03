namespace TicTacToe;

public class HelpForm : Form
{
    public HelpForm()
    {
        this.Text            = "Довідка";
        this.Size            = new Size(380, 280);
        this.MinimumSize     = this.Size;
        this.MaximumSize     = this.Size;
        this.StartPosition   = FormStartPosition.CenterParent;
        this.BackColor       = Color.FromArgb(212, 208, 200);
        this.Font            = new Font("Tahoma", 8f);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox     = false;

        // Header
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 36,
            BackColor = Color.FromArgb(58, 110, 165),
        };
        header.Controls.Add(new Label
        {
            Text      = "Керування",
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font      = new Font("Tahoma", 11f, FontStyle.Bold),
            BackColor = Color.Transparent,
        });
        this.Controls.Add(header);

        // Controls table
        var rows = new[]
        {
            ("ЛКМ (ліва кнопка миші)",        "Поставити свій символ на поле"),
            ("ПКМ + перетягування",            "Панорамування (переміщення поля)"),
            ("Колесо миші",                    "Прокрутка вгору / вниз"),
            ("Ctrl + Колесо миші",             "Прокрутка вліво / вправо"),
            ("Кнопка «Нова гра»",              "Почати нову партію"),
            ("Кнопка «Скинути рахунок»",       "Обнулити рахунок обох гравців"),
        };

        int y = 46;
        foreach (var (key, desc) in rows)
        {
            var keyLbl = new Label
            {
                Text      = key,
                Left = 12, Top = y, Width = 170, Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Tahoma", 8f, FontStyle.Bold),
                BackColor = Color.Transparent,
            };
            var descLbl = new Label
            {
                Text      = desc,
                Left = 186, Top = y, Width = 178, Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
            };
            this.Controls.Add(keyLbl);
            this.Controls.Add(descLbl);

            // separator line
            var sep = new Panel
            {
                Left = 12, Top = y + 28, Width = 350, Height = 1,
                BackColor = Color.FromArgb(160, 160, 160),
            };
            this.Controls.Add(sep);
            y += 30;
        }

        var btnClose = new Button
        {
            Text         = "Закрити",
            Left = 140, Top = y + 6, Width = 90, Height = 26,
            FlatStyle    = FlatStyle.System,
            DialogResult = DialogResult.OK,
        };
        this.Controls.Add(btnClose);
        this.AcceptButton = btnClose;
        header.BringToFront();
    }
}
