using UnityEngine;
using System.Collections;

public class Area {

    //Areas which this Area is connected to. True, if connected to an area in that direction.
    public bool north;
    public bool east;
    public bool south;
    public bool west;

    //This area's position on the map.
    Point position;

    public Area(Point position, bool N, bool E, bool S, bool W)
    {
        this.position = position;

        north = N;
        east = E;
        south = S;
        west = W;
    }

    //Returns the neighbors connected to this Area.
    public Area[] getNeighbors()
    {
        //TODO: Implement this method.
        return null;
    }
}
