﻿using UnityEngine;
using System.Collections;

public class TileData {

    //TODO: Holds data needed to create a Tile object.
    //TODO: Needs to be A*-able.

    Point position;

    public bool isBorder = false;
    public bool isTile = false;

    public bool disallowCorridor = false;


    public TileData(Point pos)
    {
        position = pos;
    }
	
}
