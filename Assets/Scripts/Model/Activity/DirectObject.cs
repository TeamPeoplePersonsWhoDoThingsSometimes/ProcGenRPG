using UnityEngine;
using System.Collections;

public class DirectObject {

	private string name;
	private string type;

	public DirectObject(string id, string typeId) {
		name = trimCloneTag(trimCloneTag(id));//because sometime there are 2 clone tags and because last week crunchtime this is the easy fix
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

	/**
	 * This function removes any trailing (clone) tag
	 */
	private string trimCloneTag(string name) {
		string newString = name;
		if (name.ToLower().EndsWith("(clone)")) {
			newString = newString.Substring(0, newString.Length - 7);
		}
		return newString;
	}
}