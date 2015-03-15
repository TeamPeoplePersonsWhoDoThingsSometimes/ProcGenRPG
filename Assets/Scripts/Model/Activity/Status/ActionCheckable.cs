﻿using UnityEngine;
using System.Collections;

public class ActionCheckable : StatusCheckable {

	public ActionCheckable() {
		checkAction = null;
		quantity = 1;
		currentAmount = 0;
	}
	
	public ActionCheckable(IAction act) {
		checkAction = act;
		quantity = 1;
		currentAmount = 0;
	}
	
	private IAction checkAction;
	
	private int quantity;
	private int currentAmount;
	
	public bool isStatusMet(IAction action) {
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
				return true;
			}
		}

		return false;
	}

	public float GetPercentComplete() {
		return ((float)currentAmount/(float)quantity);
	}

	public bool isStatusMet() {
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
		checkAction = new PlayerAction (protocol.Action);
		currentAmount = 0;

		Debug.Log ("Built action checkable: " + checkAction.getActionType () + "," + checkAction.getDirectObject ().getIdentifier () + "," + checkAction.getDirectObject ().getTypeIdentifier () + "," + quantity);
	}
}