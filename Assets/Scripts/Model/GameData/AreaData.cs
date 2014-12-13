using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

/**
 * This class is formatted to store info about maps both on disc and during
 * gameplay
 */

[Serializable()]
public class AreaData : ISerializable{

	private static uint nextUID = 0;

	private static uint getNextID() {
		return nextUID++;
	}


	//List<Tile> EditedTerrain;//I'm thinking we could use a List to store edited terrain, so we could generate 
	//the terrain and then go through and place replace edited tiles
	private uint UID;//terrain identifier, NOT preserved across runtimes
	public int seed;
	public int length;//length of -1 should be interpreted by generators as a flag to generate a random length
	public string generatorName;
	public string name;

	//called upon deserialization
	public AreaData(SerializationInfo info, StreamingContext context) {
		this.UID = getNextID();
		seed = (int)info.GetValue("Area" + UID + "seed", typeof(int));
		byte[] bname = (byte[])info.GetValue("Area" + UID + "name", typeof(byte[]));
		name = Encoding.UTF8.GetString(bname);
		byte[] bGen = (byte[])info.GetValue("Area" + UID + "gen", typeof(byte[]));
		generatorName = Encoding.UTF8.GetString(bGen);
		length = (int)info.GetValue ("Area" + UID + "length", typeof(int));
	}
	
	public AreaData(int seed, string name, string generator, int length) {
		this.seed = seed;
		this.name = name;
		this.generatorName = generator;
		this.length = length;
		this.UID = getNextID();
	}

	public AreaData(int seed, string name, string generator, int length, uint UID) {
		this.seed = seed;
		this.name = name;
		this.generatorName = generator;
		this.length = length;
		this.UID = UID;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		byte[] bname = Encoding.UTF8.GetBytes(name);
		info.AddValue("Area" + UID + "name", bname);
		info.AddValue("Area" + UID + "seed", seed);
		byte[] bGen = Encoding.UTF8.GetBytes(generatorName);
		info.AddValue("Area" + UID + "gen", bGen);
		info.AddValue ("Area" + UID + "length", length);
	}

}
