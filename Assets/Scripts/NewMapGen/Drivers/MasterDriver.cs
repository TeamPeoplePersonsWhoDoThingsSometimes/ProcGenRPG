using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class MasterDriver : MonoBehaviour {

	/*********************************
	 * Public static fields
	*********************************/
	/*public static Map CurrentMap;
	public static Area CurrentArea;

	public static TileSet[] TileSets;
	public static GameObject[] Weapons;
	public static GameObject[] Hacks;
	
	private static QuestListener questListener;*/
	
	private const string builderDataStore = "./Assets/Resources/builder.data";
	
	/*********************************
	 * Public instance data, should be used for settings only
	*********************************/
	
	/*
	 * When we move all the builders to external applications, we will get rid of this, but for we need this to provide data
	 * to the quest builder about possible actions
	 */
	public bool generateBuilderDataOnLaunch;
	
	public TileSet[] tileSets;
	public GameObject[] weapons;
	public GameObject[] hacks;

	public GameObject player;

	private QuestListener questListener;
	
	/*********************************
	 * Static methods
	*********************************/
	
	//public log
	public void log(string log) {
		print(log);
	}
	
	//yaaay, we have a name generator :D
	public string NameGenerator() {
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
	
	public Object getItemFromProtobuf(DirectObjectProtocol proto) {
		string name = proto.Name;
		
		foreach (GameObject o in weapons) {
			Item i = o.GetComponent<Item>();
			if (i.gameObject.name.Equals(name)) {
				GameObject ret = o;
				Weapon w = o.GetComponent<Weapon>();
				w.version = "" + proto.ItemInformation.Version;
				w.name = proto.Type;
				return ret;
			}
		}
		
		foreach (GameObject o in hacks) {
			Item i = o.GetComponent<Item>();
			if (i.name.Equals(name)) {
				return o;
			}
		}
		
		return null;
	}
	
	public Object getEnemyFromProtobuf(DirectObjectProtocol proto) {
		string name = proto.Name;
		
		foreach (TileSet t in tileSets) {
			foreach (Enemy o in t.enemyTypes) {
				if (o.name.Equals(name)) {
					o.setBadass(proto.Type.Equals("Badass"));
					return o.gameObject;
				}
			}
		}
		
		return null;
	}
	
	/**
	 * Generate the protobufs for the builder application with information about potential actions
	 */
	public void generateBuilderData() {
		//first grab all enemy types from the tilesets stored in global
		HashSet<Enemy> possibleEnemyTypes = new HashSet<Enemy> ();//hashet ensures distinct enemy types
		
		foreach (TileSet t in tileSets) {
			foreach (Enemy e in t.enemyTypes) {
				possibleEnemyTypes.Add(e);
			}
		}
		
		BuilderPackage.Builder builder = BuilderPackage.CreateBuilder ();
		foreach (Enemy e in possibleEnemyTypes) {
			builder.AddEnemies(e.getDirectObject().getDirectObjectAsProtobuf());
		}
		
		foreach (GameObject o in weapons) {
			builder.AddWeapons(new DirectObject(o.name, o.GetComponent<Item>().name).getDirectObjectAsProtobuf());
		}
		
		foreach (GameObject o in hacks) {
			builder.AddHacks(new DirectObject(o.name, o.GetComponent<Item>().name).getDirectObjectAsProtobuf());
		}
		
		FileStream fs = new FileStream (builderDataStore, FileMode.OpenOrCreate);
		BuilderPackage pack = builder.Build();
		pack.WriteTo(fs);
		fs.Flush();
		fs.Close();
	}

	public Map CurrentMap { get { return currentMap; } }
	public Area CurrentArea { get { return currentArea; } }

    Map currentMap;
    Area currentArea;

    //THE MASTERDRIVER IS A SINGLETON. I LIKE SINGLETONS. -Ben
    public static MasterDriver Instance;
    void Awake()
    {
        // First, check if there are any other instances conflicting.
        if (Instance != null && Instance != this)
        {
            // If so, destroy other instances.
            Destroy(this.gameObject);
        }

        //Save our singleton instance.
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

    }

	// Use this for initialization
	void Start ()
    {
		if (generateBuilderDataOnLaunch) {
			generateBuilderData();
		}
		
		questListener = new QuestListener ();
		
        currentMap = new Map();
        currentArea = currentMap.getArea(5, 5);

		currentArea.getGroup ().generateAreas ();
        currentArea.showArea();
        Point spawnPoint = currentArea.defaultSpawn;
        player.transform.position = new Vector3(spawnPoint.x, player.transform.position.y, spawnPoint.y);

        //currentMap.debugDisplayMap();

        Debug.Log("Startup time: " + Time.realtimeSinceStartup);
	}

    //Changes the current Area to the Area in the input Direction.
    public void moveArea(Direction dir)
    {
        currentArea.releaseData();
        Point temp;

        //Get next map to move to.
        switch (dir)
        {
            case (Direction.UP):
                temp = currentArea.position;
                currentArea = currentMap.getArea(temp.up);
                break;
            case (Direction.LEFT):
                temp = currentArea.position;
                currentArea = currentMap.getArea(temp.left);
                break;
            case (Direction.DOWN):
                temp = currentArea.position;
                currentArea = currentMap.getArea(temp.down);
                break;
            case (Direction.RIGHT):
                temp = currentArea.position;
                currentArea = currentMap.getArea(temp.right);
                break;
        }

		currentArea.getGroup ().generateAreas ();
        currentArea.showArea();
		log ("Moved to area: " + currentArea.position);

        //Get reversed direction.
        Direction revDir = (Direction)(((int)dir + 2) % 4);

        //Get the portal we're coming out of.
        int i = 0;
        Tile currentPortal;

        do
        {
            currentPortal = currentArea.portals[i];
            i++;
        } while (currentPortal.gameObject.GetComponent<Portal>().dir != revDir);

        player.transform.GetChild(0).transform.localPosition = Vector3.zero;
        player.transform.GetChild(1).transform.localPosition = Vector3.zero;

        switch (revDir)
        {
            case (Direction.UP):
                player.transform.position = new Vector3(currentPortal.transform.position.x, player.transform.position.y, currentPortal.transform.position.z - 8);
                break;
            case (Direction.LEFT):
                player.transform.position = new Vector3(currentPortal.transform.position.x + 8, player.transform.position.y, currentPortal.transform.position.z);
                break;
            case (Direction.DOWN):
                player.transform.position = new Vector3(currentPortal.transform.position.x, player.transform.position.y, currentPortal.transform.position.z + 8);
                break;
            case (Direction.RIGHT):
                player.transform.position = new Vector3(currentPortal.transform.position.x - 8, player.transform.position.y, currentPortal.transform.position.z);
                break;
        }
        

    }
	

    //TODO: Create startNewGame and loadGame methods.
}
