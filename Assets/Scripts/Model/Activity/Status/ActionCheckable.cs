using UnityEngine;
using System.Collections;

public class ActionCheckable : StatusCheckable {

	public ActionCheckable(Action act) {
		action = act;
	}

	private Action action;

	public bool isStatusMet() {
		return Status.playerStatus.recentActionOccured (action);
	}
}
