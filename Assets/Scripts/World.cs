using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class World : MonoBehaviour {
	
	/*********************************
	 * Public static fields
	*********************************/
	public static DataStorage Data{get{return data;}}
	public static Area CurrentArea{get{return currentArea;}}
	public static TileSet[] TileSets;

	private static DataStorage data;
	private static Area currentArea;
	private static List<Area> notParents;



	/*********************************
	 * Public instance data, should be used for settings only
	*********************************/

	public bool saveData;
	public bool loadData;
	
	public TileSet[] tileSets;


	/*********************************
	 * Static methods
	*********************************/

	//public log
	public static void log(string log) {
		print(log);
	}

	//yaaay, we have a name generator :D
	public static string NameGenerator() {
		ASCIIEncoding ascii = new ASCIIEncoding();
		string name = "";
		int syllables = Random.Range(1,6);
		for(int i = 0; i < syllables; i++) {
			char[] chars = ascii.GetChars(new byte[] {(byte)Random.Range(97,122), (byte)Random.Range(97,122)});
			foreach(char a in chars) {
				name += a;
			}
		}
		return name;
	}

	/*public static TileSet getTileSetByName(string name) {
		string nname = name.Replace(" ", "").ToLower();
		foreach(TileSet a in TileSets) {
			if(a.generatorType.Replace(" ", "").ToLower().Equals(nname))
				return a;
		}
		return null;
	}*/

	/**
	 * direction: 0 is up, 1 is down, 2 right, 3 is left
	 */
	private static Area generateNewArea(TileSet tiles, Area from, int direction) {
		Area a = null;
		if (from == null) {
			a = new Area(tiles, null, 0, 0);
			currentArea = a;
			data.SetArea(currentArea.Data);
		} else {
			switch(direction) {
				case 0:
					a = new Area(tiles, from, from.getX(), from.getY() + 1);
					break;
				case 1:
					a = new Area(tiles, from, from.getX(), from.getY() - 1);
					break;
				case 2:
					a = new Area(tiles, from, from.getX() + 1, from.getY());
					break;
				case 3:
					a = new Area(tiles, from, from.getX() - 1, from.getY());
					break;
				default:
					Debug.LogError("invalid direction");
					break;
			}
		}
		bool added = Map.Add(a);
		if (!added) {
			a = null;
		} else {
			notParents.Add(a);
		}
		if (from != null) {
			from.SetIsParent();
			notParents.Remove(from);
		}
		return a;
	}
	
	//load area assumes that adata is already in data
	private static void LoadArea(AreaData adata) {
		Area area = new Area(adata);
		area.Init();
		currentArea = area;
	}
	
	private static void load() {
		data.Load();
		
		//code for testing/demonstration purposes
		LoadArea(data.GetArea("Landy Land"));
	}


	/*********************************
	 * Instance Methods
	 *********************************/

	// Use this for initialization
	void Start () {
		Map.Init();
		notParents = new List<Area>();
		data = new DataStorage();
		TileSets = tileSets;

		//generate starting area
		if(loadData) {
			load();
		}
		else {
			generateNewArea(TileSets[1], null, -1);
			generateAreas(TileSets[1], currentArea);
			currentArea.SetVisited();
			currentArea.Init();
			Map.PrintMap();
		}
	}

	/**
	 * Picks a random direction excluding the direction to the parent and the oposite of the given direction int
	 */
	private static int ChooseRandomDirectionFrom(Area from, int direction) {
		int wrongDir;
		switch (direction) {
			case 0:
				wrongDir = 1;
				break;
			case 1:
				wrongDir = 0;
				break;
			case 2:
				wrongDir = 3;
				break;
			case 3:
				wrongDir = 2;
				break;
			default:
				wrongDir = 0;
				Debug.LogError("wat? Impossible");
				break;

		}
		int randDir;
		do {
			randDir = Random.Range(0, 4);
		} while (randDir == from.ParentDirection() || randDir == wrongDir);
		return randDir;
	}

	/**
	 * Generates a path of areas in the given direction at most 5 spots away from the initial area
	 * The "initial" and "from" area variables are the same because "from" is updated in the recursion.
	 */
	private static void generateAreasInDirection(TileSet tiles, Area initial, Area from, int direction) {
		if (from != null && Mathf.Abs(from.getX() - initial.getX()) <= 5 && Mathf.Abs(from.getY() - initial.getY()) <= 5) {
			if (Mathf.Abs(from.getX() - initial.getX()) < 1 && Mathf.Abs(from.getY() - initial.getY()) < 1) {
				Area a = generateNewArea(tiles, from, direction);
				generateAreasInDirection(tiles, initial, a, direction);
			} else {
				int newDir = ChooseRandomDirectionFrom(from, direction);
				Area a = generateNewArea(tiles, from, newDir);
				generateAreasInDirection(tiles, initial, a, direction);
				int branchChance = Random.Range(0, 5);
				if (branchChance == 0) {
					if (direction == 0 || direction == 1) {
						if (Random.Range(0, 2) == 0) {
							direction = 2;
						} else {
							direction = 3;
						}
					} else {
						if (Random.Range(0, 2) == 0) {
							direction = 0;
						} else {
							direction = 1;
						}
					}
					a = generateNewArea(tiles, from, direction);
					generateAreasInDirection(tiles, initial, a, direction);
				}
			}
		}
	}

	/**
	 * Generates a path in every direction from the given tile
	 */
	private static void generateAreas(TileSet tiles, Area from) {
		for (int direction = 0; direction < 4; direction++) {
			Area a = generateNewArea(tiles, from, direction);
			generateAreasInDirection(tiles, a, a, direction);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(saveData) {
			Data.Save();
			saveData = !saveData;
		}

	}

	public static void TravelUp() {
		currentArea.Clear();
		currentArea = currentArea.getUp();
		Area[] edgeAreas = notParents.ToArray();
		foreach (Area area in edgeAreas) {
			if (Mathf.Abs(area.getX() - currentArea.getX()) <= 5 && Mathf.Abs(area.getY() - currentArea.getY()) <= 5) {
				generateAreas(TileSets[1], area);
			}
		}
		currentArea.SetVisited();
		Map.PrintMap();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("DownPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z + t.size / 1.5f);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelDown() {
		currentArea.Clear();
		currentArea = currentArea.getDown();
		Area[] edgeAreas = notParents.ToArray();
		foreach (Area area in edgeAreas) {
			if (Mathf.Abs(area.getX() - currentArea.getX()) <= 5 && Mathf.Abs(area.getY() - currentArea.getY()) <= 5) {
				generateAreas(TileSets[1], area);
			}
		}
		currentArea.SetVisited();
		Map.PrintMap();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("UpPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z - t.size / 1.5f);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelRight() {
		currentArea.Clear();
		currentArea = currentArea.getRight();
		Area[] edgeAreas = notParents.ToArray();
		foreach (Area area in edgeAreas) {
			if (Mathf.Abs(area.getX() - currentArea.getX()) <= 5 && Mathf.Abs(area.getY() - currentArea.getY()) <= 5) {
				generateAreas(TileSets[1], area);
			}
		}
		currentArea.SetVisited();
		Map.PrintMap();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("LeftPortal")) {
				player.transform.position = new Vector3(g.transform.position.x + t.size / 1.5f, 0.5f, g.transform.position.z);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelLeft() {
		currentArea.Clear();
		currentArea = currentArea.getLeft();
		Area[] edgeAreas = notParents.ToArray();
		foreach (Area area in edgeAreas) {
			if (Mathf.Abs(area.getX() - currentArea.getX()) <= 5 && Mathf.Abs(area.getY() - currentArea.getY()) <= 5) {
				generateAreas(TileSets[1], area);
			}
		}
		currentArea.SetVisited();
		Map.PrintMap();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("RightPortal")) {
				player.transform.position = new Vector3(g.transform.position.x - t.size / 1.5f, 0.5f, g.transform.position.z);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

}
