using UnityEngine;
using System.Collections;
using PriorityQueue;
using System.Collections.Generic;

public class Map
{

    #region Plans

    //TODO: Make Map Expandable. (Try to avoid extending arrays. Takes too long to copy large Maps.)
        //possibly remove restriction of moving outside of a Map, and when player enters a non-existant area,
        //make a new map, and place the player on that one, yet keep the last one? (Rough idea)

    //Possible Solution:
        //Allow Areas to point outside of map, and when a player moves outside of the map, either load or generate 
        //the map they're trying to move to. Would require a data structure to hold all the Maps, and their seed values.
            //A 2-key dictionary might work well for this. The keys being x and y values, of course.
            //Probably can get 2-key dictionary from online. People usually make them and give them away.

        //**Generate the maps, first. THEN, run a method to randomly create connections on both edges of an Map.
            //This would probably be the best solution. Somewhat easy to implement and more natural looking.
                //ONLY problem, is how to save this as data? Can we seed this every time?
                    //The seed for the connections between two maps, could be the seeds of the two Maps addded together?
                    //In theory, that should work every time, since the seeds of the two maps are the same every time.
            //This will require that every Map surrounding the current map have a seed value. Simple.
                //Map class will have to be changed, so that it is only generated when a method is called, rather than
                //when it is constructed. The constructor should only give the Map object it's seed value.
                    //Need new functions, like generateMap(), and releaseData(). Much like the Area class.

        //Should a World class be created again, to hold the 2-key dictionary? It would be a nice wrapper, 
        //that also allows for loading and saving Map seeds.

        //NOTE: Area Groups will only be able to exist within one Map. They cannot spill over onto other Maps.


    //TODO: Create a function that assigns a quest to a random Area, and returns the Area's position.

    //TODO: Create a function that assigns a quest to a random AreaGroup in this Map, and returns the AreaGroup's name.

    #endregion


    #region Variables

    //Seed for this map. (Will be used to save the map).
    private int seed;

    //The map.
    private Area[,] areaMap;

    //The point which map generation starts around.
    private Point origin;

    private List<AreaGroup> areaGroups;

    #endregion


    #region Constructors

    public Map(int seed = 0)
    {
        //Instantiate Object.
        areaGroups = new List<AreaGroup>();

        //If a seed isn't given, a random seed is generated.
        if (seed == 0)
        {
            seed = (int)(Random.value * 1000000000); //One trillion different possible seed values.
        }
        this.seed = seed;

        //Set up arrays.
        areaMap = new Area[15, 15];
        origin = new Point(7, 7);

        //Do generations with a Vertex array.
        Vertex[,] areaData = new Vertex[areaMap.GetLength(0),areaMap.GetLength(1)];
        initializeVerticies(areaData);
        generateConnections(areaData);
        destroyWalls(areaData);

        //Convert Vertex array into Area Array.
        createAreaMap(areaData);

        generateAreaGroups();
        
    }

    #endregion


    #region Algorithms

    //Generates the initial connections from area to area. (PRIM'S ALGORITHM).
    private void generateConnections(Vertex[,] areaData)
    {

        System.Random random = new System.Random(seed); //I use the .NET Random Number Generator. No reason why.

        PriorityQueue<int, Edge> queue = new PriorityQueue<int, Edge>();

        //Add the starting point to the array.
        Vertex nextVertex = areaData[origin.x, origin.y];
        nextVertex.isConnected = true;

        queue.Enqueue(random.Next(), nextVertex.N);
        queue.Enqueue(random.Next(), nextVertex.E);
        queue.Enqueue(random.Next(), nextVertex.S);
        queue.Enqueue(random.Next(), nextVertex.W);


        /* PSEUDO-CODE
         * while the queue is not empty.
         *      Take the first value in the queue.
         *      If not already connected.
         *          Set Next Vertex to isConnected.
         *          Set BOTH edges to isUsed.
         *          Put new edges in the queue.
         */

        //while the queue is not empty.
        while(!queue.IsEmpty)
        {
            //Take the first value in the queue.
            Edge temp = queue.DequeueValue();
            Point tempPoint = temp.to;

            //If not already connected, and Point is a valid point on the map.
            if (withinMapBounds(tempPoint) && !areaData[tempPoint.x, tempPoint.y].isConnected)
            { 

                //Set Next Vertex to isConnected.
                nextVertex = areaData[tempPoint.x, tempPoint.y];
                nextVertex.isConnected = true;

                //Set BOTH edges to isUsed.

                //First edge.
                temp.isUsed = true;

                //Second edge.
                Point direction = temp.from - temp.to;
                if (direction.x > 0)
                {
                    nextVertex.E.isUsed = true;
                }
                else if (direction.x < 0)
                {
                    nextVertex.W.isUsed = true;
                }
                else if (direction.y > 0)
                {
                    nextVertex.N.isUsed = true;
                }
                else if (direction.y < 0)
                {
                    nextVertex.S.isUsed = true;
                }

                //Put new edges in the queue.
                if (!nextVertex.N.isUsed)
                {
                    queue.Enqueue(random.Next(), nextVertex.N);
                }
                if (!nextVertex.E.isUsed)
                {
                    queue.Enqueue(random.Next(), nextVertex.E);
                }
                if (!nextVertex.S.isUsed)
                {
                    queue.Enqueue(random.Next(), nextVertex.S);
                }
                if (!nextVertex.W.isUsed)
                {
                    queue.Enqueue(random.Next(), nextVertex.W);
                }
            }
        }
        
    }

