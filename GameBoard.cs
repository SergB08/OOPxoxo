namespace TicTacToe;

/// <summary>
/// Зберігає стан нескінченного ігрового поля для хрестиків-нуликів.
/// Умова перемоги: 5 символів поспіль (правила Гомоку для нескінченного поля).
/// </summary>
public class GameBoard
{
    // розріджене сховище: зберігаємо лише зайняті клітинки
    private readonly Dictionary<(int row, int col), CellState> _cells = new();

    public int WinLength { get; set; } = 5;

    public CellState GetCell(int row, int col)
    {
        _cells.TryGetValue((row, col), out var state);
        return state;
    }

    public void SetCell(int row, int col, CellState state)
    {
        if (state == CellState.Empty)
            _cells.Remove((row, col));
        else
            _cells[(row, col)] = state;
    }

    public bool IsEmpty(int row, int col) => GetCell(row, col) == CellState.Empty;

    public void Clear() => _cells.Clear();

    /// <summary>Повертає всі зайняті клітинки разом із їх сусідами як можливі ходи.</summary>
    public IEnumerable<(int row, int col)> GetCandidateMoves()
    {
        var candidates = new HashSet<(int, int)>();
        if (_cells.Count == 0)
        {
            // перший хід пропонуємо у центр поля
            candidates.Add((0, 0));
            return candidates;
        }

        foreach (var (pos, _) in _cells)
        {
            for (int dr = -2; dr <= 2; dr++)
                for (int dc = -2; dc <= 2; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    var candidate = (pos.row + dr, pos.col + dc);
                    if (IsEmpty(candidate.Item1, candidate.Item2))
                        candidates.Add(candidate);
                }
        }
        return candidates;
    }

    /// <summary>Перевіряє чи хід гравця на клітинку (row,col) є виграшним.</summary>
    public bool CheckWin(int row, int col, CellState player)
    {
        int[][] directions = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, -1 } };
        foreach (var d in directions)
        {
            int count = 1;
            count += CountInDirection(row, col, d[0], d[1], player);
            count += CountInDirection(row, col, -d[0], -d[1], player);
            if (count >= WinLength) return true;
        }
        return false;
    }

    private int CountInDirection(int row, int col, int dr, int dc, CellState player)
    {
        int count = 0;
        int r = row + dr, c = col + dc;
        while (GetCell(r, c) == player)
        {
            count++;
            r += dr;
            c += dc;
            if (count >= WinLength) break;
        }
        return count;
    }

    /// <summary>Повертає список клітинок що утворили виграшну лінію, або null якщо виграшу немає.</summary>
    public List<(int, int)>? GetWinningCells(int row, int col, CellState player)
    {
        int[][] directions = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, -1 } };
        foreach (var d in directions)
        {
            var line = new List<(int, int)> { (row, col) };
            CollectInDirection(row, col, d[0], d[1], player, line);
            CollectInDirection(row, col, -d[0], -d[1], player, line);
            if (line.Count >= WinLength) return line;
        }
        return null;
    }

    private void CollectInDirection(int row, int col, int dr, int dc, CellState player, List<(int, int)> line)
    {
        int r = row + dr, c = col + dc;
        while (GetCell(r, c) == player && line.Count < WinLength + 4)
        {
            line.Add((r, c));
            r += dr;
            c += dc;
        }
    }

    public int OccupiedCount => _cells.Count;
}

public enum CellState { Empty, X, O }

/// <summary>
/// Рушій штучного інтелекту на основі евристичного оцінювання з переглядом на 1-2 ходи вперед.
/// </summary>
public static class AiEngine
{
    // генератор випадкових чисел для вибору серед рівноцінних ходів
    private static readonly Random _rng = new();

