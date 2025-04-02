using System.Collections.Generic;
public class MoveManager
{
    private Tile[,] board;
    public Stack<Move> moveHistory = new Stack<Move>();

    public MoveManager(Tile[,] board)
    {
        this.board = board;
    }
    // se làm thêm undo move trong tương lai 
}