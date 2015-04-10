using UnityEngine;
using System.Collections;

public class ActionCheckable : StatusCheckable {

	public ActionCheckable() {
		checkAction = null;
		quantity = 1;
		currentAmount = 0;
		not = false;
	}
	
	public ActionCheckable(IAction act) {
		checkAction = act;
		quantity = 1;
		currentAmount = 0;
	}
	
	private IAction checkAction;
	
	private int quantity;
	private int currentAmount;
	private bool not;

	public bool isStatusMet(IAction action) {
		bool positiveMet = isStatusMetInterior (action);
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}

	private bool isStatusMetInterior(IAction action) {
		if (action.getActionType ().Equals (checkAction.getActionType ()) &&
		    action.getDirectObject ().getIdentifier ().Equals (checkAction.getDirectObject ().getIdentifier ()) &&
		    action.getDirectObject ().getTypeIdentifier ().Equals (checkAction.getDirectObject ().getTypeIdentifier ()))
			currentAmount++;
		if (currentAmount >= quantity)
			return true;

		//Level actions require special checks
		if (checkAction.getActionType ().Equals (ActionType.LEVEL_UP) && action.getActionType().Equals(ActionType.LEVEL_UP)) {
			string newVersion = action.getDirectObject().getTypeIdentifier();
			int level = Player.getMiddle(newVersion) * 10 + Player.getMinor(newVersion);
			if (level >= int.Parse(checkAction.getDirectObject().getTypeIdentifier())) {
				currentAmount+=1;
				return true;
			}
		}

		if(checkAction.getActionType().Equals(ActionType.CONVERSATION_NODE_HIT) && action.getActionType().Equals(ActionType.CONVERSATION_NODE_HIT)) {
			if(checkAction.getDirectObject().getTypeIdentifier().Equals(action.getDirectObject().getTypeIdentifier())) {
				currentAmount+=1;
				return true;
			}
		}

		return false;
	}

	public IAction getRequiredAction() {
		return checkAction;
	}

	public float GetPercentComplete() {
		return ((float)currentAmount/(float)quantity);
	}

	public bool isStatusMet() {
		bool positiveMet = isStatusMetInterior ();
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}

	private bool isStatusMetInterior() {
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
		} else if(protocol.Action.Target.HasAmount) { //because its like this too in more places than I feel like changing
			quantity = protocol.Action.Target.Amount;
		} else {
			quantity = 1;
		}

		if (protocol.HasNot) {
			not = protocol.Not;
		} else {
			not = false;
		}

		checkAction = new PlayerAction (protocol.Action);
		currentAmount = 0;

//		Debug.Log ("Built action checkable: " + checkAction.getActionType () + "," + checkAction.getDirectObject ().getIdentifier () + "," + checkAction.getDirectObject ().getTypeIdentifier () + "," + quantity);
	}

	public void setFromData (StatusSave saveData) {
		currentAmount = saveData.Count;
	}

	public void setBuilderWithData(ref StatusSave.Builder saveData) {
		saveData.SetCount (currentAmount);
	}

}