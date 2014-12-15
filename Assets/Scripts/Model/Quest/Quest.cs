using UnityEngine;
using System.Collections.Generic;

public class Quest : ActionEventListener {

	/**
	 * Initialize the quest with the given steps
	 */
	public Quest(Dictionary<StatusCheckable, bool>[] questSteps) : base() {
		steps = questSteps;
	}

	/**
	 * List of list of status checkables for this quest
	 * external list should be stepped through in order
	 * dictionary happens unordered
	 */
	private Dictionary<StatusCheckable, bool>[] steps;

	/**
	 * The current step in the quest
	 */
	private int currentStep;

	/**
	 * Quest name
	 */
	private string name;

	/**
	 * Steps and modifies the quest steps based on the action,
	 * note: may simply reference the state
	 */
	public override void onAction (IAction action)
	{
		Debug.Log ("Action registered");
		Dictionary<StatusCheckable, bool> curr = steps [currentStep];
		StatusCheckable[] statuses = new StatusCheckable [curr.Keys.Count];
		curr.Keys.CopyTo(statuses,0);

		//go through all the current status checks
		foreach(StatusCheckable a in statuses) {
			bool done = false;
			curr.TryGetValue(a, out done);

			//if the status check is not saved as done, then
			//see if it has recently been satisfied, if it has,
			//then save that, otherwise, we may not step state,
			//so return
			if(!done) {
				if(a.isStatusMet()) {
					Debug.Log ("Action Met");
					curr.Remove(a);
					curr.Add(a, true);
					continue;
				}
				return;
			}
		}

		//all current status checks are satisfied, step quest
		currentStep++;

		if (currentStep >= steps.Length) {
			Debug.Log("Quest Complete!");
		}
	}
}
