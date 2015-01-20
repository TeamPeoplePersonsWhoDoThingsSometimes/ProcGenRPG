using UnityEngine;
using System.Collections;

public class ActionCheckable : StatusCheckable {

	public ActionCheckable() {
		checkAction = null;
		quantity = 1;
		currentAmount = 0;
	}

	public ActionCheckable(Action act) {
		checkAction = act;
		quantity = 1;
		currentAmount = 0;
	}

	private Action checkAction;

	private int quantity;
	private int currentAmount;

	public bool isStatusMet(Action action) {
		if (action.getActionType ().Equals (checkAction.getActionType ()) &&
						action.getDirectObject ().getIdentifier ().Equals (checkAction.getDirectObject ().getIdentifier ()) &&
						action.getDirectObject ().getTypeIdentifier ().Equals (checkAction.getDirectObject ().getTypeIdentifier ()))
						currentAmount++;
		if (currentAmount >= quantity)
						return true;
		return false;
	}

	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		if (protocol.HasAmount) {
			quantity = protocol.Amount;
		} else {
			quantity = 1;
		}
		checkAction = new PlayerAction (protocol.Action);
		currentAmount = 0;

		Debug.Log ("Built action checkable: " + checkAction.getActionType () + "," + checkAction.getDirectObject ().getIdentifier () + "," + checkAction.getDirectObject ().getTypeIdentifier () + "," + quantity);
	}
}
