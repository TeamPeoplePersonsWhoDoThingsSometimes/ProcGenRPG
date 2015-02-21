﻿using UnityEngine;
using System.Collections.Generic;

public class Quest : ActionEventListener {

	/**
	 * This class helps keep steps ordered together nicely
	 */
	public class Step {
		private string name;
		private string description;
		private Dictionary<StatusCheckable, bool> statuses;
		private List<SpawnCommand> commands;

		public Step(string n, string d, Dictionary<StatusCheckable, bool> s, List<SpawnCommand> c) {
			name = n;
			description = d;
			statuses = s;
			commands = c;
		}

		public Step(StatusStepProtocol proto) {
			name = proto.Name;
			description = proto.Description;

			StatusCheckableFactory factory = new StatusCheckableFactory ();
			IList<StatusCheckableProtocol> statusProtocols = proto.StatusesInStepList;
			statuses = new Dictionary<StatusCheckable, bool>();
			foreach (StatusCheckableProtocol p in statusProtocols) {
				statuses.Add(factory.getStatusCheckableFromProtocol(p), false);
			}

			IList<SpawnCommandProtocol> commandProtocols = proto.CommandsList;
			commands = new List<SpawnCommand>();
			foreach (SpawnCommandProtocol c in commandProtocols) {
				commands.Add(new SpawnCommand(c));
			}
		}

		public bool isStepFinished() {
			foreach (StatusCheckable s in statuses.Keys) {
				bool test;
				statuses.TryGetValue(s, out test);
				if (!test)
					return false;
			}
			return true;
		}

		public bool updateStatusChecks(IAction action) {
			StatusCheckable[] stats = new StatusCheckable [curr.Keys.Count];
			statuses.Keys.CopyTo(stats,0);
			
			//go through all the current status checks
			foreach (StatusCheckable a in stats) {
				bool done = false;
				statuses.TryGetValue (a, out done);

				//if the status check is not saved as done, then
				//see if it has recently been satisfied, if it has,
				//then save that, otherwise, we may not step state,
				//so return
				if (!done) {
					if (a.isStatusMet (action)) {
						Debug.Log ("Action Met");
						statuses.Remove (a);
						statuses.Add (a, true);
					}
				}
			}
		}
	}

	/**
	 * Initialize the quest with the given steps
	 */
	public Quest(Step[] questSteps) {
		steps = questSteps;
	}

	/**
	 * Initialize the quest with protobuf information
	 */
	public Quest(QuestProtocol quest) {
		name = quest.Name;
		currentStep = 0;
		IList<StatusStepProtocol> stepProtocols = quest.StepsList;
		steps = new Step[stepProtocols.Count];
		for(int i = 0; i < stepProtocols.Count; i++) {
			steps[i] = new Step(stepProtocols[i]);
		}
		Debug.Log ("Built quest, steps: " + this.steps.Length);
	}

	/**
	 * List of list of status checkables for this quest
	 * external list should be stepped through in order
	 * dictionary happens unordered
	 */
	private Step[] steps;

	/**
	 * The current step in the quest
	 */
	private int currentStep;

	/**
	 * Quest name
	 */
	private string name;

	/**
	 * Starts this quest by registering the quest with the event invoker
	 * if it is not already started
	 */
	public void startQuestIfMetByAction(IAction act) {
		if (currentStep != 0)
			return;
		
		Debug.Log ("Check Quest: " + this.name);
		return steps[0].isStepFinished();
	}

	/**
	 * Steps and modifies the quest steps based on the action,
	 * note: may simply reference the state
	 */
	public override void onAction (IAction action)
	{
		Debug.Log ("Action registered");

		steps [currentStep].updateStatusChecks (action);

		//return unless we need to move to the next step
		if (!steps [currentStep].isStepFinished)
						return;

		//all current status checks are satisfied, step quest
		currentStep++;

		if (currentStep >= steps.Length) {
			Debug.Log("Quest Complete!");
			deregister();
		}
	}
}
