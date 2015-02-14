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

    //Note: generateRoom() will NOT generate the Tiles. Only the insides of the Room.
            
    //TODO: Create a function and varible that requires Quest Material to be generated in this Room.

    private Point botLeft; //Bottom left corner of this room.
    private Point topRight; //Top Right corner of this room.

    public int length //X size of this Room.
    {
        get
        {
            return (topRight.x - botLeft.x) + 1;
        }
    }

    public int height //Y size of this Room.
    {
        get
        {
            return (topRight.y - botLeft.y) + 1;
        }
    }

    public Room(Point botLeft, Point topRight)
    {
        this.botLeft = botLeft;
        this.topRight = topRight;
    }

    #region Public Methods

    //Returns true if this Room intersects the other Room. False, otherwise.
    public bool intersects(Room other)
    {
        //If this Rect's MIN X position is greater than other's MAX X position, they'll never touch in any dimension.
        //If this Rect's MAX X position is less than other's MIN X position, they'll never touch in any dimension.

        //Same goes for other dimensions.

        //If neither of these are true in one dimension, they have a common point in that dimension.
        //If neither of these are true for ALL dimensions, then they MUST intersect at some point in space.
        //Inversely, if any of these are true, they will never intersect.

        return !(this.botLeft.x > other.topRight.x || this.topRight.x < other.botLeft.x
             || this.botLeft.y > other.topRight.y || this.topRight.y < other.botLeft.y);
    }

    public Point getBotLeft()
    {
        return botLeft;
    }
    public Point getTopRight()
    {
        return topRight;
    }

    #endregion

    //Make a enum that represents a quest type, so that a room will know what to generated.

}
