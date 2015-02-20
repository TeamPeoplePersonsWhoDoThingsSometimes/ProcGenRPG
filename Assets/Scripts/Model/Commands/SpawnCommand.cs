using UnityEngine;
using System.Collections;

public class SpawnCommand {

	private MapType spawnArea;
	private SpawnAreaTypeSpecification specification;//LOCAL is the range within the current area, DISTANCE is the area-to-area distance range
	private int range;
	private int quantity;//when area spawns were a thing, this needed to be dependant on direct objects, which is why this is initialized wierdly
	private Object objectToSpawn;

	//preserve default
	public SpawnCommand() {
		spawnArea = MapType.CITY;
		range = 1;
		quantity = 1;
	}
	
	public SpawnCommand(SpawnCommandProtocol proto) {
		spawnArea = proto.SpawnArea;
		range = proto.Range;
		specification = proto.SpawnSpecification;

		if (proto.HasItem) {
			objectToSpawn = MasterDriver.getItemFromProtobuf(proto.Item);
			quantity = proto.Item.Amount;
        }
		
		if(proto.HasEnemy) {
			objectToSpawn = MasterDriver.getEnemyFromProtobuf(proto.Enemy);
			quantity = proto.Enemy.Amount;
		}
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
}
