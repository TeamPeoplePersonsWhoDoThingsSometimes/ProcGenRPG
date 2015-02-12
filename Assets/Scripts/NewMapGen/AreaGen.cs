using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AreaGen {

    //Takes in references to a TileData array and Room list, creates an Area, and puts the Area back into these inputs.
    public static void defaultGen(int seed, out TileData [,] tiles, out List<Room> rooms)
    {
        //Create tiles and rooms.
        tiles = new TileData[50, 50];
        rooms = new List<Room>();

        //TODO: Generate rooms, put in room List, and place on Tile array.
        defaultRoomGen(seed, ref tiles, ref rooms);

        //TODO: Do A* to connect each room. (Maybe have a list of corridor tiles, which A* creates?)
        defaultConnect(seed, ref tiles, ref rooms);
    }

    private static void defaultRoomGen(int seed, ref TileData[,] tiles, ref List<Room> rooms)
    {
        //TODO: Implement this method.

        System.Random random = new System.Random();

        //TODO: Determine number of Rooms to make.
        int numOfRooms = random.Next(5, 10);

        int currentRoom = 0;
        int failures = 0;

        //Can we require that a Room be within some min distance of another Room, yet further than a max distance?


        //While we still need more Rooms, and we've failed to make a room less than 5 times in a row.
        while (currentRoom < numOfRooms && failures < 6)
        {
            //Create random sized room.
            int xSize = random.Next(4, 8);
            int ySize = random.Next(4, 8);

            //Create random botLeft Point for the Room.
            Point placement = new Point(random.Next(0, tiles.GetLength(0)), random.Next(0, tiles.GetLength(1)));

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
                //TODO: Place the Room

                //NOTE: When placing a Room, the tiles bordering the Room need to be marked as "non-Room-Able" tiles,
                //so that Rooms can't be placed there.
                //(MAYBE?) corners need to be marked as "non-corridor-able," since we don't
                //want corridors to enter the corner of rooms. That'd be odd.

                //TODO: Add this Room to the Area, as tiles.

                failures = 0;
            }
            else
            {
                failures++;
            }
            
        }
    }

    private static void defaultConnect(int seed, ref TileData[,] tiles, ref List<Room> rooms)
    {
        //TODO: Implement this method.

        //NOTE: Needs to create the weight array the same time every time, and connect the Rooms
            //the same way every time.


    }

}
