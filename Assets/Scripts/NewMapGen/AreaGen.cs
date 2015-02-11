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

        //Can we require that a Room be within some min distance of another Room, yet further than a max distance?

        //TODO: Create random sized room.

        //TODO: Check to see if this room fits in the Area, and doesn't overlap other rooms.
            //Checking for overlaping only needs to check the borders of the Room.
            //NEEDS to ensure that it's not trying to place an impossible room indefinitely.

        //NOTE: When placing a Room, the tiles bordering the Room need to be marked as "non-Room-Able" tiles,
            //so that Rooms can't be placed there.
            //(MAYBE?) corners need to be marked as "non-corridor-able," since we don't
            //want corridors to enter the corner of rooms. That'd be odd.

        //TODO: Add this Room to the Area, as tiles.
    }

    private static void defaultConnect(int seed, ref TileData[,] tiles, ref List<Room> rooms)
    {
        //TODO: Implement this method.

        //NOTE: Needs to create the weight array the same time every time, and connect the Rooms
            //the same way every time.


    }

}
