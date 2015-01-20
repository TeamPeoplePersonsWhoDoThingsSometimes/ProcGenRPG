using UnityEngine;
using System.Collections.Generic;

public class Quest : ActionEventListener {

	/**
	 * Initialize the quest with the given steps
	 */
	public Quest(Dictionary<StatusCheckable, bool>[] questSteps) {
		steps = questSteps;
	}

	/**
	 * Initialize the quest with protobuf information
	 */
	public Quest(QuestProtocol quest) {
		name = quest.Name;
		currentStep = 0;
		IList<StatusStepProtocol> stepProtocols = quest.StepsList;
		StatusCheckableFactory factory = new StatusCheckableFactory ();
		steps = new Dictionary<StatusCheckable, bool>[stepProtocols.Count];
		for(int i = 0; i < stepProtocols.Count; i++) {
			Dictionary<StatusCheckable, bool> step = new Dictionary<StatusCheckable, bool>();
			IList<StatusCheckableProtocol> statusProtocols = stepProtocols[i].StatusesInStepList;
			foreach(StatusCheckableProtocol p in statusProtocols) {
				step.Add(factory.getStatusCheckableFromProtocol(p), false);
			}
			steps[i] = step;
		}
		Debug.Log ("Built quest, steps: " + this.steps.Length);
	}

	/**
	 * Starts this quest by registering the quest with the event invoker
	 * if it is not already started
	 */
	public void startQuestIfMetByAction(IAction act) {
		if (currentStep != 0)
			return;

		Debug.Log ("Check Quest: " + this.name);
		foreach(StatusCheckable s in steps[0].Keys) {
			if(s.isStatusMet(act)) {
				register ();
				Debug.Log ("Started Quest: " + this.name);
				currentStep = 1;
			}
		}
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
				if(a.isStatusMet(action)) {
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
			deregister();
		}
	}
}
