using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : ChessPiece
{
    private void Awake()
    {
        chessType = ChessType.Pawn;
        score = 1;
        moved = false;
    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();
        int direction = teamColor == TeamColor.White ? 1 : -1;

        // Nước đi 1 ô về phía trước
        if (IsInBounds(currentX, currentY + direction) && tileBoard[currentX, currentY + direction].chessPiece == null)
        {
            if (currentY + direction == 0 || currentY + direction == 7)
            {
                tiles.Add((SpecialMove.Promote, tileBoard[currentX, currentY + direction]));
            }
            else tiles.Add((SpecialMove.None, tileBoard[currentX, currentY + direction]));
        }

        // Nước đi 2 ô khi ở vị trí ban đầu
        if (((currentY == 1 && teamColor == TeamColor.White) || (currentY == 6 && teamColor == TeamColor.Black)) &&
            tileBoard[currentX, currentY + direction].chessPiece == null &&
            tileBoard[currentX, currentY + 2 * direction].chessPiece == null)
        {
            tiles.Add((SpecialMove.None, tileBoard[currentX, currentY + 2 * direction]));
        }

        // Ăn chéo thông thường
        if (IsInBounds(currentX - 1, currentY + direction) &&
            tileBoard[currentX - 1, currentY + direction].chessPiece != null &&
            tileBoard[currentX - 1, currentY + direction].chessPiece.teamColor != this.teamColor)
        {
            if (currentY + direction == 0 || currentY + direction == 7)
            {
                tiles.Add((SpecialMove.Promote, tileBoard[currentX - 1, currentY + direction]));
            }
            else tiles.Add((SpecialMove.None, tileBoard[currentX - 1, currentY + direction]));
        }
        if (IsInBounds(currentX + 1, currentY + direction) &&
            tileBoard[currentX + 1, currentY + direction].chessPiece != null &&
            tileBoard[currentX + 1, currentY + direction].chessPiece.teamColor != this.teamColor)
        {
            if (currentY + direction == 0 || currentY + direction == 7)
            {
                tiles.Add((SpecialMove.Promote, tileBoard[currentX + 1, currentY + direction]));
            }
            else tiles.Add((SpecialMove.None, tileBoard[currentX + 1, currentY + direction]));
        }
        if (previousMove != null &&
    previousMove.fromTile != null &&  // Thêm kiểm tra này
    previousMove.fromPiece != null &&
    previousMove.toTile != null &&
    previousMove.fromPiece.chessType == ChessType.Pawn) // Nếu nước trước là tốt di chuyển
        {
            int lastMoveX = previousMove.toTile.currentX;
            int lastMoveY = previousMove.toTile.currentY;

            // Kiểm tra nếu quân tốt đối phương vừa đi 2 ô

            if (Mathf.Abs(previousMove.fromTile.currentY - lastMoveY) == 2 && lastMoveY == currentY)
            {
                // Nếu tốt đối phương ở bên trái mình
                if (lastMoveX == currentX - 1)
                {
                    tiles.Add((SpecialMove.Enpassant, tileBoard[currentX - 1, currentY + direction]));
                }
                // Nếu tốt đối phương ở bên phải mình
                else if (lastMoveX == currentX + 1)
                {
                    tiles.Add((SpecialMove.Enpassant, tileBoard[currentX + 1, currentY + direction]));
                }
            }
        }

        return tiles;
    }

    public override void MoveInBoard(Tile targetTile, SpecialMove specialMove)
    {
        this.moved = true;

        if (specialMove == SpecialMove.None)
        {
            base.MoveInBoard(targetTile, specialMove);
            return;
        }
        if (specialMove == SpecialMove.Enpassant)
        {
            Tile tile = ChessBoard.Instance.tileBoard[targetTile.currentX, this.currentY];

            //giet
            ChessBoard.Instance.AddDeadPiece(tile.chessPiece);
            tile.chessPiece = null;

            //lay vi tri
            targetTile.chessPiece = this;
            MoveInWorld(targetTile);
            return;
        }
        if (specialMove == SpecialMove.Promote)
        {
            if (targetTile.chessPiece != null)
            {
                ChessBoard.Instance.AddDeadPiece(targetTile.chessPiece);
            }
            if (teamColor == TeamColor.White)
            {
                ChessBoard.Instance.AddPieceToBoard(ChessBoard.Instance.chessPieceData.queenWhitePrefab, targetTile.currentX, targetTile.currentY);
            }
            else
            {
                ChessBoard.Instance.AddPieceToBoard(ChessBoard.Instance.chessPieceData.queenBlackPrefab, targetTile.currentX, targetTile.currentY);
            }
            Destroy(gameObject);
        }
    }
}