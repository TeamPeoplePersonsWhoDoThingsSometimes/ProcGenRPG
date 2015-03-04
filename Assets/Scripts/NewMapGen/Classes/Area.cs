using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Area
{

    #region Plans

    //TODO: Multithreaded Area generation, when the player is in an adjacent Area. To reduce load times.

    //TODO: Create a function that randomly assigns a quest to one of its Rooms.

    #endregion


    #region Variables

    //AreaGroup which this Area is a part of.
    private AreaGroup group;

    //Areas which this Area is connected to. True, if connected to an area in that direction.
    public bool north;
    public bool east;
    public bool south;
    public bool west;


    //This area's position on the map.
    public Point position
    {
        get;
        private set;
    }

	//This area's default spawn location if the player did not enter the area via a portal
	public Point defaultSpawn
	{
		get
		{
			return rooms[0].center*10;
		}
	}
    
    //The Map which this Area is a part of.
    private Map parentMap;

    private List<Room> rooms; //The rooms which this Area contains.
    private List<TileData> corridors; //This might be useless?
    private TileData[,] tiles;

    private List<Tile> objects;

    private int areaSeed;

    private bool isGenerated = false;
    private bool isShowing = false;
    private bool isHidden = false;

    #endregion


    #region Constructors

    public Area(Map parentMap, Point position, int seed, bool N, bool E, bool S, bool W)
    {
        this.parentMap = parentMap;
        this.position = position;
        this.areaSeed = seed;

        north = N;
        east = E;
        south = S;
        west = W;

        rooms = new List<Room>();

    }

    #endregion


    #region Generation methods

    //Generates the 2D array and Rooms List needed to show this area.
    public void generateArea()
    {
        //Default Area Generation.
        AreaGen.defaultGen(areaSeed, out tiles, out rooms, out corridors);

        isGenerated = true;

        //Will have a switch statement determining which mapGenerator to use based on the tileSet, if we
            //want to have Area's generated differently.
    }

    //Creates GameObjects of the 2D array, and displays them. Or if they're already created, just display them.
    public void showArea()
    {
        //Don't do anything, if this area is already showing.
        if (!isShowing)
        {
            //Ensure that this area has been generated. If not, do the generation now.
            if (!isGenerated)
            {
                generateArea();
            }

            //Determine the TileSet to be used.
            TileSet mySet;
            switch (group.type)
            {
                case(AreaType.GrassyPath):
                    mySet = LoadResources.instance.grassyPath.GetComponent<TileSet>();
                    break;
                case(AreaType.Dungeon):
                    mySet = LoadResources.instance.dungeon.GetComponent<TileSet>();
                    break;
                default:
                    throw new System.MissingFieldException("Area does not have an Area Group!");
            }

            objects = new List<Tile>();

            GameObject parent = new GameObject();
            parent.name = "Area Parent";

            //Create the GameObjects by iterating through the information.
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    //Do GameObject creation here.
                    if (tiles[i,j] != null)
                    {

                        if (tiles[i, j].isTile)
                        {
							Tile temp = (Tile)GameObject.Instantiate(mySet.tiles[0], new Vector3(i * 10, mySet.tiles[0].y, j * 10), Quaternion.identity);
                            temp.transform.parent = parent.transform;
                            objects.Add(temp);
                        }
                        else if (tiles[i,j].isBorder)
                        {
							Tile temp = (Tile)GameObject.Instantiate(mySet.tiles[1], new Vector3(i * 10, mySet.tiles[1].y, j * 10), Quaternion.identity);
                            temp.transform.parent = parent.transform;
                            objects.Add(temp);
                        }

                    }
                }
            }

            System.Random random = new System.Random(areaSeed);

            foreach(Room r in rooms)
            {
                r.showRoom(random);
            }

            isShowing = true;
        }

    }

    //Disables the GameObjects for this Area.
    public void hideArea()
    {
        if (isShowing)
        {
            foreach(Room r in rooms)
            {
                r.hideRoom();
            }

            foreach(Tile t in objects)
            {
                t.enabled = false;
            }

            isShowing = false;
        }
    }

    //Nulls the data of this Area for garbage collection.
    public void releaseData()
    {
        if (isGenerated)
        {
            foreach(Tile t in objects)
            {
                GameObject.Destroy(t);
            }
            objects = null;

            foreach (Room r in rooms)
            {
                r.destroyRoom();
            }
            rooms = null;

            tiles = null;
            corridors = null;

            isGenerated = false;
            isShowing = false;
        }
    }

    #endregion


    #region Public methods

    //Returns the neighbors connected to this Area. (Should always have at least one)
    public Area[] getNeighbors()
    {
        Area[] temp = new Area[connections()];

        int i = 0;

        if (north && parentMap.withinMapBounds(position.up))
        {
            temp[i] = parentMap.getArea(position.up);
            i++;
        }
        if (east && parentMap.withinMapBounds(position.right))
        {
            temp[i] = parentMap.getArea(position.right);
            i++;
        }
        if (south && parentMap.withinMapBounds(position.down))
        {
            temp[i] = parentMap.getArea(position.down);
            i++;
        }
        if (west && parentMap.withinMapBounds(position.left))
        {
            temp[i] = parentMap.getArea(position.left);
            i++;
        }

        return temp;
    }

    public void setGroup(AreaGroup g)
    {
        group = g;
    }
    public AreaGroup getGroup()
    {
        return group;
    }
    public AreaType getType()
    {
        if (group == null)
        {
            return AreaType.NotAssigned;
        }
        return group.type;
    }

    #endregion


    #region Helper methods

    //Returns the number of connections this Area has to other Areas.
    private int connections()
    {
        return (north ? 1 : 0) + (east ? 1 : 0) + (south ? 1 : 0) + (west ? 1 : 0);
    }

    #endregion


    #region Debug Methods

    public void debugShowArea()
    {
        if (!isGenerated)
        {
            generateArea();
        }

        Debug.Log("Map seed is : " + parentMap.getSeed());
        Debug.Log("Area Seed is: " + areaSeed);

        GameObject parent = new GameObject();
        parent.name = "Area Debug Parent";

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i,j] != null)
                {
                    GameObject current = (GameObject) GameObject.Instantiate(LoadResources.instance.spriteHolder, new Vector3(i, j, 0), Quaternion.identity);

                    SpriteRenderer renderer = current.GetComponent<SpriteRenderer>();
                    renderer.sprite = LoadResources.instance.fourWay;

                    current.transform.parent = parent.transform;

                    if(tiles[i,j].isBorder)
                    {
                        renderer.color = Color.black;
                    }
                    else if (tiles[i,j].isTile)
                    {
                        renderer.color = Color.white;
                    }
                    else
                    {
                        renderer.color = Color.red;
                    }
                }
            }
        }
    }

    #endregion


}
