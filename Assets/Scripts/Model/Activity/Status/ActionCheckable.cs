using UnityEngine;
using System.Collections;

public class ActionCheckable : StatusCheckable {

	public ActionCheckable(IAction act) {
		action = act;
	}

	private IAction action;

	public bool isStatusMet() {
		return Status.playerStatus.recentActionOccured (action);
	}
}
