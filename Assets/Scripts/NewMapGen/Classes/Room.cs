using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {

    //A rectangular space within an Area.


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
	List<GameObject> spawnedObjects;//objects that were theorhetically spawned in this room- 
									//note however that this may contain any objects spawned in this area
									//see Area.basicRoom for more details

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
				if(g != null) {
					g.SetActive(true);
				}
			}

            foreach (GameObject g in generateEnemies(random)) {
                objects.Add(g);
            }

            foreach (GameObject g in generateChests())
            {
                objects.Add(g);
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
				if(g != null) {
					g.SetActive(false);
				}
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

	public void addSpawnedObject(GameObject obj) {
		spawnedObjects.Add (obj);
	}

	public List<SpawnedObject> getSpawnedObjects(Area area) {
		List<SpawnedObject> spawnDataList = new List<SpawnedObject> ();

		foreach (GameObject obj in spawnedObjects) {
			if (obj == null) continue;//skip destroyed objects
			
			Enemy enemy = obj.GetComponent<Enemy>();
			if (enemy != null) {
				spawnDataList.Add(enemy.getSpawnedObjectInformation(area));
			}

			ItemDropObject itemDrop = obj.GetComponent<ItemDropObject>();
			if (itemDrop != null) {
				GameObject item = itemDrop.item;
				if (item != null) {
					spawnDataList.Add(item.GetComponent<Item>().getSpawnedObjectInformation(area, itemDrop.transform));
				}
			}
		}

		return spawnDataList;

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

    private GameObject[] generateEnemies(System.Random rand)
	{
        List<Enemy> types;
		List<float> typeChances;
		List<int> tempEnemyList = new List<int>();

        switch (parent.getBiome()) {
            case (Biome.C):
            case (Biome.PYTHON):
            case (Biome.HTML):
                types = LoadResources.Instance.grassyPath.GetComponent<TileSet>().enemyTypes;
				typeChances = LoadResources.Instance.grassyPath.GetComponent<TileSet>().enemyTypeChances;
                break;
            default:
                throw new System.Exception("Uninitialized Area Type!");
        }

		int numEnemies = rand.Next(2,11);
		for(int j = 0; j < typeChances.Count; j++) {
			if(Utility.ComparableVersionInt(types[j].maxVersion) >= Utility.ComparableVersionInt(Player.version)
			   && Utility.ComparableVersionInt(types[j].minVersion) <= Utility.ComparableVersionInt(Player.version)) {
				for(int i = tempEnemyList.Count; i < numEnemies || i < (int)(typeChances[j]*numEnemies); i++) {
					if(rand.NextDouble() < ((double)typeChances[j])) {
//						numEnemies++;
						tempEnemyList.Add(j);
					}
				}
//				tempEnemyList.Add(j);
			}
		}
		GameObject[] enemies = new GameObject[tempEnemyList.Count];
//		Debug.Log("NUMENEMIES: " + numEnemies);

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy temp = types[tempEnemyList[i]];
            Vector3 pos = 10 * new Vector3(rand.Next(botLeft.x, topRight.x), 0.3f, rand.Next(botLeft.y, topRight.y));
//            Debug.Log("Enemy position: " + pos);
            enemies[i] = ((Enemy) GameObject.Instantiate(temp, pos, Quaternion.identity)).gameObject;
//			Debug.Log("Spawning " + enemies[i].name);
        }

        return enemies;
    }

    private List<GameObject> generateChests()
    {
        System.Random rand = new System.Random();
        List<GameObject> lst = new List<GameObject>();

        GameObject chest = LoadResources.Instance.Chest;
        for (int i = 0; i < 3; i++)
        {
            if (rand.NextDouble() < 0.07)
            {
                Vector3 pos = 10 * new Vector3(rand.Next(botLeft.x + 1, topRight.x - 1), 0.1f, rand.Next(botLeft.y + 1, topRight.y - 1));
                GameObject temp = (GameObject)GameObject.Instantiate(chest, pos, Quaternion.identity);
                lst.Add(temp);
            }
        }

        return lst;
    }

    #endregion

}
