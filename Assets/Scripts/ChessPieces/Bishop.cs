using System.Collections.Generic;

public class Bishop : ChessPiece
{
    private void Awake()
    {
        chessType = ChessType.Bishop;
        score = 3;
        moved = false;
    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, 1));   // Bottom-right
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, -1)); // Top-left
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, 1));  // Top-right
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, -1));  // Bottom-left
        return tiles;
    }
}