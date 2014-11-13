using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class MapGenerator {

	private static readonly int MaxLength = 1000;
	//private static List<System.Type> Generators;
	//private static string generatorNameString;
	private Area a;

	/**
	 * Okay, this is fucking awesome, check this out
	 * We load the current assembly, i.e. all the classes in our unity project,
	 * and then iterate through and store any class that subclasses MapGenerator
	 * to get all the generators.  We can then get any generator that we have
	 * made simply by using the name of the class a a string!  Best of all its
	 * simple code which is easy to maintain!! So now, to make any new generator, we simply
	 * extend, and then this stuff here makes sure we can access it easily!
	 */
	/*public static MapGenerator getNewGeneratorByName(string generatorName) {
		TileSet tiles = World.getTileSetByName(generatorName);
		string gName = generatorName.Replace(" ", "").ToLower();
	
		int count = 0;
		System.Type type = null;
		foreach(System.Type a in Generators) {
			if(a.ToString().ToLower().Equals(gName))
				type = a;
			count++;
		}

		if(type == null || tiles == null)
			throw new NoSuchMapGeneratorException(generatorName, generatorNameString);

		ConstructorInfo constructor = type.GetConstructor(new System.Type[0]);
		MapGenerator generator = (MapGenerator)constructor.Invoke(new System.Type[0]);
		generator.setTiles(tiles.tiles);
		generator.setEnemies(tiles.enemies);
		return generator;
	}*/

	/**
	 * Returns a mapGenerator of the type described by the TileSet instantiated to the given area
	 * This has no benefits to constructing the generator on your own, but handles the switching on
	 * the tileSet generator type in one centralized location, and so ought to be used nonetheless
	 * whenever instantiating with these parameters.
	 */
	public static MapGenerator getMapGenerator(Area a, TileSet tiles) {
		switch (tiles.generatorType) {
		case GeneratorTypes.OVERWORLD:
			return new GrassyPathGenerator (a, tiles);
		case GeneratorTypes.DUNGEON:
			return new DungeonGenerator(a, tiles);
		case GeneratorTypes.CITY:
			return new CityGenerator(a, tiles);
		default:
			return new GrassyPathGenerator(a,tiles);
		}
	}
	
	//static constructor, this gets all classes in the assembly that are children of MapGenerator
	/*static MapGenerator() {
		generatorNameString = "";
		System.Type type = typeof(MapGenerator);
		Generators = new List<System.Type>();
		foreach(System.Type a in type.Assembly.GetExportedTypes()) {
			if(a.IsSubclassOf(type)) {
				Generators.Add(a);
				generatorNameString += a.ToString() + ", ";
			}
		}
		if(generatorNameString.EndsWith(", "))
			generatorNameString = generatorNameString.Substring(0,generatorNameString.Length - 2);
	}*/

	
	public TileSet tileSet;

	protected List<Tile> spawnedTiles;

	protected List<Tile> spawnedWalls;

	public MapGenerator(Area a, TileSet tiles) {
		this.a = a;
		tileSet = tiles;
	}

	public void SetArea(Area a) {
		this.a = a;
	}

	/**
	 * Spawn an enemy of the given type at the given coordinates (x,y) converted to (x,z)
	 */
	protected void SpawnEnemy(int enemyType, float x, float y) {
		GameObject.Instantiate(tileSet.enemyTypes[enemyType], new Vector3(x, 0.5f, y), Quaternion.identity);
	}

	public void InitWithData(AreaData data) {
		if(data.length == -1)
			data.length = Random.Range(100, MaxLength);
		Random.seed = data.seed;
		Init(data.length);
	}

	public void Init(int length) {
		//seed = Random.seed;

		spawnedTiles = new List<Tile>();
		spawnedWalls = new List<Tile>();
		generateGround(length);
		foreach(Tile t in spawnedTiles) {
			t.Init();
		}
		generateStructures(spawnedTiles, a.HasUp(), a.HasDown(), a.HasRight(), a.HasLeft());
	}

	/**
	 * Overload in a subclass in order to generate basic terrain
	 */
	protected virtual void generateGround(int length) {}

	/**
	 * Overload in a subclass in order to generate walls
	 * and other structures which rely on having already generated
	 * terrain
	 */
	protected virtual void generateStructures(List<Tile> ground, bool up, bool down, bool right, bool left) {}

	/**
	 * Returns the current amount of spawned tiles
	 */
	protected int tileCount() {
		return spawnedTiles.Count;
	}

	/*
	 * checks to see if a tile is at the given coordinates
	 */
	protected bool TileExists(float x, float z) {
		//see if tiles are at coordinates by checking an absolute value difference of the two components
		foreach(Tile t in spawnedTiles) {
//			Debug.Log("XDiff: " + (Mathf.Abps((t.X + t.size/2) - x)) + "ZDiff: " + Mathf.Abs((t.Z + t.size/2) - z));
			if((Mathf.Abs((t.X + t.size/2) - x) < t.size/2 && Mathf.Abs((t.Z + t.size/2) - z) < t.size/2) || (t.X == x && t.Z == z)) {
				return true;
			}
		}
		//see if walls are at coordinates by the same method
		foreach(Tile t in spawnedWalls) {
			if(Mathf.Abs((t.X + t.size/2) - x) < t.size/2 && Mathf.Abs((t.Z + t.size/2) - z) < t.size/2) {
				return true;
			}
		}
		return false;
	}

	/**
	 * spawn a tile of the given type at the given coordinates
	 * 
	 * note: this method does perform a check of TileExists(x,z), and returns true or false
	 * based on this method.  Use ForceTile if you need the tile to be placed regardless of the precence
	 * of a previously placed tile.
	 */
	protected bool SpawnTile(float x, float z, int type) {
		/*foreach(Tile t in spawnedTiles) {
			t.Init();
		}*/
		foreach(Tile t in spawnedWalls) {
			t.Init();
		}
		if(!TileExists(x, z)) {
			if (tileSet.tiles[type].ground) {
				spawnedTiles.Add((Tile)GameObject.Instantiate(tileSet.tiles[type], new Vector3(x, 0, z), Quaternion.identity));
			}  else { //non-ground tiles should be spawned higher up
				spawnedWalls.Add((Tile)GameObject.Instantiate(tileSet.tiles[type], new Vector3(x, 3.4f, z), Quaternion.identity));
			}
		}  else {
			return false;	
		}
		return true;
		
	}

	/**
	 * Force spawn a tile at the given location, in other words, after you call this method,
	 * a tile will be at the given coordinates independent of anything else in the game world
	 */
	protected void ForceTile(float x, float z, int type) {
		foreach(Tile t in spawnedTiles) {
			t.Init();
		}
		foreach(Tile t in spawnedWalls) {
			t.Init();
		}

		if(type == 0) {
			spawnedTiles.Add((Tile)GameObject.Instantiate(tileSet.tiles[type], new Vector3(x, 0, z), Quaternion.identity));
		}  else if (type == 1) {
			spawnedWalls.Add((Tile)GameObject.Instantiate(tileSet.tiles[type], new Vector3(x, 3.4f, z), Quaternion.identity));
		}  else {
			GameObject[] items = GameObject.FindGameObjectsWithTag("Wall");
			for (int i = 0; i < items.Length; i++) {
				if(items[i].transform.position.x == x && items[i].transform.position.z == z) {
					GameObject.Destroy(items[i]);
				}
			}
			spawnedWalls.Add((Tile)GameObject.Instantiate(tileSet.tiles[type], new Vector3(x, 3.4f, z), Quaternion.identity));
		}
	}

	/**
	 * Clear the scene of all objects
	 */
	public void Clear() {
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
		GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
		GameObject[] items = new GameObject[walls.Length + grounds.Length + portals.Length];
		walls.CopyTo(items, 0);
		grounds.CopyTo(items, walls.Length);
		portals.CopyTo(items, walls.Length + grounds.Length);
		for (int i = 0; i < items.Length; i++) {
			GameObject.Destroy(items[i]);
		}

	}
	
	
}

public enum GeneratorTypes {
	OVERWORLD,
	CITY,
	DUNGEON
}

/**
 * Throw this exception in cases where a map generator type is requested, but the requested name type does not exist
 */
/*class NoSuchMapGeneratorException : System.Exception {
	private string invalidId;
	private string pNames;
	public override string Message{get{
			return "Invalid Generator Requested: " + invalidId + "\nAvailiable generators: " + pNames + "\nNames are NOT case sensitive";
		}}

	public NoSuchMapGeneratorException(string name, string possibleNames) : base(){
		invalidId = name;
		pNames = possibleNames;
	}
}*/
