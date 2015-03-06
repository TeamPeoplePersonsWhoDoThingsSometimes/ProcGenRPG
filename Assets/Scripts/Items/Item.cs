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

	public virtual string InfoString() {
		return name + " (" + Utility.ByteToString(value) + ")\n" + description;
	}

}
