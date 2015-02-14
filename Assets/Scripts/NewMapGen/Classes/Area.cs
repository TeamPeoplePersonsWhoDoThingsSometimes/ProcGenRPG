﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Area
{

    #region Plans

    //TODO: Add booleans representing whether or not this Area has been generated, is being shown, or is
        //currently being generated by a thread. (Also have a refence to the thread, to kill it, if need be.)

    //TODO: This Area will also need references to all of the enemies and objects in this Area. To kill them.

    //TODO: Multithreaded Area generation, when the player is in an adjacent Area. To reduce load times.

    //TODO: Create a function that randomly assigns a quest to one of its Rooms.
    
    //Plans: To generate the Area, first generate the rooms, and place them into the 2d tile array.
                //Then, genereate a 2d array of int values, which will function as weights.
                //While all rooms are not connected.
                    //Use an A* algorithm to create corridors from Room to Room.
    //Once all rooms are connected, generate decorations/enemies/Quest material in each room.

    //Worse Idea: I was thinking that we could repurpose the mapGenerator class to an AreaGenerator class, so that separate AreaTypes
        //can use their own AreaGenerator, to make unique Area shapes.
        //The base AreaGenerator class would have a static method which starts the generation of an Area, but the 
            //protected methods of the class would do the actual generation.
        //These protected methods would then be overridden in derrived AreaGenerators, so they will generate a unique
            //shape, or else use the default generator, if the functions aren't overriden.
        //Then, the generateArea() method of this class will contain a switch statement (with the switch being the areaType),
            //deciding which AreaGenerator to use to generate this Area.
        //NOTE: We will need to get both a Room List, and a 2D tileArray back from the mapGenerator.

    //BETTER Idea: Rather than making a bunch of AreaGenerators, why not make one AreaGenerator struct, which ONLY contains
        //functions for generating different types of Areas? This would probably be better, since we never actually need an 
        //AreaGenerator object, we only need the output from the function.
            //THIS way, we could also mix and match certain types of Generating functions, i.e. say we want a "skinny
                //Room generator" function that makes skinny rooms, and a "fat path generator", which makes wide corridors?
                //We could mix and match them as much as we want to get whatever Area style we want.

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
    
    //The Map which this Area is a part of.
    private Map parentMap;

    private List<Room> rooms; //The rooms which this Area contains.
    private List<TileData> corridors;
    private TileData[,] tiles;

    private List<GameObject> objects;

    private int areaSeed;

    private bool isGenerated = false;
    private bool isShowing = false;

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
        //TODO: Implement this method.

        //Don't do anything, if this area is already showing.
        if (!isShowing)
        {
            //Ensure that this area has been generated. If not, do the generation now.
            if (!isGenerated)
            {
                generateArea();
            }

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {

                }
            }


            //Should take the 2D array generated by the generateArea method, create gameObjects out of them,
            //and display them to the player.

            //Probably should keep a list of these gameObjects, so that they may be destroyed.

            //Will have a switch statement determining which tileset to use for the gameObjects.
        }

    }

    //Disables the GameObjects for this Area.
    public void hideArea()
    {
        //Should (Destroy or hide? Make a separate function for destroying?) all gameObjects 
        //that were generated by this Area.
    }

    //Nulls the data of this Area for garbage collection.
    public void releaseData()
    {
        //Should null the 2D arrays of information, so that they may be garbage collected.
        //Prevents running out of RAM.
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
