using UnityEngine;
using System.Collections;

public class QuestStarClearer : ActionEventListener {

	public QuestStarClearer() {
		register ();
	}
	
	public override void onAction(IAction action) {
		Debug.Log ("Quest Star: " + MasterDriver.Instance.CurrentArea.getSpawnedObjects ().Count);
		if (MasterDriver.Instance.CurrentArea.getSpawnedObjects ().Count <= 1) {
			Debug.Log ("Remove star: " + MasterDriver.Instance.CurrentArea.position.x + "," + MasterDriver.Instance.CurrentArea.position.y);
			WorldMap.RemoveStarAt(MasterDriver.Instance.CurrentArea.position.x, MasterDriver.Instance.CurrentArea.position.y);
		}
	}
}
