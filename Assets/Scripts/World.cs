using UnityEngine;
using System.Collections;
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
	
	private static void generateNewArea(TileSet tiles) {
		Area area = new Area(tiles);
		area.setUp(new Area(tiles));
		area.Init();
		currentArea = area;
		data.SetArea(currentArea.Data);
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
		data = new DataStorage();
		TileSets = tileSets;

		//generate starting area
		if(loadData)
			load();
		else
			generateNewArea(TileSets[1]);
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
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("DownPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z + 7f);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelDown() {
		currentArea.Clear();
		currentArea = currentArea.getDown();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("UpPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z - 7f);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelRight() {
		currentArea.Clear();
		currentArea = currentArea.getRight();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("LeftPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

	public static void TravelLeft() {
		currentArea.Clear();
		currentArea = currentArea.getLeft();
		currentArea.Init();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = new Vector3(0, 0, 0);
		foreach(GameObject g in items) {
			Tile t = g.GetComponent<Tile>();
			if(t.name.Equals("RightPortal")) {
				player.transform.position = new Vector3(g.transform.position.x, 0.5f, g.transform.position.z);
				GameObject.Find("Main Camera").GetComponent<FollowPlayer>().SetToPlayer();
			}
		}
	}

}
