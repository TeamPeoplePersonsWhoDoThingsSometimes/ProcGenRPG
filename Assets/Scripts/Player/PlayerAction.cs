using UnityEngine;
using System.Collections;

public class PlayerAction : Action {

	DirectObject obj;
	ActionType action;

	public PlayerAction(DirectObject obj, ActionType action) {
		this.obj = obj;
		this.action = action;
	}

	public ActionType getActionType() {
		return action;
	}

	public DirectObject getDirectObject() {
		return obj;
	}
}
