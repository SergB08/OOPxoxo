namespace TicTacToe;

public partial class MainForm : Form
{
    // ── Layout constants ────────────────────────────────────────────────────
    private const int CellSize    = 44;
    private const int LineWidth   = 1;
    private const int PieceMargin = 6;

    // ── Game state ──────────────────────────────────────────────────────────
    private readonly GameBoard _board = new();
    private GameMode  _mode;
    private CellState _currentPlayer = CellState.X; // X always goes first visually
    private CellState _humanPlayer   = CellState.O; // in VsComputer: human is O
    private bool      _gameOver  = false;
    private List<(int, int)>? _winCells;

    private int _scoreX = 0;
    private int _scoreO = 0;

    // ── View / pan ──────────────────────────────────────────────────────────
    private Point _viewOffset = Point.Empty;  // board pixel offset
    private Point _panStart;
    private bool  _isPanning = false;
    private const int ScorePanelHeight = 56;

    // ── Controls ────────────────────────────────────────────────────────────
    private Panel  _boardPanel  = null!;
    private Label  _statusLabel = null!;
    private Label  _scoreXLabel = null!;
    private Label  _scoreOLabel = null!;
    private Button _newGameBtn  = null!;
    private Button _resetScoreBtn = null!;
    private Panel  _scorePanel  = null!;

    public MainForm(GameMode mode, int winLength)
    {
        _mode = mode;
        _board.WinLength = winLength;
        InitializeComponent();
        BuildUI();
        StartNewGame();
    }

    // ── UI Construction ─────────────────────────────────────────────────────

    private void BuildUI()
    {
        this.Text            = "Хрестики-нулики — Нескінченне поле";
        this.MinimumSize     = new Size(520, 520);
        this.Size            = new Size(640, 640);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = SystemColors.Control;
        this.Font            = new Font("Tahoma", 8f);

        // ── Top toolbar ──────────────────────────────────────────────────
        var toolbar = new Panel
        {
            Dock   = DockStyle.Top,
            Height = 32,
            BackColor = Color.FromArgb(212, 208, 200),
        };
        Xp3DPanel(toolbar);

        _newGameBtn = XpButton("Нова гра", 4, 4, 90);
        _newGameBtn.Click += (_, _) => StartNewGame();

        _resetScoreBtn = XpButton("Скинути рахунок", 98, 4, 120);
        _resetScoreBtn.Click += (_, _) => ResetScore();

        _statusLabel = new Label
        {
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Left      = 226, Top = 0,
            Width     = 320, Height = 32,
            BackColor = Color.Transparent,
            Font      = new Font("Tahoma", 8f, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 0, 128),
        };

        toolbar.Controls.AddRange(new Control[] { _newGameBtn, _resetScoreBtn, _statusLabel });
        this.Controls.Add(toolbar);

        // ── Score panel ───────────────────────────────────────────────────
        _scorePanel = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = ScorePanelHeight,
            BackColor = Color.FromArgb(58, 110, 165),
        };

        _scoreXLabel = ScoreLabel(ContentAlignment.MiddleLeft,  8);
        _scoreOLabel = ScoreLabel(ContentAlignment.MiddleRight, 0);
        _scoreOLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _scoreOLabel.Left   = _scorePanel.Width - 8 - 230;

        _scorePanel.Controls.Add(_scoreXLabel);
        _scorePanel.Controls.Add(_scoreOLabel);
        _scorePanel.SizeChanged += (_, _) =>
        {
            _scoreOLabel.Left = _scorePanel.Width - 8 - 230;
        };

        this.Controls.Add(_scorePanel);

        // ── Board panel ───────────────────────────────────────────────────
        _boardPanel = new Panel
        {
            Dock        = DockStyle.Fill,
            BackColor   = Color.White,
            BorderStyle = BorderStyle.Fixed3D,
        };

        _boardPanel.Paint       += BoardPanel_Paint;
        _boardPanel.MouseDown   += BoardPanel_MouseDown;
        _boardPanel.MouseMove   += BoardPanel_MouseMove;
        _boardPanel.MouseUp     += BoardPanel_MouseUp;
        _boardPanel.MouseWheel  += BoardPanel_MouseWheel;
        _boardPanel.Resize      += (_, _) => _boardPanel.Invalidate();

        this.Controls.Add(_boardPanel);

