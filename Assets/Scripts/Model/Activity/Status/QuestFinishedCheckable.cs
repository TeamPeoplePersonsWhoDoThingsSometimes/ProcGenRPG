using UnityEngine;
using System.Collections;

public class QuestFinishedCheckable : StatusCheckable {
	
	public QuestFinishedCheckable() {
		checkQuest = "";
		quantity = 1;
		not = false;
	}
	
	public QuestFinishedCheckable(string questName) {
		checkQuest = questName;
		quantity = 1;
		not = false;
	}
	
	private string checkQuest;
	
	private int quantity;
	private bool not;
	
	public bool isStatusMet(IAction action) {
		bool positiveMet = isStatusMetInterior (action);
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}
	
	private bool isStatusMetInterior(IAction action) {
		return MasterDriver.Instance.MasterQuestListener ().getIsQuestFinished (checkQuest);
	}
	
	public string getRequiredQuest() {
		return checkQuest;
	}
	
	public float GetPercentComplete() {
		return ((float)(isStatusMet() ? 1 : 0)/(float)quantity);
	}
	
	public bool isStatusMet() {
		bool positiveMet = isStatusMetInterior ();
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}
	
	private bool isStatusMetInterior() {
		return MasterDriver.Instance.MasterQuestListener ().getIsQuestFinished (checkQuest);
	}
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		if (protocol.HasAmount) {
			quantity = protocol.Amount;
		} else {
			quantity = 1;
		}
		
		if (protocol.HasNot) {
			not = protocol.Not;
		} else {
			not = false;
		}
		
		checkQuest = protocol.Quest.Name;
		
		Debug.Log ("Built quest finished checkable: " + checkQuest);
	}
	
	public void setFromData (StatusSave saveData) {}
	
	public void setBuilderWithData(ref StatusSave.Builder saveData) {
		saveData.SetCount (0);
	}

	public void setActive() {
	}
	
}