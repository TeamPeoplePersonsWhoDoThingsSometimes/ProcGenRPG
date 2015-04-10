using UnityEngine;
using System.Collections;

public class NumAreasCheckable : StatusCheckable {
	
	public NumAreasCheckable() {
		areas = 0;
	}
	
	public NumAreasCheckable(int numAreas) {
		areas = numAreas;
	}
	
	private int areas;
	private bool not;
	
	public bool isStatusMet(IAction action) {
		bool positiveMet = isStatusMetInterior (action);
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}
	
	private bool isStatusMetInterior(IAction action) {
		return Status.playerStatus.getVisitedAreas ().Count >= areas;
	}
	
	public bool isStatusMet() {
		bool positiveMet = isStatusMetInterior ();
		if (not) {
			return !positiveMet;
		}
		return positiveMet;
	}
	
	private bool isStatusMetInterior() {
		return Status.playerStatus.getVisitedAreas ().Count >= areas;
	}
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	public void setFromProtocol (StatusCheckableProtocol protocol) {
		areas = protocol.NumAreas.NumAreas;
		
		if (protocol.HasNot) {
			not = protocol.Not;
		} else {
			not = false;
		}
		
//		Debug.Log ("Built Num Areas checkable: " + areas);
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

	public void setActive() {
	}
}