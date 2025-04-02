using UnityEngine;

[CreateAssetMenu(fileName = "ChessPieceData", menuName = "Data/ChessPieceData")]
public class ChessPieceData : ScriptableObject
{
    [Header("Black Team")]
    public ChessPiece kingBlackPrefab;

    public ChessPiece queenBlackPrefab;
    public ChessPiece bishopBlackPrefab;
    public ChessPiece knightBlackPrefab;
    public ChessPiece pawnBlackPrefab;
    public ChessPiece rookBlackPrefab;

    [Header("White Team")]
    public ChessPiece kingWhitePrefab;

    public ChessPiece queenWhitePrefab;
    public ChessPiece bishopWhitePrefab;
    public ChessPiece knightWhitePrefab;
    public ChessPiece pawnWhitePrefab;
    public ChessPiece rookWhitePrefab;
}