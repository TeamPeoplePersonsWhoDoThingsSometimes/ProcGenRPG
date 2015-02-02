using UnityEngine;
using System.Collections;

public class Room {

    //A rectangular space within an Area.

    private Point position; //Bottom left corner of this room.

    private Point topRight; //Top Right corner of this room. (The length and width).

    public int length //X size of this Room.
    {
        get
        {
            return topRight.x - position.x;
        }
    }

    public int height //Y size of this Room.
    {
        get
        {
            return topRight.y - position.y;
        }
    }

    //Make a enum that represents a quest type, so that a room will know what to generated.

}
