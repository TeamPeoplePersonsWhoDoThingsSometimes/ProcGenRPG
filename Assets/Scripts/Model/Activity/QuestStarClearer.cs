using UnityEngine;
using System.Collections;

public class QuestStarClearer : ActionEventListener {

	public QuestStarClearer() {
		register ();
	}
	
	public override void onAction(IAction action) {
		Debug.Log ("Area Objects: " + MasterDriver.Instance.CurrentArea.getSpawnedObjects ().Count);
		if (MasterDriver.Instance.CurrentArea.getSpawnedObjects ().Count == 0) {
			WorldMap.RemoveStarAt(MasterDriver.Instance.CurrentArea.position.x, MasterDriver.Instance.CurrentArea.position.y);
		}
	}
}
