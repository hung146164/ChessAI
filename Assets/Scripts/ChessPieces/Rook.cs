using System.Collections.Generic;

public class Rook : ChessPiece
{
    private void Awake()
    {
        chessType = ChessType.Rook;
        score = 5;
        moved = false;

    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();

        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, 0));   // Down
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, 0));  // Up
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 0, 1));   // Right
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 0, -1));  // Left

        return tiles;
    }
}