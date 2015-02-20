using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PriorityQueue;

public static class AreaGen {

    //NOTE: When placing a Room, the tiles bordering the Room need to be marked as "non-Room-Able" tiles,
    //so that Rooms can't be placed there.
    //(MAYBE?) corners need to be marked as "non-corridor-able," since we don't
    //want corridors to enter the corner of rooms. That'd be odd.

    //Can we require that a Room be within some min distance of another Room, yet further than a max distance?

    #region Default Gen

    //Takes in references to a TileData array and Room list, creates an Area, and puts the Area back into these inputs.
    public static void defaultGen(int seed, out TileData [,] tiles, out List<Room> rooms, out List<TileData> corridors)
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        watch.Start();

        //Create tiles and rooms.
        tiles = new TileData[30, 30];
        rooms = new List<Room>();

        //Generate rooms, put in room List, and place on Tile array.
        defaultRoomGen(seed, ref tiles, ref rooms);

        //Do A* to connect each room.
        corridors = defaultConnect(seed, ref tiles, ref rooms);

        watch.Stop();
        Debug.Log("Area Generation time: " + watch.Elapsed);

    }

    private static void defaultRoomGen(int seed, ref TileData[,] tiles, ref List<Room> rooms)
    {
        System.Random random = new System.Random();

        //Determine number of Rooms to make.
        int numOfRooms = random.Next(5, 10);

        int currentRoom = 0;
        int failures = 0;

        //While we still need more Rooms, and we've failed to make a room less than 5 times in a row.
        while (currentRoom < numOfRooms && failures < 6)
        {
            //Create random sized room.
            int xSize = random.Next(4, 8);
            int ySize = random.Next(4, 8);

            //Create random botLeft Point to place the Room.
            Point placement = new Point(random.Next(1, tiles.GetLength(0) - xSize - 1), random.Next(1, tiles.GetLength(1) - ySize - 1));

            Room newRoom = new Room(placement, new Point(placement.x + xSize, placement.y + ySize));

            bool roomFailed = false;
            //Test to ensure this Room doesn't overlap other rooms.
            foreach (Room r in rooms)
            {
                if (newRoom.intersects(r))
                {
                    roomFailed = true;
                    break;
                }
            }

            if(!roomFailed)
            {
                //Place the Room as Tiles in the Area.
                placeRoom(ref tiles, newRoom);

                rooms.Add(newRoom);

                //Reset the failure count, since this was a success!
                failures = 0;
                currentRoom++;
            }
            else
            {
                failures++;
            }
            
        }
    }

    //Creates corridors from room to room,and returns the list of corridors. (Rooms tiles are not included in corridors).
    private static List<TileData> defaultConnect(int seed, ref TileData[,] tiles, ref List<Room> rooms)
    {
        //Generate the weight map for A*.
        int[,] weights = generateWeight(seed, tiles);

        List<TileData> corridors = new List<TileData>();

        //Connect first Room to all others.
        for (int i = 1; i < rooms.Count; i++)
        {
            //Get starting point. (middle of start Room)
            Point start = rooms[0].getTopRight();
            start = new Point(start.x - rooms[0].length / 2, start.y - rooms[0].height / 2);

            //Get ending point. (middle of other Room)
            Point end = rooms[i].getTopRight();
            end = new Point(end.x - rooms[i].length / 2, end.y - rooms[i].height / 2);

            //Plan:
            //Add starting point to Open List.
            //Sort Open list by F value, which is the distance from the end (found by heuristic) + currentCost.
            //While Open List is NOT empty
                //Get next Point to calculate, and put it on closed list.
                //foreach neighbor to this nextPoint
                    //If neighbor point is the final point, make connections and break.
                    //If neighbor point is not walkable, continue.
                    //If neighbor point is NOT on the open List, calc ALL it's moveCost values F,G,H, and set the next Point
                        //as it's cameFrom point.
                    //If neighbor point is ON the open List (and maybe the closed),
                        //if this REALcost (G, not heuristic) from this nextPoint is better than
                        //the one it already has, replace its cameFrom with next point, and re-calc its F,G,H

            //OpenList sorted by F values.
            PriorityQueue<int, path> openList = new PriorityQueue<int, path>();

            pathMap map = new pathMap(tiles.GetLength(0), tiles.GetLength(1));

            path startPath = map.getPath(start);
            startPath.cameFrom = null; //Starting point came From null. (this is a flag for later)
            startPath.openList = true;
            startPath.setValues(0, 0, 0); //Start point doesn't need values.

            openList.Enqueue(0, startPath);

            bool isFinished = false;

            while (!openList.IsEmpty && !isFinished)
            {
                path next = openList.DequeueValue();
                next.closedList = true;

                foreach (path neighbor in next.getNeighbors())
                {
                    if (neighbor.position.Equals(end))
                    {
                        //Do ending stuff!
                        isFinished = true;

                        neighbor.cameFrom = next;

                        //Start function to get the path, and put the path into corridors, and put the corridors on the tile map.
                        corridors.AddRange(getFinishedPath(neighbor, ref tiles));

                        break;
                    }

                    //If not walkable, then check for that here. (currently not possible)

                    if (!neighbor.openList)
                    {
                        //PUT on open List.
                        neighbor.openList = true;

                        neighbor.cameFrom = next;
                        neighbor.setValues(next.cost + weights[neighbor.position.x, neighbor.position.y], neighbor.position.tileDiff(end));

                        openList.Enqueue(neighbor.probableCost, neighbor);
                    }
                    else if (!neighbor.closedList)
                    {
                        //Compare its current values, and change em if need be.
                        int newCost = next.cost + weights[neighbor.position.x, neighbor.position.y];
                        if (newCost < neighbor.cost)
                        {
                            //May not actually work...
                            KeyValuePair<int, path> oldPair = new KeyValuePair<int, path>(neighbor.probableCost, neighbor);

                            openList.Remove(oldPair);
                            neighbor.setValues(newCost, neighbor.position.tileDiff(end));

                            openList.Enqueue(neighbor.probableCost, neighbor);
                        }
                    }
                }

            } //End of While Loop


        } //End of For Loop

        return corridors;
    }

    #endregion


    #region Helper Methods

    //Puts the input Room on the map as TileData objects, and surrounds it with border.
    private static void placeRoom(ref TileData[,]tiles, Room r)
    {
        Point botLeft = r.getBotLeft();
        Point topRight = r.getTopRight();

        //Add Room to map.
        for(int i = botLeft.x; i < topRight.x + 1; i++)
        {
            for(int j = botLeft.y; j < topRight.y + 1; j++)
            {
                TileData temp = new TileData(new Point(i, j));
                temp.isTile = true;
                tiles[i, j] = temp;
            }
        }

        //Add border to Map.

        //Move along x direction.
        for (int i = botLeft.x - 1; i < topRight.x + 2; i++)
        {
            //If on the left/right edges
            if (i == botLeft.x - 1 || i == topRight.x + 1)
            {
                for (int j = botLeft.y - 1; j < topRight.y + 2; j++)
                {
                    if (tiles[i, j] == null || !tiles[i, j].isTile)
                    {
                        TileData temp = new TileData(new Point(i, j));
                        temp.isBorder = true;
                        tiles[i, j] = temp;
                    }
                }
            }
            else
            {
                TileData temp = new TileData(new Point(i, botLeft.y - 1));
                temp.isBorder = true;
                tiles[i, botLeft.y - 1] = temp;

                TileData temp2 = new TileData(new Point(i, botLeft.y - 1));
                temp2.isBorder = true;
                tiles[i, topRight.y + 1] = temp2;
            }
        }

    }

    //Generates a randomized 2D int Array, containing numbers 1-5.
    private static int[,] generateWeight(int seed, TileData[,] tiles)
    {
        int length = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        System.Random random = new System.Random(seed);
        int[,] weights = new int[length, height];

        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (tiles[i, j] == null)
                {
                    weights[i, j] = random.Next(2, 8);
                }
                else if (tiles[i, j].isBorder)
                {
                    //Don't go through borders, if possible.
                    weights[i, j] = 5;
                }
                else
                {
                    //If this is part of a Room, then set its weight to one, to prefer already existing paths.
                    weights[i, j] = 1;
                }
            }
        }

        return weights;
    }

    //Gets the final path from the A* algorithm.
    private static List<TileData> getFinishedPath(path endPoint, ref TileData[,] tiles)
    {
        path nextPath = endPoint;
        List<TileData> finishedPath = new List<TileData>();
        
        //While we're not yet at the start.
        while (nextPath.cameFrom != null)
        {
            Point pos = nextPath.position;

            //If this tile isn't already there, or it's just a border tile.
            if (tiles[pos.x, pos.y] == null || tiles[pos.x, pos.y].isBorder)
            {
                //Make a new TileData there.
                TileData temp = new TileData(pos);
                temp.isTile = true;
                tiles[pos.x, pos.y] = temp;
                finishedPath.Add(temp);

                //TODO: Check TileDatas around this one, to add borders to paths.
                foreach(Point p in pos.getAdjacent(true))
                {
                    if (tiles[p.x, p.y] == null || !tiles[p.x, p.y].isTile)
                    {
                        TileData temp2 = new TileData(p);
                        temp2.isBorder = true;
                        tiles[p.x, p.y] = temp2;

                    }
                }
            }

            nextPath = nextPath.cameFrom;
        }

        return finishedPath;
    }

    #endregion


    #region Internal Classes

    //Internal class used to help generate corridors.
    internal class pathMap
    {
        path[,] map;

        internal pathMap(int length, int height)
        {
            map = new path[length, height];
        }

        internal path getPath(Point p)
        {
            return getPath(p.x, p.y);
        }
        internal path getPath(int x, int y)
        {
            if (map[x,y] == null)
            {
                map[x, y] = new path(new Point(x, y), this);
            }

            return map[x, y];
        }

        internal bool isWithinMap(Point p)
        {
            return (p.x > -1 && p.y > -1 && p.x < map.GetLength(0) && p.y < map.GetLength(1));
        }
    }

    //Internal class used to help generate corridors.
    internal class path
    {
        internal int cost;         //G
        internal int estDist;      //H
        internal int probableCost; //F = G + H

        internal path cameFrom;
        internal Point position;

        internal pathMap map;

        internal bool closedList = false;
        internal bool openList = false;

        internal path(Point myPoint, pathMap myMap)
        {
            this.position = myPoint;
            this.map = myMap;
        }

        //Returns an array of the neighbors of this path.
        internal List<path> getNeighbors()
        {
            Point[] points = position.getAdjacent();

            List<path> paths = new List<path>();
            
            foreach(Point p in points)
            {
                if (map.isWithinMap(p))
                {
                    paths.Add(map.getPath(p));
                }
            }

            return paths;
        }

        internal void setValues(int G, int H, int F)
        {
            this.cost = G;
            this.estDist = H;
            this.probableCost = F;
        }

        internal void setValues(int G, int H)
        {
            setValues(G, H, G + H);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            path p = obj as path;
            if ((System.Object)p == null)
            {
                return false;
            }

            return this.position.Equals(p.position);
        }

    }

    #endregion


}
