using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {

    //A rectangular space within an Area.

    //TODO: Create a function and varible that requires Quest Material to be generated in this Room.

    #region Variables

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

	public Point center //Point at the center of this Room.
	{
		get
		{
            Point diff = (topRight - botLeft);
            diff.x /= 2;
            diff.y /= 2;
            return botLeft + diff;
		}
	}

    List<GameObject> objects;
	List<GameObject> spawnedObjects;

    Area parent;

    public bool isGenerated = false;
    public bool isShowing = false;

    #endregion


    #region Constructors

    public Room(Area area, Point botLeft, Point topRight)
    {
		objects = new List<GameObject> ();
		spawnedObjects = new List<GameObject> ();
        this.botLeft = botLeft;
        this.topRight = topRight;
		parent = area;
    }

    #endregion


    #region Generation Methods

    //Generates and shows the Room, if not yet Generated. Otherwise, shows the Room.
    public void showRoom(System.Random random)
    {
        if (!isGenerated)
        {            
            objects = RoomGen.generateRoom(random.Next(10));

			foreach (GameObject g in spawnedObjects) {
				g.SetActive(true);
			}

            isGenerated = true;
            isShowing = true;
        }
        else if (!isShowing)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(true);
            }

			foreach (GameObject g in spawnedObjects) {
				g.SetActive(true);
			}

            isShowing = true;
        }


    }

    public void hideRoom()
    {
        if (isShowing)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(false);
            }

			foreach (GameObject g in spawnedObjects)
			{
				g.SetActive(false);
			}

            isShowing = false;
        }
    }

    public void destroyRoom()
    {
        if (isGenerated)
        {
            foreach(GameObject g in objects)
            {
                GameObject.Destroy(g);
            }

			//TODO optimize
			foreach (GameObject g in spawnedObjects)
			{
				g.SetActive(false);
			}

            isGenerated = false;
            isShowing = false;
        }
    }

    public void createPortal(Direction dir, ref TileData[,] tiles)
    {
        if (dir == null)
        {
            throw new System.ArgumentException("Input Direction cannot be null.");
        }

        System.Random random = new System.Random(100);

        int x;
        TileData temp = null;

        switch (dir) {
            case (Direction.UP):
                x = topRight.x - (length / 2 + random.Next(-1, 1));
                temp = tiles[x, topRight.y + 1];
                break;
            case (Direction.LEFT):
                x = topRight.y - (height / 2 + random.Next(-1, 1));
                temp = tiles[botLeft.x - 1, x];
                break;
            case (Direction.DOWN):
                x = botLeft.x + (length / 2 + random.Next(-1, 1));
                temp = tiles[x, botLeft.y - 1];
                break;
            case (Direction.RIGHT):
                x = botLeft.y + (height / 2 + random.Next(-1, 1));
                temp = tiles[topRight.x + 1, x];
                break;
        }

        temp.isBorder = false;
        temp.isTile = false;
        temp.isPortal = true;
        temp.portalDirection = dir;
    }

    #endregion


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

    public void generateQuestMaterial(SpawnCommand sc)
    {
		System.Random random = new System.Random();

		for (int i = 0; i < sc.getQuantity(); i++) {
			//Get a random point in the Room.
			Point place = new Point(random.Next(botLeft.x, topRight.x), random.Next(botLeft.y, topRight.y)) * 10;

			GameObject obj = sc.spawnObject(new Vector3(place.x, 5, place.y));

			if (!parent.Equals(MasterDriver.Instance.CurrentArea)) {
				obj.SetActive(false);
			}

			spawnedObjects.Add(obj);
		}
    }

    #endregion


    #region Helper Methods

    /*private void spawnQuestObjects()
    {
        System.Random random = new System.Random();

        //Make as many objects as specified.
        for (int i = 0; i < quantity; i++)
        {
            //Get a random point in the Room.
            Point place = new Point(random.Next(botLeft.x, topRight.x), random.Next(botLeft.y, topRight.y)) * 10;

            GameObject.Instantiate(questObject, place.toVector3(), Quaternion.identity);
        }

    }*/

    #endregion

}
