using UnityEngine;
using System.Collections.Generic;

public enum Rarity {Common, Uncommon, Rare, Anomaly};

public class Item : MonoBehaviour {

	public int value;
	public string name;
	public string description;
	public Sprite icon;

	public Rarity RarityVal;

	public DirectObject getDirectObject() {
		return new DirectObject (this.gameObject.name, name);
	}

	public SpawnedObject getSpawnedObjectInformation(Area area, Transform location) {
		SpawnedObject.Builder builder = SpawnedObject.CreateBuilder ();

		GlobalPosition.Builder pBuilder = GlobalPosition.CreateBuilder ();
		pBuilder.SetAreaX (area.position.x);
		pBuilder.SetAreaY (area.position.y);
		pBuilder.SetLocalX ((int)location.position.x);
		pBuilder.SetLocalY ((int)location.position.z);
		builder.SetObjectPosition (pBuilder.Build ());

		builder.SetObjectData (getDirectObject().getDirectObjectAsProtobuf());

		return builder.Build ();
	}

	public virtual string InfoString() {
		return name + " (" + Utility.ByteToString(value) + ")\n" + description;
	}

}
