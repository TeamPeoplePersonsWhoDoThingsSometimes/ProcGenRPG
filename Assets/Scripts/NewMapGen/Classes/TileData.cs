using UnityEngine;
using System.Collections;

public class TileData {

    //TODO: Holds data needed to create a Tile object.
    //TODO: Needs to be A*-able.

    Point position;

    public bool isBorder = false; //If this Tile is a wall.
    public bool isTile = false; //If this Tile is a walkable Tile.

    public bool disallowCorridor = false; //If this Tile cannot be a Walkable Tile.

    public bool isPortal = false; //If this Tile is a portal.
    public Direction portalDirection; //Direction of the portal


    public TileData(Point pos)
    {
        position = pos;
    }

}

public enum Direction
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}
