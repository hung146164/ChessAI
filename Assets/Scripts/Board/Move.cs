public class Move
{
    public Tile fromTile { get; private set; }
    public Tile toTile { get; private set; }
    public ChessPiece fromPiece { get; private set; }
    public ChessPiece toPiece { get; private set; }
    public SpecialMove specialMove { get; private set; }

    public Move(Tile fromTile, Tile toTile, ChessPiece fromPiece, ChessPiece toPiece, SpecialMove specialMove)
    {
        this.fromTile = fromTile;
        this.toTile = toTile;
        this.fromPiece = fromPiece;
        this.toPiece = toPiece;
        this.specialMove = specialMove;
    }
}
