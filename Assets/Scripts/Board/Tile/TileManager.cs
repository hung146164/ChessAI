using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private Tile lastTileHover = null;
    public void TileHover(Tile tile)
    {
        if (tile == null || tile==lastTileHover) return;

        TileExit(lastTileHover);

        tile.SetHoverMaterial();

        lastTileHover = tile;
    }
    public void TileExit(Tile tile)
    {
        if (tile == null) return;

        tile.SetExitMaterial();
    }
    public void TileSelect(Tile tile)
    {
        if(tile == null) return;
        tile.SetSelectMaterial();
    }
    public void ValidTile(Tile tile)
    {
        if (tile == null) return;
        tile.SetValidMaterial();
    }
}
