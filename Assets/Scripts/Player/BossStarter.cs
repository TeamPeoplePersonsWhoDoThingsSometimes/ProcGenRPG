using UnityEngine;
using System.Collections;

public class BossStarter : ActionEventListener {

	public BossStarter() {
		register ();
	}

	public override void onAction(IAction action) {
		if (action.getActionType().Equals(ActionType.USE_ITEM)) {
			if (action.getDirectObject().getIdentifier() == "rossumVanTossumSyntAxe" && action.getDirectObject().getTypeIdentifier() == "The Key") {
				MasterDriver.Instance.save(4);
				Application.LoadLevel("FinalBossTest");
				MasterDriver.loadPlayerObjectData = true;
				MasterDriver.bossLevel = true;
			}
		}
	}
}