    public static (int row, int col) GetBestMove(GameBoard board, CellState aiPlayer)
    {
        CellState humanPlayer = aiPlayer == CellState.X ? CellState.O : CellState.X;

        var candidates = board.GetCandidateMoves().ToList();
        if (candidates.Count == 0) return (0, 0);

        int bestScore = int.MinValue;
        var bestMoves = new List<(int, int)>();

        foreach (var (row, col) in candidates)
        {
            // перевіряємо чи є негайний виграшний хід
            board.SetCell(row, col, aiPlayer);
            if (board.CheckWin(row, col, aiPlayer))
            {
                board.SetCell(row, col, CellState.Empty);
                return (row, col);
            }
            board.SetCell(row, col, CellState.Empty);

            // перевіряємо чи потрібно заблокувати негайний виграш суперника
            board.SetCell(row, col, humanPlayer);
            if (board.CheckWin(row, col, humanPlayer))
            {
                board.SetCell(row, col, CellState.Empty);
                return (row, col);
            }
            board.SetCell(row, col, CellState.Empty);
        }

        // оцінюємо кожен хід з переглядом на 2 напівходи
        foreach (var (row, col) in candidates)
        {
            board.SetCell(row, col, aiPlayer);
            int score = ScorePosition(board, row, col, aiPlayer, humanPlayer);

            // дивимося на один хід суперника вперед
            int opponentBest = int.MinValue;
            foreach (var (r2, c2) in board.GetCandidateMoves())
            {
                board.SetCell(r2, c2, humanPlayer);
                int opScore = ScorePosition(board, r2, c2, humanPlayer, aiPlayer);
                if (opScore > opponentBest) opponentBest = opScore;
                board.SetCell(r2, c2, CellState.Empty);
            }

            // підсумковий бал: власна атака мінус половина найкращої відповіді суперника
            int totalScore = score - opponentBest / 2;
            board.SetCell(row, col, CellState.Empty);

            if (totalScore > bestScore)
            {
                bestScore = totalScore;
                bestMoves.Clear();
                bestMoves.Add((row, col));
            }
            else if (totalScore == bestScore)
            {
                bestMoves.Add((row, col));
            }
        }

        // якщо кілька ходів з однаковим балом обираємо випадковий
        return bestMoves[_rng.Next(bestMoves.Count)];
    }

    /// <summary>Евристична оцінка стану поля з точки зору гравця player.</summary>
    private static int ScorePosition(GameBoard board, int lastRow, int lastCol, CellState player, CellState opponent)
    {
        int score = 0;

        // оцінюємо всі лінії що проходять через область навколо останнього ходу
        int[][] directions = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, -1 } };
        var evaluated = new HashSet<(int, int, int, int)>();

        var toCheck = new List<(int, int)> { (lastRow, lastCol) };
        for (int dr = -2; dr <= 2; dr++)
            for (int dc = -2; dc <= 2; dc++)
                toCheck.Add((lastRow + dr, lastCol + dc));

        foreach (var (row, col) in toCheck)
            foreach (var d in directions)
            {
                // канонічний початок лінії щоб не оцінювати одну лінію двічі
                int sr = row - d[0] * 4, sc = col - d[1] * 4;
                var key = (sr, sc, d[0], d[1]);
                if (!evaluated.Add(key)) continue;

                score += EvaluateLine(board, sr, sc, d[0], d[1], player, opponent);
            }

        return score;
    }

    private static int EvaluateLine(GameBoard board, int startRow, int startCol, int dr, int dc,
        CellState player, CellState opponent)
    {
        int score = 0;
        // перебираємо всі вікна розміром WinLength вздовж лінії
        for (int offset = 0; offset <= 4; offset++)
        {
            int playerCount = 0, emptyCount = 0;
            bool blocked = false;
            for (int i = 0; i < board.WinLength; i++)
            {
                int r = startRow + (offset + i) * dr;
                int c = startCol + (offset + i) * dc;
                var cell = board.GetCell(r, c);
                if (cell == player) playerCount++;
                else if (cell == CellState.Empty) emptyCount++;
                // якщо в лінії є фігура суперника вона заблокована
                else { blocked = true; break; }
            }

            // нараховуємо бали лише за незаблоковані лінії
            if (!blocked)
            {
                score += playerCount switch
                {
                    4 => 10000,  // майже виграшна лінія
                    3 => 500,    // три в ряд
                    2 => 50,     // два в ряд
                    1 => 5,      // одна фігура
                    _ => 0
                };
            }
        }
        return score;
    }
}