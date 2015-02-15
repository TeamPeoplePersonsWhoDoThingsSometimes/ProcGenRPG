using UnityEngine;
using System.Collections;

public class SpawnCommand {

	public SpawnCommand() {

	}

	public SpawnCommand(SpawnCommandProtocol proto) {

	}

	/*
	private Area.MapType spawnArea;
	//private QuestProtobuf.SpawnAreaTypeSpecification specification;
	private int range;
	private int quantity;//not used on area spawns
	
	//following fields are singularly exclusive
	private Area areaToSpawn;
	private Item itemToSpawn;
	private Enemy enemyToSpawn;
	
	//preserve default
	public SpawnCommand() {
		spawnArea = Area.MapType.CITY;
		range = 1;
		quantity = 1;
	}
	
	public SpawnCommand(SpawnCommandProtocol proto) {
		spawnArea = Area.MapType.valueOf(proto.SpawnArea);
		range = proto.Range;
		specification = proto.SpawnSpecification;
		
		if (proto.HasArea) {
			areaToSpawn = new Area(proto.Area);
		}
		
		//TODO items not yet implemented
		/*if (proto.hasItem()) {
            itemToSpawn = new Item(proto.getItem());
        }*/
		
		/*
		if(proto.HasEnemy) {
			enemyToSpawn = new Enemy(proto.Enemy);
		}
	}
	
	public Area.MapType getSpawnArea() {
		return spawnArea;
	}
	
	/**
     * Sets the area in which to spawn
     * @param spawnArea 
     */

    /*
	public void setSpawnArea(Area.MapType spawnArea) {
		this.spawnArea = spawnArea;
	}
	
	public int getRange() {
		return range;
	}
	
	public void setRange(int range) {
		this.range = range;
	}
	
	public int getQuantity() {
		return quantity;
	}
	
	public void setQuantity(int quantity) {
		this.quantity = quantity;
	}
	
	public Area getAreaToSpawn() {
		return areaToSpawn;
	}
	
	/**
     * Sets the area which will be spawned
     * @param areaToSpawn 
     */

    /*
	public void setAreaToSpawn(Area areaToSpawn) {
		this.areaToSpawn = areaToSpawn;
		itemToSpawn = null;
		enemyToSpawn = null;
	}
	
	public Item getItemToSpawn() {
		return itemToSpawn;
	}
	
	public void setItemToSpawn(Item itemToSpawn) {
		this.itemToSpawn = itemToSpawn;
		areaToSpawn = null;
		enemyToSpawn = null;
	}
	
	public Enemy getEnemyToSpawn() {
		return enemyToSpawn;
	}
	
	public void setEnemyToSpawn(Enemy enemyToSpawn) {
		this.enemyToSpawn = enemyToSpawn;
		areaToSpawn = null;
		itemToSpawn = null;
	}
	
	public override string toString() {
		DirectObject.ObjectType type = commandType();
		if(type.Equals(DirectObject.ObjectType.AREA)) {
			return "Area Spawn";
		} else if(type.equals(DirectObject.ObjectType.ENEMY)) {
			return "Enemy Spawn";
		} else if(type.equals(DirectObject.ObjectType.ITEM)) {
			return "Item Spawn";
		}
		return "Empty Command";
	}
	
	/**
     * Gets this command's type,
     * @return the command's type int 
     */

     /*
	public DirectObject.ObjectType commandType() {
		if(areaToSpawn != null) {
			return DirectObject.ObjectType.AREA;
		} else if(enemyToSpawn != null) {
			return DirectObject.ObjectType.ENEMY;
		} else if(itemToSpawn != null) {
			return DirectObject.ObjectType.ITEM;
		}
		return DirectObject.ObjectType.AREA;
	}
	
	public QuestProtobuf.SpawnAreaTypeSpecification getSpecification() {
		return specification;
	}
	
	public void setSpecification(QuestProtobuf.SpawnAreaTypeSpecification specification) {
		this.specification = specification;
	}
	
	public SpawnCommandProtocol getSpawnCommandAsProto() {
		SpawnCommandProtocol.Builder builder = SpawnCommandProtocol.newBuilder();
		
		builder.setRange(range);
		builder.setSpawnSpecification(specification);
		if (commandType().equals(DirectObject.ObjectType.AREA))
			builder.setArea(areaToSpawn.getDirectObjectAsProtobuf());
		else if (commandType().equals(DirectObject.ObjectType.ENEMY))
			builder.setEnemy(enemyToSpawn.getDirectObjectAsProtobuf());
		
		builder.setSpawnArea(spawnArea.toString());
		return builder.build();
	}
	*/
}