        // Make sure paint order: toolbar on top, score below, board fills rest
        toolbar.BringToFront();
        _scorePanel.BringToFront();
    }

    private static Button XpButton(string text, int x, int y, int w)
    {
        return new Button
        {
            Text      = text,
            Left      = x, Top = y,
            Width     = w, Height = 23,
            FlatStyle = FlatStyle.System,
            Font      = new Font("Tahoma", 8f),
        };
    }

    private static Label ScoreLabel(ContentAlignment align, int left)
    {
        return new Label
        {
            AutoSize  = false,
            Width     = 230, Height = ScorePanelHeight,
            Left      = left, Top = 0,
            TextAlign = align,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font      = new Font("Tahoma", 14f, FontStyle.Bold),
        };
    }

    private static void Xp3DPanel(Panel p)
    {
        p.Paint += (s, e) =>
        {
            var g = e.Graphics;
            var r = ((Panel)s!).ClientRectangle;
            using var lightPen = new Pen(Color.White);
            using var darkPen  = new Pen(Color.FromArgb(128, 128, 128));
            g.DrawLine(lightPen, r.Left, r.Top, r.Right, r.Top);
            g.DrawLine(lightPen, r.Left, r.Top, r.Left,  r.Bottom);
            g.DrawLine(darkPen,  r.Left, r.Bottom-1, r.Right, r.Bottom-1);
        };
    }

    // ── Game lifecycle ───────────────────────────────────────────────────────

    private void StartNewGame()
    {
        _board.Clear();
        _gameOver   = false;
        _winCells   = null;
        _viewOffset = Point.Empty;

        // X always moves first. In VsComputer, computer IS X and goes first.
        _currentPlayer = CellState.X;
        _humanPlayer   = (_mode == GameMode.VsComputer) ? CellState.O : CellState.X;

        UpdateLabels();
        _boardPanel.Invalidate();

        if (_mode == GameMode.VsComputer)
            MakeComputerMove();
    }

    private void ResetScore()
    {
        _scoreX = 0; _scoreO = 0;
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (_mode == GameMode.VsComputer)
        {
            _scoreXLabel.Text = $"💻 Комп: {_scoreX}";
            _scoreOLabel.Text = $"Гравець: {_scoreO} ✖";
        }
        else
        {
            _scoreXLabel.Text = $"✖ Гравець 1: {_scoreX}";
            _scoreOLabel.Text = $"Гравець 2: {_scoreO} ◯";
        }

        if (_gameOver)
        {
            // Status is set by the win/draw handler
        }
        else
        {
            string whose = "";
            if (_mode == GameMode.VsComputer)
                whose = _currentPlayer == _humanPlayer ? "Ваш хід (◯)" : "Комп'ютер думає…";
            else
                whose = _currentPlayer == CellState.X ? "Хід: Гравець 1 (✖)" : "Хід: Гравець 2 (◯)";
            _statusLabel.Text = whose;
        }
    }

    // ── Board rendering ──────────────────────────────────────────────────────

    private void BoardPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g   = e.Graphics;
        var w   = _boardPanel.ClientSize.Width;
        var h   = _boardPanel.ClientSize.Height;
        int ox  = _viewOffset.X + w / 2;   // pixel origin = board (0,0)
        int oy  = _viewOffset.Y + h / 2;

        // Visible cell range
        int colMin = (int)Math.Floor((-ox) / (double)CellSize) - 1;
        int colMax = (int)Math.Ceiling((w - ox) / (double)CellSize) + 1;
        int rowMin = (int)Math.Floor((-oy) / (double)CellSize) - 1;
        int rowMax = (int)Math.Ceiling((h - oy) / (double)CellSize) + 1;

        // Grid lines
        using var gridPen = new Pen(Color.FromArgb(210, 210, 220));
        for (int c = colMin; c <= colMax; c++)
        {
            int px = ox + c * CellSize;
            g.DrawLine(gridPen, px, 0, px, h);
        }
        for (int r = rowMin; r <= rowMax; r++)
        {
            int py = oy + r * CellSize;
            g.DrawLine(gridPen, 0, py, w, py);
        }

        // Origin marker
        using var axisPen = new Pen(Color.FromArgb(180, 180, 200), 1.5f);
        g.DrawLine(axisPen, ox, 0, ox, h);
        g.DrawLine(axisPen, 0, oy, w, oy);

        // Pieces
        for (int row = rowMin; row <= rowMax; row++)
        for (int col = colMin; col <= colMax; col++)
        {
            var state = _board.GetCell(row, col);
            if (state == CellState.Empty) continue;

            bool isWinCell = _winCells != null && _winCells.Contains((row, col));
            DrawPiece(g, state, ox + col * CellSize, oy + row * CellSize, isWinCell);
        }

        // Hover hint (show faint marker on empty cells near cursor) — done via cursor styling
    }

    private static void DrawPiece(Graphics g, CellState state, int px, int py, bool highlight)
    {
        int m  = PieceMargin;
        int sz = CellSize - m * 2;
        var r  = new Rectangle(px + m, py + m, sz, sz);

        if (highlight)
        {
            using var hBrush = new SolidBrush(Color.FromArgb(60, 255, 215, 0));
            g.FillRectangle(hBrush, px, py, CellSize, CellSize);
        }

        if (state == CellState.X)
        {
            int thickness = highlight ? 4 : 3;
            using var pen = new Pen(highlight ? Color.FromArgb(200, 40, 0) : Color.FromArgb(180, 0, 0), thickness);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
            g.DrawLine(pen, r.Right, r.Top, r.Left, r.Bottom);
        }
        else
        {
            int thickness = highlight ? 4 : 3;
            using var pen = new Pen(highlight ? Color.FromArgb(0, 80, 200) : Color.FromArgb(0, 60, 180), thickness);
            g.DrawEllipse(pen, r);
        }
    }

    // ── Input handling ───────────────────────────────────────────────────────

    private void BoardPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            _isPanning = true;
            _panStart  = e.Location;
            _boardPanel.Cursor = Cursors.Hand;
            return;
        }

        if (e.Button != MouseButtons.Left || _gameOver) return;
        if (_mode == GameMode.VsComputer && _currentPlayer != _humanPlayer) return;

        var (row, col) = PixelToCell(e.Location);
        if (!_board.IsEmpty(row, col)) return;

        PlacePiece(row, col);
    }

    private void BoardPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isPanning) return;
        _viewOffset = new Point(
            _viewOffset.X + e.X - _panStart.X,
            _viewOffset.Y + e.Y - _panStart.Y);
        _panStart = e.Location;
        _boardPanel.Invalidate();
    }

    private void BoardPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            _isPanning = false;
            _boardPanel.Cursor = Cursors.Default;
        }
    }

    private void BoardPanel_MouseWheel(object? sender, MouseEventArgs e)
    {
        // Scroll zooms/pans
        if (ModifierKeys.HasFlag(Keys.Control))
            _viewOffset = new Point(_viewOffset.X + e.Delta / 4, _viewOffset.Y);
        else
            _viewOffset = new Point(_viewOffset.X, _viewOffset.Y + e.Delta / 4);
        _boardPanel.Invalidate();
    }

    private (int row, int col) PixelToCell(Point p)
    {
        int w  = _boardPanel.ClientSize.Width;
        int h  = _boardPanel.ClientSize.Height;
        int ox = _viewOffset.X + w / 2;
        int oy = _viewOffset.Y + h / 2;
        int col = (int)Math.Floor((p.X - ox) / (double)CellSize);
        int row = (int)Math.Floor((p.Y - oy) / (double)CellSize);
        return (row, col);
    }

    // ── Move processing ──────────────────────────────────────────────────────

    private void PlacePiece(int row, int col)
    {
        _board.SetCell(row, col, _currentPlayer);
        _boardPanel.Invalidate();

        if (_board.CheckWin(row, col, _currentPlayer))
        {
            _winCells = _board.GetWinningCells(row, col, _currentPlayer);
            _gameOver = true;
            _boardPanel.Invalidate();

            bool isHumanWin = (_mode == GameMode.TwoPlayers || _currentPlayer == _humanPlayer);
            if (_mode == GameMode.VsComputer)
            {
                if (_currentPlayer == _humanPlayer) { _scoreO++; _statusLabel.Text = "🎉 Ви перемогли!"; }
                else { _scoreX++; _statusLabel.Text = "🤖 Комп'ютер переміг!"; }
            }
            else
            {
                if (_currentPlayer == CellState.X) { _scoreX++; _statusLabel.Text = "🎉 Переміг Гравець 1 (✖)!"; }
                else { _scoreO++; _statusLabel.Text = "🎉 Переміг Гравець 2 (◯)!"; }
            }
            UpdateScoreLabels();
            return;
        }

        // Switch player
        _currentPlayer = _currentPlayer == CellState.X ? CellState.O : CellState.X;
        UpdateLabels();

        if (_mode == GameMode.VsComputer && _currentPlayer != _humanPlayer && !_gameOver)
        {
            // Delay AI move slightly so UI updates
            var timer = new System.Windows.Forms.Timer { Interval = 120 };
            timer.Tick += (_, _) => { timer.Stop(); MakeComputerMove(); };
            timer.Start();
        }
    }

    private void MakeComputerMove()
    {
        if (_gameOver) return;
        var (row, col) = AiEngine.GetBestMove(_board, _currentPlayer);
        PlacePiece(row, col);
    }

    private void UpdateScoreLabels()
    {
        if (_mode == GameMode.VsComputer)
        {
            _scoreXLabel.Text = $"💻 Комп: {_scoreX}";
            _scoreOLabel.Text = $"Гравець: {_scoreO} ✖";
        }
        else
        {
            _scoreXLabel.Text = $"✖ Гравець 1: {_scoreX}";
            _scoreOLabel.Text = $"Гравець 2: {_scoreO} ◯";
        }
    }

    // ── Designer stub ────────────────────────────────────────────────────────
    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.AutoScaleDimensions = new SizeF(6F, 13F);
        this.AutoScaleMode       = AutoScaleMode.Font;
        this.ResumeLayout(false);
    }
}

public enum GameMode { VsComputer, TwoPlayers }