    //Destroys a number of the walls of the maze based on the seed. This makes the maze braided, instead of perfect.
    //Higher PercOfWalls destroys more walls. Large values may cause an infinite loop. Values Less than 0.5 are generally safe.
    private void destroyWalls(Vertex[,] areaData, double PercOfWalls = 0.2)
    {
        //Number of walls to destroy. (Not an actual percentage of the walls, but is kinda like one.)
        int numOfWalls = (int) (areaData.GetLength(0) * areaData.GetLength(1) * PercOfWalls);

        System.Random random = new System.Random(seed);

        int i = 0;
        while (i < numOfWalls)
        {
            //Pick a random vertex.
            int randX = random.Next(0, areaData.GetLength(0));
            int randY = random.Next(0, areaData.GetLength(1));
            Vertex temp = areaData[randX, randY];

            //Remove a wall, if this vertex has less than 3 connections to it.
            if (temp.connections() < 4)
            {
                //Pick a random, unused edge.
                Edge[] array = temp.getUnusedEdges();
                Edge edge = array[random.Next(0, array.Length)];

                //If this is a valid connection to add.
                if (withinMapBounds(edge.to))
                {
                    //Set BOTH side to be used.

                    //First edge.
                    edge.isUsed = true;

                    //Second edge.
                    Point direction = edge.from - edge.to;
                    Vertex nextVertex = areaData[edge.to.x, edge.to.y];
                    if (direction.x > 0)
                    {
                        nextVertex.E.isUsed = true;
                    }
                    else if (direction.x < 0)
                    {
                        nextVertex.W.isUsed = true;
                    }
                    else if (direction.y > 0)
                    {
                        nextVertex.N.isUsed = true;
                    }
                    else if (direction.y < 0)
                    {
                        nextVertex.S.isUsed = true;
                    }

                    //Increment i, since a wall was destroyed.
                    i++;
                }
            }
        }

    }

    //Divides the areas into AreaGroups, which will have the same biome (tileset). (Flood-Fill algorithm)
    private void generateAreaGroups()
    {
        //Plan:
        //  Search in a floodfill pattern (Moving along paths), until no more Areas exist to flood fill.
        //  When an Area without an AreaGroup is found,
        //      Create a new AreaGroup Object, and add it to the list.
        //      Start floodfill to assign surrounding Areas to this group.

        bool isFinished = false;

        System.Random random = new System.Random(seed);

        int numOfTypes = System.Enum.GetNames(typeof(AreaType)).Length;

        int x = 0;
        int y = 0;

        Queue<Area> unset = new Queue<Area>();

        while (!isFinished)
        {
            Point bounds = getMapBounds();

            Area search = getArea(x, y);

            if (search.getType() == 0)
            {
                //Get the area.
                Area temp = search;
                int distance = random.Next(2, 4); //Get a random distance for this AreaGroup to expand.

                List<Area> array = new List<Area>();
                floodFill(null, temp, distance, array);
                
                //Don't allow tiny AreaGroups.
                if (array.Count > 3)
                {
                    //Get a random AreaType
                    AreaGroup group = new AreaGroup((AreaType)random.Next(1, numOfTypes));

                    for (int i = 0; i < array.Count; i++)
                    {
                        if (array[i].getType() == AreaType.NotAssigned)
                        {
                            group.addArea(array[i]);
                        }
                    }
                }
                else
                {
                    foreach(Area a in array)
                    {
                        unset.Enqueue(a);
                    }
                }

            }

            x++;

            if (x > bounds.x)
            {
                x = 0;
                y++;
                if (y > bounds.y)
                {
                    isFinished = true;
                }
            }
        }

        while (unset.Count > 0)
        {
            //Set this Area to the nearest AreaGroup.
            Area next = unset.Dequeue();
            if (next.getType() == AreaType.NotAssigned)
            {
                foreach (Area a in next.getNeighbors())
                {
                    if (a.getGroup() != null)
                    {
                        a.getGroup().addArea(next);
                        break;
                    }
                }
                if (next.getType() == AreaType.NotAssigned)
                {
                    unset.Enqueue(next);
                }
            }

        }

    }

