using System.Collections.Generic;

public class Knight : ChessPiece
{
    private static readonly int[,] Moves =
    {
        { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 },
        { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 }
    };

    private void Awake()
    {
        chessType = ChessType.Knight;
        score = 3;
        moved = false;
    }

    public override List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();
        for (int i = 0; i < Moves.GetLength(0); i++)
        {
            int x = currentX + Moves[i, 0];
            int y = currentY + Moves[i, 1];
            if (IsInBounds(x, y) && (tileBoard[x, y].chessPiece == null || tileBoard[x, y].chessPiece.teamColor != this.teamColor))
            {
                tiles.Add((SpecialMove.None, tileBoard[x, y]));
            }
        }
        return tiles;
    }

}