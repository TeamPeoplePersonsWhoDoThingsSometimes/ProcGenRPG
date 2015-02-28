using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class MasterDriver : MonoBehaviour {

	/*********************************
	 * Public static fields
	*********************************/
	public static TileSet[] TileSets;
	public static GameObject[] Weapons;
	public static GameObject[] Hacks;
	
	private static QuestListener questListener;
	
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
	
	public static Object getItemFromProtobuf(DirectObjectProtocol proto) {
		string name = proto.Name;
		
		foreach (GameObject o in Weapons) {
			Item i = o.GetComponent<Item>();
			if (i.name.Equals(name)) {
				GameObject ret = o;
				Weapon w = o.GetComponent<Weapon>();
				w.version = "" + proto.ItemInformation.Version;
				return ret;
			}
		}
		
		foreach (GameObject o in Hacks) {
			Item i = o.GetComponent<Item>();
			if (i.name.Equals(name)) {
				return o;
			}
		}
		
		return null;
	}
	
	public static Object getEnemyFromProtobuf(DirectObjectProtocol proto) {
		string name = proto.Name;
		
		foreach (TileSet t in TileSets) {
			foreach (Enemy o in t.enemyTypes) {
				if (o.name.Equals(name)) {
					o.setBadass(proto.Type.Equals("Badass"));
					return o;
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


    Map currentMap;
    Area currentArea;


	// Use this for initialization
	void Start ()
    {
		if (generateBuilderDataOnLaunch) {
			generateBuilderData();
		}
		
		questListener = new QuestListener ();
		
		TileSets = tileSets;
		Weapons = weapons;
		Hacks = hacks;

        currentMap = new Map();

        currentMap.getArea(5, 5).showArea();
		player.transform.position = new Vector3(currentMap.getArea (5, 5).defaultSpawn.x, player.transform.position.y, currentMap.getArea (5, 5).defaultSpawn.y);

        //currentMap.debugDisplayMap();

        Debug.Log("Startup time: " + Time.realtimeSinceStartup);
	}
	

    //TODO: Create startNewGame and loadGame methods.
}
