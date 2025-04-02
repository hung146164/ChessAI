using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public static ChessBoard Instance { get; private set; }

    public const int BOARD_ROWS = 8;
    public const int BOARD_COLUMNS = 8;

    public Tile[,] tileBoard;

    private float cellSize = 1.5f;


    public ChessPieceData chessPieceData;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float boardOriginX;
    [SerializeField] private float boardOriginY;

    [SerializeField] private Transform whiteDead;
    [SerializeField] private Transform blackDead;
    private int whitedeadCount;
    private int blackdeadCount;
    

    public ChessPiece whiteKing;
    public ChessPiece blackKing;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        CreateBoard();
        CreateAllChessPiece();
        whiteKing = tileBoard[4, 0].chessPiece;
        blackKing = tileBoard[4, 7].chessPiece;

    }

    /// <summary>
    ///  Done
    /// </summary>
    public Tile InstantiateTileAtPosition(Vector3 position)
    {
        Tile tile = Instantiate(tilePrefab, position, Quaternion.identity);
        tile.transform.SetParent(transform);
        return tile;
    }

    private void CreateBoard()
    {
        tileBoard = new Tile[BOARD_ROWS, BOARD_COLUMNS];
        for (int y = 0; y < BOARD_ROWS; y++)
        {
            for (int x = 0; x < BOARD_COLUMNS; x++)
            {
                Vector3 pos = new Vector3(boardOriginX + x * cellSize, 0, boardOriginY + y * cellSize);
                Tile tile = InstantiateTileAtPosition(pos);
                tile.currentX = x;
                tile.currentY = y;
                tileBoard[x, y] = tile;
            }
        }
    }

    private void CreateAllChessPiece()
    {
        CreatePiecesForTeam(chessPieceData.kingWhitePrefab, chessPieceData.queenWhitePrefab, chessPieceData.bishopWhitePrefab,
            chessPieceData.knightWhitePrefab, chessPieceData.pawnWhitePrefab, chessPieceData.rookWhitePrefab, 0);
        CreatePiecesForTeam(chessPieceData.kingBlackPrefab, chessPieceData.queenBlackPrefab, chessPieceData.bishopBlackPrefab,
            chessPieceData.knightBlackPrefab, chessPieceData.pawnBlackPrefab, chessPieceData.rookBlackPrefab, 7);
    }

    private void CreatePiecesForTeam(ChessPiece king, ChessPiece queen, ChessPiece bishop, ChessPiece knight, ChessPiece pawn, ChessPiece rook, int row)
    {
        AddPieceToBoard(rook, 0, row);
        AddPieceToBoard(knight, 1, row);
        AddPieceToBoard(bishop, 2, row);
        AddPieceToBoard(queen, 3, row);
        AddPieceToBoard(king, 4, row);
        AddPieceToBoard(bishop, 5, row);
        AddPieceToBoard(knight, 6, row);
        AddPieceToBoard(rook, 7, row);
        int pawnRow = (row == 0) ? 1 : 6;
        for (int i = 0; i < BOARD_COLUMNS; i++)
        {
            AddPieceToBoard(pawn, i, pawnRow);
        }
    }
    
    public void AddPieceToBoard(ChessPiece piecePrefab, int x, int y)
    {
        ChessPiece piece = Instantiate(piecePrefab, transform);
        piece.MoveInWorld(tileBoard[x, y]);
        tileBoard[x, y].chessPiece = piece;
    }

    public Tile GetTileAt(int x, int y) => tileBoard[x, y];
    public void RemovePieceFromTile(int x, int y) => tileBoard[x, y].chessPiece = null;
    public void PlacePieceAt(ChessPiece piece, int x, int y)
    {
        tileBoard[x, y].chessPiece = piece;
        piece.currentX = x;
        piece.currentY = y;
        piece.MoveInWorld(tileBoard[x, y]);
    }

    public void AddDeadPiece(ChessPiece piece)
    {
        piece.isDead = true;
        TeamColor team = piece.teamColor;
        piece.transform.position=((team == TeamColor.Black ? blackDead: whiteDead).transform.position+ new Vector3(0,0,(team==TeamColor.Black?-blackdeadCount:whitedeadCount)));
        if(team==TeamColor.Black)
        {
            blackdeadCount++;
        }
        else
        {
            whitedeadCount++;
        }
    }
}