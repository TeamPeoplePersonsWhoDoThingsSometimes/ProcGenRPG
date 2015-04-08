using UnityEngine;
using System.Collections;

public class TierCheckable : StatusCheckable {
	
	public TierCheckable() {
		checkTier = 0;
		not = false;
	}
	
	public TierCheckable(int tier) {
		checkTier = tier;
	}
	
	private int checkTier;
	private bool not;

	public bool isStatusMet(IAction action) {
		bool positiveMet = isStatusMetInterior (action);
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}

	private bool isStatusMetInterior(IAction action) {
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
		bool positiveMet = isStatusMetInterior ();
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}

	private bool isStatusMetInterior() {
		if (checkTier <= Status.playerStatus.getLevel())
			return true;
		return false;
	}
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		checkTier = protocol.Tier.Tier;

		if (protocol.HasNot) {
			not = protocol.Not;
		} else {
			not = false;
		}

		Debug.Log ("Built Tier checkable: " + checkTier);
	}

	/**
	 * Set this status checkable with the given saveData
	 */
	public void setFromData (StatusSave saveData) {}
	
	/**
	 * Set the given save data proto with the information from this status
	 */
	public void setBuilderWithData (ref StatusSave.Builder saveData) {
		saveData.Count = 0;
	}
}