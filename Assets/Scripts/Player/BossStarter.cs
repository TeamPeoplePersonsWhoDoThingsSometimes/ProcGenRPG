using UnityEngine;
using System.Collections;

public class BossStarter : ActionEventListener {

	public BossStarter() {
		register ();
	}

	public override void onAction(IAction action) {
		if (action.getActionType().Equals(ActionType.USE_ITEM)) {
			if (action.getDirectObject().getIdentifier() == "rossumVanTossumSyntAxe" && action.getDirectObject().getTypeIdentifier() == "The Key") {
				Application.LoadLevel("FinalBossTest");
			}
		}
	}
}
