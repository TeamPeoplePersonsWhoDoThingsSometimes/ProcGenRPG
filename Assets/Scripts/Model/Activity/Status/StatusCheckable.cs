using UnityEngine;
using System.Collections;

/**
 * This interface defines a method which may be used to check to see
 * if the status currently meets a given state
 */
public interface StatusCheckable {

	/**
	 * Is the status checked by this StatusCheckable met
	 */
	bool isStatusMet (IAction action);

	bool isStatusMet ();
	
	/**
	 * Set this status checkable with the information from the given protocol
	 */
	void setFromProtocol (StatusCheckableProtocol protocol);

	/**
	 * Set this status checkable with the given saveData
	 */
	void setFromData (StatusSave saveData);

	/**
	 * Set the given save data proto with the information from this status
	 */
	void setBuilderWithData (ref StatusSave.Builder saveData);

	void setActive();
}
