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

	public const string saveGameFile1 = "./save1.sav";
	public const string saveGameFile2 = "./save2.sav";
	public const string saveGameFile3 = "./save3.sav";

	/*********************************
	 * Public instance data, should be used for settings only
	*********************************/
	
	/*
	 * When we move all the builders to external applications, we will get rid of this, but for we need this to provide data
	 * to the quest builder about possible actions
	 */
	public bool generateBuilderDataOnLaunch;

	public bool saveGame;
	public bool loadGame;
	
	public TileSet[] tileSets;
	public GameObject[] weapons;
	public GameObject[] hacks;

	public GameObject player;

	private QuestListener questListener;
	private QuestStarClearer clearer;

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

		if (name == "Byte") {
			return Utility.GetByteObject();
		}

		foreach (GameObject o in weapons) {
			Item i = o.GetComponent<Item>();
			if (i.gameObject.name.Equals(name)) {
				return o;
			}
		}
		
		foreach (GameObject o in hacks) {
			Item i = o.GetComponent<Item>();
			if (i.gameObject.name.Equals(name)) {
				return o;
			}
		}
		
		return null;
	}
	
	public Object getEnemyFromProtobuf(DirectObjectProtocol proto) {
		string name = proto.Name;
		Debug.Log("Finding enemy: " + name);
		foreach (TileSet t in tileSets) {
			foreach (Enemy o in t.enemyTypes) {
				if (o.gameObject.name.Equals(name)) {
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

		builder.AddWeapons (new DirectObject ("Byte", "Byte").getDirectObjectAsProtobuf ());

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

	public QuestListener MasterQuestListener() {
		return questListener;
	}

	// Use this for initialization
	void Start ()
    {

		if (generateBuilderDataOnLaunch) {
			generateBuilderData();
		}

		bool atCity = false;
		if (loadGame && PersistentInfo.saveFile != 0) {
			load (PersistentInfo.saveFile);
			loadGame = false;
		} else {
			questListener = new QuestListener ();

			currentMap = new Map ();
			currentArea = currentMap.getArea (4, 4);
            try
            {
                currentArea.getGroup().generateAreas();
                Point spawnPoint = currentArea.defaultSpawn;

                player.transform.position = new Vector3(spawnPoint.x, player.transform.position.y, spawnPoint.y);
            }
            catch (System.Exception e)
            {
                //nah...
				atCity = true;
				Debug.Log("IM AT THE CITY");
            }
			Debug.Log("IM AT " + player.transform.position);
			currentMap.debugDisplayMap();
			questListener.initializeQuests ();
		}

    	currentArea.showArea();

		if(atCity) {
			int i = 0;
			Tile currentPortal;
			do
			{
				currentPortal = currentArea.portals[i];
				i++;
			} while (currentPortal.gameObject.GetComponent<Portal>().dir != Direction.DOWN);
			player.transform.GetChild(0).transform.localPosition = Vector3.zero;
			player.transform.GetChild(1).transform.localPosition = Vector3.zero;
			player.transform.position = new Vector3(currentPortal.transform.position.x, player.transform.position.y, currentPortal.transform.position.z - 8);
		}

		clearer = new QuestStarClearer ();
		new BossStarter ();

        Debug.Log("Startup time: " + Time.realtimeSinceStartup);
	}

	void Update()
	{
		if (saveGame && PersistentInfo.saveFile != 0) {
			saveGame = false;
			save(PersistentInfo.saveFile);
		}
	}

	public void goToCity() {
		currentArea.releaseData();
		FollowPlayer.Travel();
		currentArea = currentMap.getArea(4,4);
		currentArea.showArea();
		
		//Get the portal we're coming out of.
		int i = 0;
		Tile currentPortal;
		
		do
		{
			currentPortal = currentArea.portals[i];
			i++;
		} while (currentPortal.gameObject.GetComponent<Portal>().dir != Direction.DOWN);
		
		player.transform.GetChild(0).transform.localPosition = Vector3.zero;
		player.transform.GetChild(1).transform.localPosition = Vector3.zero;
		

		player.transform.position = new Vector3(currentPortal.transform.position.x, player.transform.position.y, currentPortal.transform.position.z - 8);
		
		
		FMOD_StudioSystem.instance.PlayOneShot("event:/environment/portal",Player.playerPos.position, PlayerPrefs.GetFloat("MasterVolume"));
		
		//Fire Movement Event
		DirectObject obj = new DirectObject ("Area", currentArea.position.x + " " + currentArea.position.y);
		PlayerAction action = new PlayerAction (obj, ActionType.MOVE_AREA);
		ActionEventInvoker.primaryInvoker.invokeAction (action);
		
		if(PersistentInfo.saveFile == 0) {
			save(PersistentInfo.saveFile);
		}
	}

    //Changes the current Area to the Area in the input Direction.
    public void moveArea(Direction dir)
    {
        currentArea.releaseData();
        Point temp;

		FollowPlayer.Travel();

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

		//currentArea.getGroup ().generateAreas ();
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

		FMOD_StudioSystem.instance.PlayOneShot("event:/environment/portal",Player.playerPos.position, PlayerPrefs.GetFloat("MasterVolume"));

		//Fire Movement Event
		DirectObject obj = new DirectObject ("Area", currentArea.position.x + " " + currentArea.position.y);
		PlayerAction action = new PlayerAction (obj, ActionType.MOVE_AREA);
		ActionEventInvoker.primaryInvoker.invokeAction (action);

		if(PersistentInfo.saveFile == 0) {
			save(PersistentInfo.saveFile);
		}
    }
	

    //TODO: Create startNewGame and loadGame methods.

	public void save(int file) {
		if(file == 0) {
			return;
		}
		PlayerStatus playerData = player.GetComponentInChildren<Player> ().getPlayerStatus ();

		List<QuestSave> questData = questListener.getQuestData ();
		List<SpawnedObject> spawnedObjects = currentMap.getAllSpawnedObjects ();
		int seed = currentMap.getSeed ();

		SavePackage.Builder packageBuilder = SavePackage.CreateBuilder ();
		packageBuilder.SetPlayer (playerData);
		packageBuilder.AddRangeQuestData (questData);
		packageBuilder.AddRangeSpawnedObjects (spawnedObjects);
		packageBuilder.SetSeed (seed);

		SavePackage package = packageBuilder.Build ();

		try {
			FileStream fs;
			if(file == 1) {
				PlayerPrefs.SetString("Load1",player.GetComponentInChildren<Player>().GetName());
				fs = new FileStream (saveGameFile1, FileMode.Create);
			} else if(file == 2) {
				PlayerPrefs.SetString("Load2",player.GetComponentInChildren<Player>().name);
				fs = new FileStream (saveGameFile2, FileMode.Create);
			} else {
				PlayerPrefs.SetString("Load3",player.GetComponentInChildren<Player>().name);
				fs = new FileStream (saveGameFile3, FileMode.Create);
			}
			package.WriteTo(fs);
			fs.Flush();
			fs.Close();
		} catch (IOException excep) {
			log("IO ERROR: COULD NOT SAVE GAME DATA");
		}
	}

	public void load(int file) {
		Debug.Log("LOADING : " + file);
		SavePackage package;

		try {
			FileStream fs;
			if(file == 1) {
				fs = new FileStream (saveGameFile1, FileMode.Open);
			} else if(file == 2) {
				fs = new FileStream (saveGameFile2, FileMode.Open);
			} else {
				fs = new FileStream (saveGameFile3, FileMode.Open);
			}
			package = SavePackage.ParseFrom(fs);
			fs.Flush();
			fs.Close();
		} catch (IOException excep) {
			IOException dummy = excep;
			log("IO ERROR: COULD NOT LOAD GAME DATA");
			return;
		}

		//WORLD GENERATION
		currentMap = new Map (package.Seed);

		//QUESTS
		List<QuestSave> questSaves = new List<QuestSave> ();
		questSaves.AddRange(package.QuestDataList);
		questListener = new QuestListener (questSaves);

		//PLAYER STATUS
		player.transform.position = new Vector3 (0.0f, player.transform.position.y, 0.0f);//global shift
		currentArea = currentMap.getArea (package.Player.PlayerPosition.AreaX, package.Player.PlayerPosition.AreaY);
		player.GetComponentInChildren<Player> ().setPlayerStatus (package.Player);
		currentArea.getGroup ().generateAreas ();

		Transform camera = getCamera ();

		camera.position = new Vector3 (package.Player.PlayerPosition.LocalX, camera.position.y, package.Player.PlayerPosition.LocalY);
		Quaternion rotation = camera.rotation;
		rotation.eulerAngles = new Vector3 (0.0f, package.Player.Rotation, 0.0f);
		camera.rotation = rotation;

		//SPAWNED OBJECTS
		List<SpawnedObject> spawnedObjects = new List<SpawnedObject> ();
		spawnedObjects.AddRange(package.SpawnedObjectsList);
		foreach (SpawnedObject o in spawnedObjects) {
			GlobalPosition pos = o.ObjectPosition;
			Area area = currentMap.getArea(pos.AreaX, pos.AreaY);
			GameObject obj;

			WorldMap.AddStarAt(pos.AreaX, pos.AreaY, o.Description);

			if (o.HasEnemyAttributes) {
				obj = (GameObject)getEnemyFromProtobuf(o.ObjectData);
				obj = (GameObject)GameObject.Instantiate(obj);
				obj.GetComponent<Enemy>().name = o.ObjectData.Name;
				//TODO set enemy health
			} else {
				obj = (GameObject)getItemFromProtobuf(o.ObjectData);
				ItemDropObject drop = LoadResources.Instance.CommonItemDrop.GetComponent<ItemDropObject>();
				GameObject newDrop = (GameObject)GameObject.Instantiate (drop.gameObject);
				GameObject newObject = (GameObject)GameObject.Instantiate (obj);
				newObject.SetActive(false);
				newObject.GetComponent<Item>().name = o.ObjectData.Type;

				if (o.ObjectData.HasItemInformation && o.ObjectData.ItemInformation.HasSaveVersion && newObject.GetComponent<Weapon>() != null) {
					newObject.GetComponent<Weapon>().version = o.ObjectData.ItemInformation.SaveVersion;
				}

				newDrop.GetComponent<ItemDropObject>().item = newObject;
				obj = newDrop;
			}

			obj.transform.position = new Vector3 (pos.LocalX, obj.transform.position.y, pos.LocalY);

			if(area.basicRoom != null) {
				area.basicRoom.addSpawnedObject(obj);
			}

			if (area != currentArea) {
				obj.SetActive(false);
			}
		}

	}

	public Transform getCamera() {
		//best way I could find to get children in unity?
		Transform[] playerChildren = player.GetComponentsInChildren<Transform> ();
		Transform camera = null;
		foreach (Transform t in playerChildren) {
			if (t.gameObject.name.Equals("CamRotate")) {
				camera = t;
			}
		}
		return camera;
	}
}
