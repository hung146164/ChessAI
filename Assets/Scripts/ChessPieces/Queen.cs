using System.Collections.Generic;

public class Queen : ChessPiece
{
    private void Awake()
    {
        chessType = ChessType.Queen;
        score = 9;
        moved = false;
    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();
        // Di chuyển theo hàng dọc, ngang.
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, 0));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, 0));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 0, 1));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 0, -1));
        // Di chuyển theo đường chéo.
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, 1));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, -1));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, -1, 1));
        tiles.AddRange(GetTilesInDirection(tileBoard, currentX, currentY, 1, -1));
        return tiles;
    }


}