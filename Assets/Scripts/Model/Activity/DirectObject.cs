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
	 * Constructor to build a direct object from a protobuf
	 */
	public DirectObject(DirectObjectProtocol protocol) {
		name = protocol.Name;
		type = protocol.Type;
	}

	/**
	 * Builds a direct object protobuf out of the information in this object
	 */
	public DirectObjectProtocol getDirectObjectAsProtobuf() {
		DirectObjectProtocol.Builder builder = DirectObjectProtocol.CreateBuilder ();
		builder.SetName (name);
		builder.SetType (type);
		return builder.Build ();

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
