using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    private static readonly int[,] MoveDirections =
    {
        { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 },
        { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }
    };

    private void OnEnable()
    {
        chessType = ChessType.King;
        score = 1000;
        moved = false;
    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> availableMoves = new List<(SpecialMove, Tile)>();

        // Di chuyển thông thường của vua
        for (int i = 0; i < MoveDirections.GetLength(0); i++)
        {
            int x = currentX + MoveDirections[i, 0];
            int y = currentY + MoveDirections[i, 1];
            if (IsInBounds(x, y) &&
               (tileBoard[x, y].chessPiece == null || tileBoard[x, y].chessPiece.teamColor != teamColor))
            {
                availableMoves.Add((SpecialMove.None, tileBoard[x, y]));
            }
        }

        // Kiểm tra castling (nếu chưa di chuyển)
        if (!moved)
        {
            // Castling queenside: rookOffset = -4 (vị trí xe ở cột 0 khi vua ở cột 4)
            if (CanCastle(tileBoard, -4))
                availableMoves.Add((SpecialMove.Castling, tileBoard[currentX - 2, currentY]));
            // Castling kingside: rookOffset = 3 (vị trí xe ở cột 7 khi vua ở cột 4)
            if (CanCastle(tileBoard, 3))
                availableMoves.Add((SpecialMove.Castling, tileBoard[currentX + 2, currentY]));
        }
        return availableMoves;
    }

    private bool CanCastle(Tile[,] tileBoard, int rookOffset)
    {
        int rookX = currentX + rookOffset;
        if (rookX < 0 || rookX > 7) return false;
        ChessPiece rook = tileBoard[rookX, currentY].chessPiece;
        if (rook == null || rook.chessType != ChessType.Rook || rook.moved)
            return false;

        int step = rookOffset > 0 ? 1 : -1;
        // Kiểm tra các ô giữa vua và xe phải trống
        for (int i = currentX + step; i != rookX; i += step)
        {
            if (tileBoard[i, currentY].chessPiece != null)
                return false;
        }
        return true;
    }

    public override void MoveInBoard(Tile targetTile, SpecialMove specialMove)
    {
        // Đánh dấu vua đã di chuyển
        this.moved = true;

        if (specialMove == SpecialMove.None)
        {
            // Nếu không phải castling, gọi phương thức di chuyển thông thường
            base.MoveInBoard(targetTile, specialMove);
            return;
        }

        // Castling
        // Nếu nước đi castling: 
        // - Nếu king di chuyển đến ô có currentX == 2, đó là queenside castling, xe nằm ở cột 0.
        // - Nếu king di chuyển đến ô có currentX == 6, đó là kingside castling, xe nằm ở cột 7.
        int targetX = targetTile.currentX;
        int rookX = (targetX == 2) ? 0 : 7;
        Tile rookTile = ChessBoard.Instance.tileBoard[rookX, this.currentY];
        // Tùy vào bên, xe sẽ di chuyển về ô 3 (queenside) hoặc ô 5 (kingside)
        Tile rookTo = (rookX == 0) ? ChessBoard.Instance.tileBoard[3, this.currentY] :
                                     ChessBoard.Instance.tileBoard[5, this.currentY];

        // Di chuyển xe: gọi MoveInWorld (hoặc MoveInBoard nếu có logic riêng cho xe)
        rookTile.chessPiece.MoveInWorld(rookTo);
        // Gán xe vào ô mới, xóa xe khỏi ô ban đầu
        rookTo.chessPiece = rookTile.chessPiece;
        rookTile.chessPiece = null;

        // Di chuyển vua vào ô đích
        targetTile.chessPiece = this;
        MoveInWorld(targetTile);
    }
}
