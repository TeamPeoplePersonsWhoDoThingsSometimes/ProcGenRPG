using UnityEngine;
using System.Collections;

public interface DirectObject {

	/**
	 * Returns a string identifier corresponding to this particular
	 * direct object.  Used in serialization
	 */
	string getIdentifier();

	/**
	 * Returns a astring identifier corresponding to the type of this
	 * direct object.  Used in serialization
	 */
	string getTypeIdentifier();
}
