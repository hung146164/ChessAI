using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TeamColor myTeamColor = TeamColor.White;
    public TeamColor botTeam = TeamColor.Black;
    public TeamColor gameTurn = TeamColor.White;
    public GameMode gameMove = GameMode.Offline;
    public LayerMask tileMask;

    public GameObject winScreen;
    public GameObject loseScreen;

    public Transform whiteCamera;
    public Transform blackCamera;

    private Tile currentTile;
    private TileManager tileManager;
    private MoveManager moveManager;

    public ChessBoard chessBoard;
    private Tile[,] tileBoard;

    private List<(SpecialMove, Tile)> validMoves = new List<(SpecialMove, Tile)>();

    private bool isBotThinking = false;
    private System.Threading.CancellationTokenSource cts;
    private bool isGameOver = false;
    public bool isOnline = false;

    public float startTime = 0;
    private void Awake()
    {
        tileManager = new TileManager();
        tileBoard = chessBoard.tileBoard;
        moveManager = new MoveManager(tileBoard);
        currentTile = null;

        isGameOver = false; // Bắt đầu game
    }

    private void Update()
    {
        if (isGameOver) return;
        if(startTime<1f)
        {
            startTime += Time.deltaTime;
            Debug.Log(startTime);
            return;
        }
        if(isOnline)
        {
            if(myTeamColor==gameTurn)
            {
                HandleTileGame(GetTileFromRaycast());
            }
        }
        else
        {
            if (myTeamColor == gameTurn)
            {
                HandleTileGame(GetTileFromRaycast());
            }
            else if (gameTurn == botTeam && !isBotThinking)
            {
                isBotThinking = true;
                cts?.Cancel();
                cts = new System.Threading.CancellationTokenSource();
                StartBotThinkingAsync(cts.Token);
            }
        }
        
    }

    public void SetTeam(TeamColor team)
    {
        Transform targetCamera = (team == TeamColor.White) ? whiteCamera : blackCamera;
        Camera.main.transform.position = targetCamera.position;
        Camera.main.transform.rotation = targetCamera.rotation;

        myTeamColor= team;
        botTeam = team == TeamColor.Black ? TeamColor.White : TeamColor.Black;
    }

    private Tile GetTileFromRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, tileMask))
        {
            if (hit.collider.TryGetComponent(out Tile tile))
            {
                return tile;
            }
        }
        return null;
    }

    private void HandleTileGame(Tile tile)
    {
        HandleTileHover(tile);
        HandleTileSelect(tile);
        HandleTilePlaced(tile);
    }

    private void HandleTileHover(Tile tile)
    {
        if (tile == null) return;
        if (!Input.GetMouseButton(0))
        {
            tileManager.TileHover(tile);
        }
    }

    private void HandleTileSelect(Tile tile)
    {
        if (tile == null || tile.chessPiece == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            tileManager.TileSelect(tile);
            currentTile = tile;
            SetUpValidMoves(tile);
        }
    }

    private void HandleTilePlaced(Tile tile)
    {
        if (Input.GetMouseButtonUp(0))
        {
            validMoves.ForEach(move =>
            {
                if (move.Item2 == tile)
                {
                    if(isOnline==true)
                    {
                        photonView.RPC("MovePiece", RpcTarget.All, move.Item1, tile.currentX, tile.currentY, currentTile.currentX, currentTile.currentY);
                    }
                    else
                    {
                        MovePiece(move.Item1, tile.currentX, tile.currentY,currentTile.currentX,currentTile.currentY);
                    }
                }
            });
            ResetValidMoves();
        }
    }
    private void ChangeTurn()
    {
        gameTurn = gameTurn==TeamColor.White?TeamColor.Black:TeamColor.White;
    }
    [PunRPC]
    private void MovePiece(SpecialMove specialMove, int x, int y,int movingx,int movingy)
    {
        Tile tile = tileBoard[x, y];
        Tile movingTile = tileBoard[movingx, movingy];

        if (specialMove == SpecialMove.Promote)
            moveManager.moveHistory.Push(new Move(movingTile, tile, null, tile.chessPiece, specialMove));
        else
            moveManager.moveHistory.Push(new Move(movingTile, tile, movingTile.chessPiece, tile.chessPiece, specialMove));

        movingTile.chessPiece.MoveInBoard(tile, specialMove);
        tileBoard[movingx, movingy].chessPiece = null;
        ChangeTurn();
    }

    private void SetUpValidMoves(Tile tile)
    {
        Move lastMove = moveManager.moveHistory.Count > 0 ? moveManager.moveHistory.Peek() : null;
        List<(SpecialMove, Tile)> availableMoves = tile.chessPiece.GetAvailableMoves(tileBoard, lastMove);
        RemoveUnvalidMove(availableMoves, tile);
        HighlightValidMoves();
    }

    private void RemoveUnvalidMove(List<(SpecialMove, Tile)> moves, Tile begin)
    {
        ChessPiece myKing = myTeamColor == TeamColor.White ? chessBoard.whiteKing : chessBoard.blackKing;
        List<(SpecialMove, Tile)> validMovesTemp = new List<(SpecialMove, Tile)>();

        foreach (var move in moves)
        {
            ChessPiece capturedPiece = SimulateMove(begin, move.Item2);
            if (!IsKingInCheck(myKing))
                validMovesTemp.Add(move);
            UndoSimulateMove(begin, move.Item2, capturedPiece);
        }
        validMoves = validMovesTemp;
    }

    private ChessPiece SimulateMove(Tile fromTile, Tile toTile)
    {
        ChessPiece movedPiece = fromTile.chessPiece;
        ChessPiece capturedPiece = toTile.chessPiece;
        movedPiece.currentX = toTile.currentX;
        movedPiece.currentY = toTile.currentY;
        toTile.chessPiece = movedPiece;
        fromTile.chessPiece = null;
        return capturedPiece;
    }

    private void UndoSimulateMove(Tile fromTile, Tile toTile, ChessPiece capturedPiece)
    {
        ChessPiece movedPiece = toTile.chessPiece;
        movedPiece.currentX = fromTile.currentX;
        movedPiece.currentY = fromTile.currentY;
        fromTile.chessPiece = movedPiece;
        toTile.chessPiece = capturedPiece;
    }

    private bool IsKingInCheck(ChessPiece king)
    {
        foreach (var tile in tileBoard)
        {
            if (tile.chessPiece != null && tile.chessPiece.teamColor != king.teamColor)
            {
                Move lastMove = moveManager.moveHistory.Count > 0 ? moveManager.moveHistory.Peek() : null;
                List<(SpecialMove, Tile)> enemyMoves = tile.chessPiece.GetAvailableMoves(tileBoard, lastMove);
                foreach (var move in enemyMoves)
                {
                    if (move.Item2.currentX == king.currentX && move.Item2.currentY == king.currentY)
                        return true;
                }
            }
        }
        return false;
    }

    private void HighlightValidMoves()
    {
        foreach (var move in validMoves)
            tileManager.ValidTile(move.Item2);
    }

    private void ResetValidMoves()
    {
        foreach (var move in validMoves)
            tileManager.TileExit(move.Item2);
        validMoves.Clear();
    }

    private Dictionary<long, TTEntry> transpositionTable = new Dictionary<long, TTEntry>();

    private struct TTEntry
    {
        public BestMove bestMove;
        public int depth;
    }

    private struct BestMove
    {
        public int score;
        public SpecialMove specialMove;
        public Tile fromTile;
        public Tile toTile;
    }

    private async void StartBotThinkingAsync(System.Threading.CancellationToken token)
    {
        Debug.Log("Bot starts thinking...");
        BestMove bestMove = default;

        try
        {
            bestMove = await Task.Run(() => MinimaxAlphaBeta(3, int.MinValue, int.MaxValue, true, botTeam), token);

            token.ThrowIfCancellationRequested();

            if (bestMove.fromTile != null && bestMove.toTile != null)
            {
                Debug.Log($"Bot found best move from {bestMove.fromTile.name} to {bestMove.toTile.name} with score {bestMove.score}");
                ChessPiece movingPiece = bestMove.fromTile.chessPiece;

                moveManager.moveHistory.Push(new Move(bestMove.fromTile, bestMove.toTile, movingPiece, bestMove.toTile.chessPiece, bestMove.specialMove));
                movingPiece.MoveInBoard(bestMove.toTile, bestMove.specialMove);
                bestMove.fromTile.chessPiece = null;

                gameTurn = myTeamColor;
            }
            else
            {
                Debug.LogWarning("Bot could not find a valid move.");
            }

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Bot thinking cancelled.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during bot thinking: {ex}");
        }
        finally
        {
            isBotThinking = false;
            Debug.Log("Bot finished thinking.");
        }
    }


    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private int EvaluateBoard()
    {
        int materialScore = 0;
        int mobilityScore = 0;
        int centerScore = 0;

        bool IsCenterTile(Tile t) => (t.currentX >= 3 && t.currentX <= 4) && (t.currentY >= 3 && t.currentY <= 4);

        foreach (var tile in tileBoard)
        {
            if (tile.chessPiece != null)
            {
                int pieceValue = tile.chessPiece.score;
                int sign = (tile.chessPiece.teamColor == botTeam) ? 1 : -1;

                if (tile.chessPiece.chessType != ChessType.King)
                {
                    materialScore += pieceValue * sign;
                }

                Move lastMove = moveManager.moveHistory.Count > 0 ? moveManager.moveHistory.Peek() : null;
                try
                {
                    int movesCount = tile.chessPiece.GetAvailableMoves(tileBoard, lastMove).Count;
                    mobilityScore += movesCount * sign;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Lỗi khi lấy nước đi cho {tile.chessPiece?.name} trong EvaluateBoard: {ex.Message}");
                }

                if (IsCenterTile(tile))
                {
                    if (tile.chessPiece.chessType != ChessType.King)
                    {
                        centerScore += pieceValue * sign;
                    }
                }
            }
        }

        const int materialWeight = 100;
        const float mobilityWeight = 0.5f;
        const float centerWeight = 2.0f;

        int totalScore = (materialScore * materialWeight)
                       + (int)(mobilityScore * mobilityWeight)
                       + (int)(centerScore * centerWeight);

        return totalScore;
    }

    private bool IsCenterTile(Tile tile)
    {
        return (tile.currentX >= 3 && tile.currentX <= 4) && (tile.currentY >= 3 && tile.currentY <= 4);
    }

    private long ComputeBoardHash()
    {
        long hash = 17;
        foreach (var tile in tileBoard)
        {
            int pieceHash = (tile.chessPiece != null)
                ? ((int)tile.chessPiece.chessType * (tile.chessPiece.teamColor == TeamColor.White ? 1 : -1))
                : 0;
            hash = hash * 31 + tile.currentX;
            hash = hash * 31 + tile.currentY;
            hash = hash * 31 + pieceHash;
        }
        return hash;
    }

    private List<(SpecialMove, Tile)> OrderMoves(List<(SpecialMove, Tile)> moves, TeamColor currentTeam)
    {
        List<(SpecialMove, Tile)> ordered = new List<(SpecialMove, Tile)>(moves);
        ordered.Sort((moveA, moveB) =>
        {
            ChessPiece capturedA = SimulateMoveForOrdering(moveA.Item2);
            int scoreA = EvaluateBoard();
            UndoSimulateMoveForOrdering(moveA.Item2, capturedA);

            ChessPiece capturedB = SimulateMoveForOrdering(moveB.Item2);
            int scoreB = EvaluateBoard();
            UndoSimulateMoveForOrdering(moveB.Item2, capturedB);

            return scoreB.CompareTo(scoreA);
        });
        return ordered;
    }

    private ChessPiece SimulateMoveForOrdering(Tile toTile)
    {
        ChessPiece captured = toTile.chessPiece;
        return captured;
    }

    private void UndoSimulateMoveForOrdering(Tile toTile, ChessPiece capturedPiece)
    {
        toTile.chessPiece = capturedPiece;
    }

    private BestMove MinimaxAlphaBeta(int depth, int alpha, int beta, bool isMaximizing, TeamColor currentTeam)
    {
        BestMove bestMove = new BestMove();

        long boardHash = ComputeBoardHash();
        if (transpositionTable.TryGetValue(boardHash, out TTEntry ttEntry))
        {
            if (ttEntry.depth >= depth)
            {
                return ttEntry.bestMove;
            }
        }

        if (depth == 0)
        {
            bestMove.score = EvaluateBoard();
            return bestMove;
        }

        bestMove.score = isMaximizing ? int.MinValue : int.MaxValue;

        foreach (var tile in tileBoard)
        {
            if (tile.chessPiece == null || tile.chessPiece.teamColor != currentTeam)
                continue;

            Move lastMove = moveManager.moveHistory.Count > 0 ? moveManager.moveHistory.Peek() : null;
            List<(SpecialMove, Tile)> moves = tile.chessPiece.GetAvailableMoves(tileBoard, lastMove);

            List<(SpecialMove, Tile)> validMovesForPiece = new List<(SpecialMove, Tile)>();
            ChessPiece currentKing = (currentTeam == TeamColor.White) ? chessBoard.whiteKing : chessBoard.blackKing;
            foreach (var move in moves)
            {
                ChessPiece capturedPiece = SimulateMove(tile, move.Item2);
                if (!IsKingInCheck(currentKing))
                    validMovesForPiece.Add(move);
                UndoSimulateMove(tile, move.Item2, capturedPiece);
            }

            validMovesForPiece = OrderMoves(validMovesForPiece, currentTeam);

            foreach (var move in validMovesForPiece)
            {
                ChessPiece capturedPiece = SimulateMove(tile, move.Item2);
                BestMove currentMove = MinimaxAlphaBeta(depth - 1, alpha, beta, !isMaximizing, GetOpponentColor(currentTeam));
                UndoSimulateMove(tile, move.Item2, capturedPiece);

                if (isMaximizing)
                {
                    if (currentMove.score > bestMove.score)
                    {
                        bestMove.score = currentMove.score;
                        bestMove.specialMove = move.Item1;
                        bestMove.fromTile = tile;
                        bestMove.toTile = move.Item2;
                    }
                    alpha = Math.Max(alpha, bestMove.score);
                }
                else
                {
                    if (currentMove.score < bestMove.score)
                    {
                        bestMove.score = currentMove.score;
                        bestMove.specialMove = move.Item1;
                        bestMove.fromTile = tile;
                        bestMove.toTile = move.Item2;
                    }
                    beta = Math.Min(beta, bestMove.score);
                }
                if (beta <= alpha)
                    break;
            }
        }

        transpositionTable[boardHash] = new TTEntry { bestMove = bestMove, depth = depth };
        return bestMove;
    }

    private TeamColor GetOpponentColor(TeamColor team)
    {
        return team == TeamColor.White ? TeamColor.Black : TeamColor.White;
    }

    private IEnumerator BotMove()
    {
        yield return new WaitForSeconds(1f);

        BestMove bestMove = MinimaxAlphaBeta(3, int.MinValue, int.MaxValue, true, botTeam);
        if (bestMove.fromTile != null && bestMove.toTile != null)
        {
            ChessPiece movingPiece = bestMove.fromTile.chessPiece;
            movingPiece.MoveInBoard(bestMove.toTile, bestMove.specialMove);
            bestMove.fromTile.chessPiece = null;
            moveManager.moveHistory.Push(new Move(bestMove.fromTile, bestMove.toTile, movingPiece, bestMove.toTile.chessPiece, bestMove.specialMove));
            gameTurn = myTeamColor;
        }
        isBotThinking = false;
    }
    // --- Hàm kiểm tra và kết thúc Game ---

    /// <summary>
    /// Lấy tất cả các nước đi hợp lệ cho một đội.
    /// </summary>
    private List<(ChessPiece piece, SpecialMove moveType, Tile targetTile)> GetAllValidMovesForTeam(TeamColor team)
    {
        var allValidMoves = new List<(ChessPiece, SpecialMove, Tile)>();
        ChessPiece king = (team == TeamColor.White) ? chessBoard.whiteKing : chessBoard.blackKing;
        if (king == null)
        {
            Debug.LogError($"Cannot find King for team {team} to check valid moves.");
            return allValidMoves; // Không có vua -> không có nước đi hợp lệ
        }

        // Duyệt qua tất cả các ô trên bàn cờ
        for (int x = 0; x < tileBoard.GetLength(0); x++)
        {
            for (int y = 0; y < tileBoard.GetLength(1); y++)
            {
                Tile currentTile = tileBoard[x, y];
                // Nếu ô có quân cờ và quân đó thuộc đội cần kiểm tra
                if (currentTile.chessPiece != null && currentTile.chessPiece.teamColor == team)
                {
                    Move lastMove = moveManager.moveHistory.Count > 0 ? moveManager.moveHistory.Peek() : null;
                    // Lấy các nước đi có thể của quân cờ này
                    List<(SpecialMove, Tile)> potentialMoves = currentTile.chessPiece.GetAvailableMoves(tileBoard, lastMove);

                    // Kiểm tra tính hợp lệ cho từng nước đi (có làm Vua bị chiếu không?)
                    foreach (var potentialMove in potentialMoves)
                    {
                        ChessPiece captured = SimulateMove(currentTile, potentialMove.Item2);
                        if (!IsKingInCheck(king)) // Nếu Vua không bị chiếu sau khi đi thử
                        {
                            allValidMoves.Add((currentTile.chessPiece, potentialMove.Item1, potentialMove.Item2));
                        }
                        UndoSimulateMove(currentTile, potentialMove.Item2, captured); // Hoàn tác lại bàn cờ
                    }
                }
            }
        }
        return allValidMoves;
    }

    /// <summary>
    /// Kiểm tra trạng thái hiện tại của game (Checkmate, Stalemate).
    /// </summary>
    private void CheckGameStatus()
    {
        if (isGameOver) return; // Đã kết thúc rồi thì không kiểm tra nữa

        TeamColor teamWhoseTurnItIs = gameTurn; // Đội chuẩn bị đi nước tiếp theo
        List<(ChessPiece, SpecialMove, Tile)> validMovesForTeam = GetAllValidMovesForTeam(teamWhoseTurnItIs);

        // Nếu đội sắp tới lượt không còn nước đi hợp lệ nào
        if (validMovesForTeam.Count == 0)
        {
            ChessPiece king = (teamWhoseTurnItIs == TeamColor.White) ? chessBoard.whiteKing : chessBoard.blackKing;
            if (king == null)
            { // Xử lý trường hợp không tìm thấy vua
                Debug.LogError($"Cannot find King for team {teamWhoseTurnItIs} to check game status.");
                return;
            }

            if (IsKingInCheck(king)) // Nếu Vua đang bị chiếu -> Checkmate
            {
                // Đội vừa đi nước trước đó (đối thủ của đội hết nước đi) là đội chiến thắng
                TeamColor winningTeam = GetOpponentColor(teamWhoseTurnItIs);
                EndGame(winningTeam);
            }
            else // Nếu Vua không bị chiếu -> Stalemate (Hòa)
            {
                EndGameDraw();
            }
        }
        // Có thể thêm các điều kiện hòa cờ khác ở đây (ví dụ: bất biến 50 nước, lặp lại thế cờ 3 lần, thiếu quân để chiếu hết)
    }

    /// <summary>
    /// Xử lý kết thúc game với một đội chiến thắng.
    /// </summary>
    private void EndGame(TeamColor winningTeam)
    {
        if (isGameOver) return; // Đảm bảo chỉ gọi 1 lần

        isGameOver = true;
        Debug.Log($"Game Over! {winningTeam} wins!");

        // Hủy task suy nghĩ của bot nếu đang chạy
        cts?.Cancel();

        // Hiển thị màn hình tương ứng
        if (winningTeam == myTeamColor && winScreen != null)
        {
            winScreen.SetActive(true);
        }
        else if (winningTeam == botTeam && loseScreen != null)
        {
            loseScreen.SetActive(true);
        }
        // Có thể thêm hiệu ứng âm thanh, dừng thời gian, v.v.
    }

    /// <summary>
    /// Xử lý kết thúc game với kết quả hòa.
    /// </summary>
    private void EndGameDraw()
    {
        if (isGameOver) return; // Đảm bảo chỉ gọi 1 lần

        isGameOver = true;
        Debug.Log("Game Over! Draw (Stalemate).");

        // Hủy task suy nghĩ của bot nếu đang chạy
        cts?.Cancel();

        // Hiển thị màn hình hòa nếu có
        // if (drawScreen != null) drawScreen.SetActive(true);

        // Có thể thêm hiệu ứng âm thanh, dừng thời gian, v.v.
    }
}