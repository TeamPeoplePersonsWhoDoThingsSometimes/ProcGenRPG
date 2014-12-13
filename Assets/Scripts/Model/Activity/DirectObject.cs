using UnityEngine;
using System.Collections;

public class DirectObject {

	private string name;
	private string type;

	public DirectObject(string id, string typeId) {
		name = id;
		type = typeId;
	}

	/**
	 * Returns a string identifier corresponding to this particular
	 * direct object.  Used in serialization
	 */
	public string getIdentifier() {
		return name;
	}

	/**
	 * Returns a astring identifier corresponding to the type of this
	 * direct object.  Used in serialization
	 */
	public string getTypeIdentifier() {
		return type;
	}
}
