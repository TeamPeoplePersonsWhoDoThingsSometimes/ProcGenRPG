using UnityEngine;
using System.Collections;

public class TierCheckable : StatusCheckable {
	
	public TierCheckable() {
		checkTier = 0;
	}
	
	public TierCheckable(int tier) {
		checkTier = tier;
	}
	
	private int checkTier;
	
	public bool isStatusMet(IAction action) {
		if (action.getActionType ().Equals (ActionType.LEVEL_UP)) {
			string newVersion = action.getDirectObject().getTypeIdentifier();
			int level = Player.getMiddle(newVersion) * 10 + Player.getMinor(newVersion);
			if (level >= checkTier) {
				return true;
			}
		}
		return false;
	}
	
	public bool isStatusMet() {
		if (checkTier <= Status.playerStatus.getLevel())
			return true;
		return false;
	}
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		checkTier = protocol.Tier.Tier;
		
		Debug.Log ("Built Tier checkable: " + checkTier);
	}
}