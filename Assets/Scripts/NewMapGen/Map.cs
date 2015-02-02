using UnityEngine;
using System.Collections;
using PriorityQueue;

public class Map
{

    //TODO: Make Map Expandable. (Try to avoid extending arrays. Takes too long to copy large Maps.)
        //possibly remove restriction of moving outside of a Map, and when player enters a non-existant area,
        //make a new map, and place the player on that one, yet keep the last one? (Rough idea)

    #region Variables

    //Seed for this map. (Will be used to save the map).
    private int seed;

    //The map.
    private Area[,] areaMap;

    //The point which map generation starts around.
    private Point origin;

    #endregion


    #region Constructors

    public Map(int seed = 0)
    {
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
            if (withinAreaBounds(tempPoint) && !areaData[tempPoint.x, tempPoint.y].isConnected)
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
                if (withinAreaBounds(edge.to))
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

    //Divides the areas into AreaGroups, which will have the same biome (tileset). (Dijkstra's algorithm)
    private void generateAreaGroups()
    {
        //TODO: Implement this method.
    }

    #endregion


    #region Public Methods

    //Returns the area at the input point. (Can input x and y, or a Point object)
    public Area getArea(int x, int y)
    {
        if (withinAreaBounds(x, y))
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
        for (int i = 0; i < areaData.GetLength(0); i++)
        {
            for (int j = 0; j < areaData.GetLength(1); j++)
            {
                Vertex temp = areaData[i, j];
                areaMap[i, j] = new Area(new Point(i, j), temp.N.isUsed, temp.E.isUsed, temp.S.isUsed, temp.W.isUsed);
            }
        }
    }

    //Check to see if the input point is a valid point on the map. Returns false, if not. (Can input x and y, or a Point object)
    private bool withinAreaBounds(Point p)
    {
        return (p.x > -1 && p.y > -1 && p.x < areaMap.GetLength(0) && p.y < areaMap.GetLength(1));
    }
    private bool withinAreaBounds(int x, int y)
    {
        return (x > -1 && y > -1 && x < areaMap.GetLength(0) && y < areaMap.GetLength(1));
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
