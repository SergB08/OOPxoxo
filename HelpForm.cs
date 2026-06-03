namespace TicTacToe;

public class HelpForm : Form
{
    public HelpForm()
    {
        // назва вікна довідки
        this.Text = "Довідка";
        this.Size = new Size(380, 230);
        this.MinimumSize = this.Size;
        this.MaximumSize = this.Size;
        // відкриваємо по центру відносно батьківського вікна
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(212, 208, 200);
        this.Font = new Font("Tahoma", 8f);
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
            Text = "Керування",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Tahoma", 11f, FontStyle.Bold),
            BackColor = Color.Transparent,
        });
        this.Controls.Add(header);

        // масив рядків таблиці керування: назва клавіші та її дія
        var rows = new[]
        {
            ("ЛКМ (ліва кнопка миші)",   "Поставити символ на поле"),
            ("Колесо миші",              "Прокрутка вгору / вниз"),
            ("Ctrl + Колесо миші",       "Прокрутка вліво / вправо"),
        };

        // динамічно будуємо рядки таблиці через цикл
        int y = 46;
        foreach (var (key, desc) in rows)
        {
            // лівий стовпець з назвою клавіші
            this.Controls.Add(new Label
            {
                Text = key,
                Left = 12,
                Top = y,
                Width = 170,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Tahoma", 8f, FontStyle.Bold),
                BackColor = Color.Transparent,
            });
            // правий стовпець з описом дії
            this.Controls.Add(new Label
            {
                Text = desc,
                Left = 186,
                Top = y,
                Width = 178,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
            });
            // тонка горизонтальна лінія розділювач між рядками
            this.Controls.Add(new Panel
            {
                Left = 12,
                Top = y + 28,
                Width = 350,
                Height = 1,
                BackColor = Color.FromArgb(160, 160, 160),
            });
            y += 30;
        }

        // кнопка закриття довідки
        var btnClose = new Button
        {
            Text = "Закрити",
            Left = 140,
            Top = y + 8,
            Width = 90,
            Height = 26,
            FlatStyle = FlatStyle.System,
            DialogResult = DialogResult.OK,
        };
        this.Controls.Add(btnClose);
        this.AcceptButton = btnClose;
        // заголовок поверх решти елементів
        header.BringToFront();
    }
}