    #endregion


    #region Public Methods

    //Returns the area at the input point. (Can input x and y, or a Point object)
    public Area getArea(int x, int y)
    {
        if (withinMapBounds(x, y))
        {
            return areaMap[x, y];
        }
        else
        {
            throw new System.Exception("Area requested is out of bounds!");
        }
    }
    public Area getArea(Point p)
    {
        return getArea(p.x, p.y);
    }

    //Returns the seed for this Map.
    public int getSeed()
    {
        return this.seed;
    }

    //Returns the Top-most right-most Area's position. (The max X and max Y position).
    public Point getMapBounds()
    {
        return new Point(areaMap.GetLength(0) - 1, areaMap.GetLength(1) - 1);
    }

    //Check to see if the input point is a valid point on the map. Returns false, if not. (Can input x and y, or a Point object)
    public bool withinMapBounds(Point p)
    {
        return (p.x > -1 && p.y > -1 && p.x < areaMap.GetLength(0) && p.y < areaMap.GetLength(1));
    }
    public bool withinMapBounds(int x, int y)
    {
        return (x > -1 && y > -1 && x < areaMap.GetLength(0) && y < areaMap.GetLength(1));
    }

    public void debugDisplayMap()
    {
        GameObject parent = new GameObject();
        parent.name = "Map Debug Parent";

        for(int i = 0; i < areaMap.GetLength(0); i++){
			for(int j = 0; j < areaMap.GetLength(1); j++){

				GameObject currentObject = (GameObject) GameObject.Instantiate(LoadResources.instance.spriteHolder, new Vector3(i, j, 0), Quaternion.identity);
                SpriteRenderer renderer = currentObject.GetComponent<SpriteRenderer>();

                currentObject.transform.parent = parent.transform;

				bool N = areaMap[i,j].north;
				bool E = areaMap[i,j].east;
				bool S = areaMap[i,j].south;
				bool W = areaMap[i,j].west;

				int Total = (N ? 1 : 0) + (E ? 3 : 0) + (S ? 5 : 0) + (W ? 7 : 0);

				switch (Total) {
				case 16:
                        renderer.sprite = LoadResources.instance.fourWay;
					break;
				case 8:
					if(E){
                        renderer.sprite = LoadResources.instance.corner;
					}
					else{
                        renderer.sprite = LoadResources.instance.corner;
						currentObject.transform.Rotate(new Vector3(0,0,180));
					}

					break;
				case 12:
                    renderer.sprite = LoadResources.instance.corner;
					currentObject.transform.Rotate(new Vector3(0,0,270));
					break;
				case 4:
                    renderer.sprite = LoadResources.instance.corner;
					currentObject.transform.Rotate(new Vector3(0,0,90));
					break;
				case 10:
                    renderer.sprite = LoadResources.instance.twoWay;
					currentObject.transform.Rotate(new Vector3(0,0,90));
					break;
				case 6:
                    renderer.sprite = LoadResources.instance.twoWay;
					break;
				case 9:
                    renderer.sprite = LoadResources.instance.threeWay;
					currentObject.transform.Rotate(new Vector3(0,0,90));
					break;
				case 11:
                    renderer.sprite = LoadResources.instance.threeWay;
					currentObject.transform.Rotate(new Vector3(0,0,180));
					break;
				case 13:
                    renderer.sprite = LoadResources.instance.threeWay;
					currentObject.transform.Rotate(new Vector3(0,0,270));
					break;
				case 15:
                    renderer.sprite = LoadResources.instance.threeWay;
					break;
				case 1:
                    renderer.sprite = LoadResources.instance.end;
					currentObject.transform.Rotate(new Vector3(0,0,180));
					break;
				case 3:
                    renderer.sprite = LoadResources.instance.end;
					currentObject.transform.Rotate(new Vector3(0,0,90));
					break;
				case 5:
                    renderer.sprite = LoadResources.instance.end;
					break;
				case 7:
                    renderer.sprite = LoadResources.instance.end;
					currentObject.transform.Rotate(new Vector3(0,0,270));
					break;
				default:
                    renderer.sprite = LoadResources.instance.end;
					break;
				}

                switch(areaMap[i,j].getType())
                {
                    case AreaType.Blue:
                        renderer.color = Color.blue;
                        break;
                    case AreaType.Green:
                        renderer.color = Color.green;
                        break;
                    case AreaType.Red:
                        renderer.color = Color.red;
                        break;
                    case AreaType.Yellow:
                        renderer.color = Color.yellow;
                        break;
                    case AreaType.Cyan:
                        renderer.color = Color.cyan;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #endregion


    #region Helper Methods

    //Instantiates the vertices needed for map generation.
    private void initializeVerticies(Vertex[,] areaData)
    {
        for (int i = 0; i < areaData.GetLength(0); i++)
        {
            for (int j = 0; j < areaData.GetLength(1); j++)
            {
                areaData[i,j] = new Vertex(new Point(i, j));
            }
        }
    }

    //Takes a 2d array of Verticies that have been generated into a map, and creates Areas out of them.
    private void createAreaMap(Vertex[,] areaData)
    {
        System.Random random = new System.Random(seed);

        for (int i = 0; i < areaData.GetLength(0); i++)
        {
            for (int j = 0; j < areaData.GetLength(1); j++)
            {
                Vertex temp = areaData[i, j];

                areaMap[i, j] = new Area(this, new Point(i, j), random.Next(),
                    temp.N.isUsed, temp.E.isUsed, temp.S.isUsed, temp.W.isUsed);
            }
        }
    }

    //Recursively gets all the Areas within the input distance of A, including A.
    private void floodFill(Area from, Area search, int distance, List<Area> list)
    {
        list.Add(search);

        if (distance <= 0)
        {
            return;
        }

        Area[] neighbors = search.getNeighbors();

        foreach (Area a in neighbors)
        {
            if (a.getType() == 0)
            {
                if (a != from)
                {
                    floodFill(search, a, distance - 1, list);
                }
            }
        }

    }

    #endregion


    #region Internal Classes
    //These class are only to help with the Area generation. Not useful anywhere else.

    internal class Vertex
    {
        internal bool isConnected = false;

        internal Edge N, E, S, W;

        internal Vertex(Point pos)
        {
            N = new Edge(pos, new Point(pos.x, pos.y + 1));
            E = new Edge(pos, new Point(pos.x + 1, pos.y));
            S = new Edge(pos, new Point(pos.x, pos.y - 1));
            W = new Edge(pos, new Point(pos.x - 1, pos.y));
        }

        //Returns the number of connections this Vertex has to other Vertices. From 1 to 4.
        internal int connections()
        {
            return (N.isUsed ? 1 : 0) + (E.isUsed ? 1 : 0) + (S.isUsed ? 1 : 0) + (W.isUsed ? 1 : 0);
        }

        //Returns the Edges that are not connected, in an array.
        internal Edge[] getUnusedEdges()
        {
            int size = connections();
            if (size == 4)
            {
                throw new System.Exception("This Vertex has no Unused Edges!");
            }

            Edge[] edges = new Edge[4 - connections()];

            int i = 0;

            if (!N.isUsed)
            {
                edges[i] = N;
                i++;
            }
            if (!E.isUsed)
            {
                edges[i] = E;
                i++;
            }
            if (!S.isUsed)
            {
                edges[i] = S;
                i++;
            }
            if (!W.isUsed)
            {
                edges[i] = W;
                i++;
            }

            return edges;
        }

    }

    internal class Edge
    {
        internal Point from;
        internal Point to; //WARNING: EDGE MAY POINT TO AN AREA that is OUT OF ARRAY BOUNDS.

        internal bool isUsed = false;

        internal Edge(Point from, Point to)
        {
            this.from = from;
            this.to = to;
        }

        public override string ToString()
        {
            return "From: " + from.ToString() + ". To: " + to.ToString();
        }
    }

    #endregion


}
