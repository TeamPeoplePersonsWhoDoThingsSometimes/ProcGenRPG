using UnityEngine;
using System.Collections;

public class LevelCheckable : StatusCheckable {
	
	public LevelCheckable() {
		checkLevel = 0;
	}
	
	public LevelCheckable(int level) {
		checkLevel = level;
	}
	
	private int checkLevel;
	
	public bool isStatusMet(IAction action) {
		if (action.getActionType ().Equals (ActionType.LEVEL_UP)) {
			string newVersion = action.getDirectObject().getTypeIdentifier();
			int level = Player.getMiddle(newVersion) * 10 + Player.getMinor(newVersion);
			if (level >= checkLevel) {
				return true;
			}
		}
		return false;
	}
	
	public bool isStatusMet() {
		if (checkLevel <= Status.playerStatus.getLevel())
			return true;
		return false;
	}
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		checkLevel = protocol.Level.Level;
		
		Debug.Log ("Built Level checkable: " + checkLevel);
	}
}