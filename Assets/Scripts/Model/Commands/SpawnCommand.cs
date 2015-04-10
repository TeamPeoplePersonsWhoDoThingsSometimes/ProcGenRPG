using UnityEngine;
using System.Collections;

public class SpawnCommand {

	private MapType spawnArea;
	private SpawnAreaTypeSpecification specification;//LOCAL means the spawn should occur in a room in the current area
	private int range;
	private int quantity;//when area spawns were a thing, this needed to be dependant on direct objects, which is why this is initialized wierdly
	private GameObject objectToSpawn;
	private string objectName;
	private string version;
	private bool badass;

	private Point spawnedLocation;

	//preserve default
	public SpawnCommand() {
		spawnArea = MapType.CITY;
		range = 1;
		quantity = 1;
		version = "";
	}
	
	public SpawnCommand(SpawnCommandProtocol proto) {
		version = "";
		spawnArea = proto.SpawnArea;
		range = proto.Range;
		specification = proto.SpawnSpecification;

		if (proto.HasItem) {
			objectToSpawn = (GameObject)MasterDriver.Instance.getItemFromProtobuf(proto.Item);

			if (proto.Item.HasItemInformation && proto.Item.ItemInformation.HasSaveVersion) {
				version = proto.Item.ItemInformation.SaveVersion;
			}

			objectName = proto.Item.Type;
			quantity = proto.Item.Amount;
        }
		
		if(proto.HasEnemy) {
			objectToSpawn = (GameObject)MasterDriver.Instance.getEnemyFromProtobuf(proto.Enemy);
			badass = proto.Enemy.Type.Equals("Badass");
			quantity = proto.Enemy.Amount;
		}

		if (objectToSpawn == null) {
			MasterDriver.Instance.log("Could not find spawnable object for specifications: \n" + proto);
		}

	}

	public SpawnAreaTypeSpecification getSpawnSpecification() {
		return specification;
	}
	
	public MapType getSpawnArea() {
		return spawnArea;
	}
	
	public int getRange() {
		return range;
	}
	
	public int getQuantity() {
		return quantity;
	}

	/**
	 * Gets an object that may be instantiated with GameObject.Instantiate
	 */
	public Object getObjectToSpawn() {
		return objectToSpawn;
	}

	public GameObject spawnObject(Vector3 position) {
		GameObject obj = null;
		if(objectToSpawn != null) {
			Enemy newEnemy = objectToSpawn.GetComponent<Enemy> ();
			if (newEnemy != null) {
				Enemy originalEnemy = objectToSpawn.GetComponent<Enemy>();
				GameObject newObject = (GameObject)GameObject.Instantiate (objectToSpawn.gameObject, position, Quaternion.identity);
				newEnemy = newObject.GetComponent<Enemy>();
				newEnemy.setBadass(badass);
				obj = newObject;
			}

			Item newItem = objectToSpawn.GetComponent<Item> ();
			if (newItem != null) {
				ItemDropObject drop = LoadResources.Instance.CommonItemDrop.GetComponent<ItemDropObject>();
				GameObject newDrop = (GameObject)GameObject.Instantiate (drop.gameObject, position, Quaternion.identity);
				GameObject newObject = (GameObject)GameObject.Instantiate (objectToSpawn, position, Quaternion.identity);
				newObject.SetActive(false);
				newItem = newObject.GetComponent<Item>();
				newItem.name = objectName;
				if (!version.Equals(string.Empty) && newObject.GetComponent<Weapon>() != null) {
					newObject.GetComponent<Weapon>().version = version;
				}
				drop.item = newObject;
				obj = newDrop;
			}

			//Byte is a special name reserved for spawning experience bytes
			if (objectName == "Byte") {
				obj = (GameObject)GameObject.Instantiate (objectToSpawn, position, Quaternion.identity);
			}
		} else {
			Debug.LogError("Object to spawn was null!");
		}

		return obj;
	}
}
