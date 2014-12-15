using UnityEngine;
using System.Collections;

public interface IAction {

	/**
	 * Gets the type of this action
	 */
	ActionType getActionType ();

	/**
	 * Gets a string identifier 
	 */
	DirectObject getDirectObject();
}
