namespace TicTacToe;

public class SettingsForm : Form
{
    public SettingsForm()
    {
        // назва вікна налаштувань
        this.Text = "Налаштування";
        // фіксований розмір вікна
        this.Size = new Size(300, 200);
        this.MinimumSize = this.Size;
        this.MaximumSize = this.Size;
        // відкриваємо по центру відносно батьківського вікна
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(212, 208, 200);
        this.Font = new Font("Tahoma", 8f);
        // забороняємо змінювати розмір
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // синій заголовок зверху форми
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.FromArgb(58, 110, 165),
        };
        header.Controls.Add(new Label
        {
            Text = "Налаштування",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Tahoma", 11f, FontStyle.Bold),
            BackColor = Color.Transparent,
        });
        this.Controls.Add(header);

        // група з вибором умови перемоги
        var groupBox = new GroupBox
        {
            Text = "Умова перемоги",
            Left = 16,
            Top = 46,
            Width = 256,
            Height = 80,
            Font = new Font("Tahoma", 8f, FontStyle.Bold),
        };

        // перемикач для режиму 3 в ряд
        var rb3 = new RadioButton
        {
            Text = "3 в ряд",
            Left = 12,
            Top = 20,
            Width = 110,
            Height = 22,
            Font = new Font("Tahoma", 8f),
            // відмічаємо якщо зараз обрано саме цей режим
            Checked = MenuForm.WinLength == 3,
        };
        // перемикач для режиму 5 в ряд
        var rb5 = new RadioButton
        {
            Text = "5 в ряд",
            Left = 130,
            Top = 20,
            Width = 110,
            Height = 22,
            Font = new Font("Tahoma", 8f),
            Checked = MenuForm.WinLength == 5,
        };

        groupBox.Controls.Add(rb3);
        groupBox.Controls.Add(rb5);
        this.Controls.Add(groupBox);

        // кнопка підтвердження налаштувань
        var btnOk = new Button
        {
            Text = "OK",
            Left = 60,
            Top = 138,
            Width = 75,
            Height = 26,
            FlatStyle = FlatStyle.System,
            DialogResult = DialogResult.OK,
        };
        // кнопка скасування без збереження змін
        var btnCancel = new Button
        {
            Text = "Скасувати",
            Left = 144,
            Top = 138,
            Width = 90,
            Height = 26,
            FlatStyle = FlatStyle.System,
            DialogResult = DialogResult.Cancel,
        };

        // при натисканні OK зберігаємо обраний розмір виграшної лінії
        btnOk.Click += (_, _) =>
        {
            MenuForm.ApplySettings(rb3.Checked ? 3 : 5);
        };

        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
        this.Controls.AddRange(new Control[] { btnOk, btnCancel });
        // заголовок поверх решти елементів
        header.BringToFront();
    }
}