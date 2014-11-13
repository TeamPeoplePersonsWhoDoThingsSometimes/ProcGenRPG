using UnityEngine;
using System.Collections;

/**
 * All interactable objects in the world (such that the player hits the 
 * use key to interact) should extend this class
 */
public abstract class Interactable : MonoBehaviour {

	/**
	 * true if the player is within the trigger of the interactable item
	 */
	private bool canInteract;

	/**
	 * Calls the interact method when the player hits the use key within range
	 */
	protected void OnTriggerStay(Collider other) {
		if (other.gameObject.GetComponent<Player>() != null) {
			canInteract = true;
			if (Player.useKey == null) {
				Debug.LogError("Player use key is not set");
			} else if (Input.GetKeyDown(Player.useKey)) {
				Interact();
			}
		}
	}

	/**
	 * This method will occur when the player interacts with this object.
	 * What should happen when the player hits the use key on this object?
	 */
	protected abstract void Interact();

	/**
	 * Returns true if the player is within the trigger of the interactable item
	 */
	public bool CanInteract() {
		return canInteract;
	}
}
