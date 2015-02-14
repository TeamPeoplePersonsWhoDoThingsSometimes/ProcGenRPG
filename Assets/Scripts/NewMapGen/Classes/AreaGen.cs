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

    //Takes in references to a TileData array and Room list, creates an Area, and puts the Area back into these inputs.
    public static void defaultGen(int seed, out TileData [,] tiles, out List<Room> rooms, out List<TileData> corridors)
    {
        //Create tiles and rooms.
        tiles = new TileData[30, 30];
        rooms = new List<Room>();

        //Generate rooms, put in room List, and place on Tile array.
        defaultRoomGen(seed, ref tiles, ref rooms);

        //Do A* to connect each room.
        corridors = defaultConnect(seed, ref tiles, ref rooms);

        placeCorridors(ref tiles, corridors);
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
            Point placement = new Point(random.Next(0, tiles.GetLength(0) - xSize), random.Next(0, tiles.GetLength(1) - ySize));

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
        //TODO: Implement this method.

        //Generate the weight map for A*.
        int[,] weights = generateWeight(seed, tiles);

        List<TileData> corridors = new List<TileData>();

        //TODO: Choose Rooms to connect.
        for (int i = 1; i < rooms.Count; i++)
        {

            //Plan:
            //Add starting point to Open List.
            //Sort Open list by F value, which is the distance from the end (found by heuristic) + currentCost.
            //While Open List is NOT empty
                //Get next Point to calculate, and put it on closed list.
                //foreach neighbor to this nextPoint
                    //If next point is the final point, make connections and break.
                    //If next point is not walkable, continue.
                    //If next point is NOT on the open List, calc ALL it's moveCost values F,G,H, and set the nextPoint
                        //as it's cameFrom point.
                    //If this point is ON the open List (and maybe the closed),
                        //if this REALcost (G, not heuristic) from this nextPoint is better than
                        //the one it already has, replace its cameFrom with this point, and re-calc its F,G,H

            //OpenList sorted by F values.
            PriorityQueue<int, path> openList = new PriorityQueue<int, path>();


        }

        return corridors;
    }

    #region Helper Methods

    //Puts the input Room on the map as a TileData object.
    private static void placeRoom(ref TileData[,]tiles, Room r)
    {
        Point botLeft = r.getBotLeft();
        Point topRight = r.getTopRight();

        for(int i = botLeft.x; i < topRight.x + 1; i++)
        {
            for(int j = botLeft.y; j < topRight.y; j++)
            {
                tiles[i, j] = new TileData();
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
                    weights[i, j] = random.Next(2, 5);
                }
                else
                {
                    //If this is part of a Room, then set its weight to one, to prefer already existing paths.
                    weights[i, j] = 0;
                }
            }
        }

        return weights;
    }

    //Iterates through the list of Corridors and places them all on the tile map.
    private static void placeCorridors(ref TileData[,] tiles, List<TileData> corridors)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    //Internal class used to help generate corridors.
    internal class path
    {
        int cost;         //G
        int estDist;      //H
        int probableCost; //F = G + H

        Point cameFrom;

        bool closedList = false;

        internal path(Point cameFrom)
        {
            this.cameFrom = cameFrom;
        }

        //Returns an array of the neighbors of this path.
        public path[] getNeighbors()
        {
            return null;
        }

        public Point getCameFrom()
        {
            return cameFrom;
        }
    }
}
