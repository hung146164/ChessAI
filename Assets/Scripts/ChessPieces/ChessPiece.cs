using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public TeamColor teamColor;
    public ChessType chessType;

    public AnimationCurve liftCurve;
    public int score;
    public bool isDead = false;
    public bool moved = false;
    public int currentX, currentY;

    private void OnEnable()
    {
        isDead = false;
        moved = false;
    }

    public abstract List<(SpecialMove, Tile)> GetAvailableMoves(Tile[,] tileBoard, Move previousMove);
    public virtual void MoveInBoard(Tile targetTile, SpecialMove specialMove)
    {
        this.moved = true;

        if (targetTile.chessPiece != null)
        {
            ChessBoard.Instance.AddDeadPiece(targetTile.chessPiece);
        }
        targetTile.chessPiece = this;
        
        MoveInWorld(targetTile);
        
    }

    /// <summary>
    /// Kiểm tra trong phạm vi bàn cờ.
    /// </summary>
    protected bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < ChessBoard.BOARD_ROWS && y >= 0 && y < ChessBoard.BOARD_COLUMNS;
    }

    protected List<(SpecialMove, Tile)> GetTilesInDirection(Tile[,] tileBoard, int startX, int startY, int dx, int dy)
    {
        List<(SpecialMove, Tile)> tiles = new List<(SpecialMove, Tile)>();
        int x = startX + dx;
        int y = startY + dy;

        while (IsInBounds(x, y))
        {
            if (tileBoard[x, y].chessPiece == null)
            {
                tiles.Add((SpecialMove.None, tileBoard[x, y]));
            }
            else
            {
                if (tileBoard[x, y].chessPiece.teamColor != this.teamColor)
                    tiles.Add((SpecialMove.None, tileBoard[x, y]));
                break;
            }
            x += dx;
            y += dy;
        }
        return tiles;
    }

    /// <summary>
    /// Di chuyển quân cờ trong không gian world.
    /// </summary>
    public void MoveInWorld(Tile tile)
    {
        this.currentX = tile.currentX;
        this.currentY= tile.currentY;
        StartCoroutine(MoveWithLift(tile.transform.position));
    }

    private IEnumerator MoveWithLift(Vector3 targetPosition)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            float lift = liftCurve.Evaluate(t);
            currentPosition.y += lift;
            transform.position = currentPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}