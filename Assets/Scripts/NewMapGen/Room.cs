using UnityEngine;
using System.Collections;

public class Room {

    //A rectangular space within an Area.

    //TODO: Create a generateRoom(AreaType a) function, that takes in its AreaType, which is the tileSet of the Area.
            //If (Quest Material is Required in this room)
                //Generate the quest material + tileset Decorations.
            //Else
                //Use its tileset to decorate the Room.
                //Maybe generate some enemies in this Room.
                //Whatever.
            
    //TODO: Create a function and varible that requires Quest Material to be generated in this Room.

    private Point botLeft; //Bottom left corner of this room.
    private Point topRight; //Top Right corner of this room. (The length and width).

    public int length //X size of this Room.
    {
        get
        {
            return topRight.x - botLeft.x;
        }
    }

    public int height //Y size of this Room.
    {
        get
        {
            return topRight.y - botLeft.y;
        }
    }

    //Make a enum that represents a quest type, so that a room will know what to generated.

}
