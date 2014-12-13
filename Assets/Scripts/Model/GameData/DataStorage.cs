using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/**
 * This class facilitates the storage, saving, and loading of all gamedata
 */
public class DataStorage {
	AreaData area;

	public DataStorage() {
	}

	public void SetArea(AreaData area) {
		this.area = area;
	}

	public AreaData GetArea(string name) {
		return area;
	}

	public void Save() {
		Stream stream = File.Open("data.dat", FileMode.Create);
		BinaryFormatter formatter = new BinaryFormatter();

		formatter.Serialize(stream, area);

		stream.Close();
	}

	public void Load() {
		Stream stream = File.Open("data.dat", FileMode.OpenOrCreate);
		BinaryFormatter formatter = new BinaryFormatter();
		area = (AreaData)formatter.Deserialize(stream);
		stream.Close();
	}
}