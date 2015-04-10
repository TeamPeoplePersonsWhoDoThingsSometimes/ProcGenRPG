using UnityEngine;
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
		private List<Point> spawnLocations;

		public Step(string n, string d, Dictionary<StatusCheckable, bool> s, List<SpawnCommand> c) {
			name = n;
			description = d;
			statuses = s;
			commands = c;
			spawnLocations = new List<Point>();
		}

		public string getName() {
			return this.name;
		}

		public string getDescription() {
			return this.description;
		}

		public float getPercentComplete() {
			if(statuses.Count > 1) {
				int totalStatuses = statuses.Count;
				int tempCount = 0;
				foreach (StatusCheckable s in statuses.Keys) {
					bool test;
					statuses.TryGetValue(s, out test);
					if (test)
						tempCount++;
				}
				return ((float)tempCount/(float)totalStatuses);
			} else if (statuses.Count == 1) {
				foreach(KeyValuePair<StatusCheckable, bool> kvp in statuses) {
					if(kvp.Key.GetType().Equals(typeof(ActionCheckable))) {
						ActionCheckable ac = (ActionCheckable)kvp.Key;
						return ac.GetPercentComplete();
					} else {
						return 0;
					}
				}
			} else {
				return -1f;
			}
			return 0f;
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

			spawnLocations = new List<Point>();
		}

		public bool isStepFinished() {
			StatusCheckable[] s = new StatusCheckable[statuses.Keys.Count];
			statuses.Keys.CopyTo(s,0);
			for (int i = 0; i < s.Length; i++) {
				bool test;
				statuses.TryGetValue(s[i], out test);
				if (!test)
					return false;
				else {
					if(s[i].GetType().Equals(typeof(ActionCheckable))) {
						ActionCheckable ac = ((ActionCheckable)s[i]);
						if(spawnLocations.Count == commands.Count && ac.getRequiredAction().getActionType() == ActionType.KILL || ac.getRequiredAction().getActionType() == ActionType.PICKED_UP_OBJECT) {
							WorldMap.RemoveStarAt(spawnLocations[i].x,spawnLocations[i].y);
						}
					}
				}
			}
			return true;
		}

		public void executeCommands(AreaGroup group) {
			foreach (SpawnCommand s in commands) {
				if (s.getSpawnSpecification().Equals(SpawnAreaTypeSpecification.LOCAL)) {
					spawnLocations.Add(MasterDriver.Instance.CurrentArea.position);
					MasterDriver.Instance.CurrentArea.executeSpawnCommand(s);
				} else {
					spawnLocations.Add(group.executeSpawnCommand(s));
				}
				//TODO: Figure out a better way to do this world map stuff?
				WorldMap.AddQuest(this.description);
			}
		}

		public void updateStatusChecks(IAction action) {
			StatusCheckable[] stats = new StatusCheckable [statuses.Keys.Count];
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

		/**
		 * Sets this step with the given quest information
		 */
		public void setStepWithQuestInfomation(QuestSave saveData) {
			Dictionary<StatusCheckable, bool> pairing = new Dictionary<StatusCheckable, bool> ();
			List<StatusSave> statusCheckSaves = new List<StatusSave> ();
			statusCheckSaves.AddRange(saveData.CurrentStepDataList);
			int counter = 0;
			foreach (KeyValuePair<StatusCheckable, bool> pair in statuses) {
				pair.Key.setFromData(statusCheckSaves[counter]);
				pairing.Add(pair.Key, statusCheckSaves[counter].AlreadyMet);
				counter++;
			}

			statuses = pairing;

			List<PointProto> pointProtos = new List<PointProto> ();
			pointProtos.AddRange(saveData.CurrentStepSpawnLocationsList);
			spawnLocations.Clear ();
			foreach (PointProto p in pointProtos) {
				spawnLocations.Add (new Point(p.X, p.Y));
			}
		}

		/**
		 * Sets teh given quest save data protobuf with this step information
		 */
		public void setQuestWithStepInformation(ref QuestSave.Builder builder) {

			foreach (KeyValuePair<StatusCheckable, bool> pair in statuses) {
				StatusSave.Builder sBuilder = StatusSave.CreateBuilder();
				sBuilder.SetAlreadyMet(pair.Value);
				pair.Key.setBuilderWithData(ref sBuilder);
				builder.AddCurrentStepData(sBuilder.Build ());
      		}

			foreach (Point p in spawnLocations) {
				PointProto.Builder pBuilder = PointProto.CreateBuilder ();
				pBuilder.SetX(p.x);
				pBuilder.SetY(p.y);
				builder.AddCurrentStepSpawnLocations(pBuilder.Build ());
			}

		}
	}

	private bool initSpawns;

	/**
	 * Initialize the quest with the given steps
	 */
	public Quest(Step[] questSteps) {
		steps = questSteps;
		initSpawns = false;
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
		initSpawns = false;
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

	private bool isStarted = false;

	/**
	 * Quest name
	 */
	private string name;

	/**
	 * The type of area group to associate this quest with
	 */
	private Biome biomeType;

	/**
	 * The area group this quest takes place in
	 */
	private AreaGroup group;

	public float getPercentComplete() {
		float tempCount = 0;
		float numSteps = 0;
		for (int i = 1; i < steps.Length; i++) {
			float tempPercent = steps[i].getPercentComplete();
			if(tempPercent != -1) {
				tempCount += tempPercent;
				numSteps++;
			}
		}
		return tempCount/(float)steps.Length;
	}

	public float getCurStepPercentage() {
		if (currentStep >= steps.Length) {
			return steps [currentStep - 1].getPercentComplete ();
		} else {
			return steps [currentStep].getPercentComplete ();
		}
	}

	/**
	 * 
	 */
	public void executeInitialQuestCommand() {
		if (!initSpawns) {
			steps[0].executeCommands(MasterDriver.Instance.CurrentArea.getGroup());
			initSpawns = true;
		}
	}

	/**
	 * Starts this quest by registering the quest with the event invoker
	 * if it is not already started
	 */
	public bool startQuestIfMetByAction(IAction act) {
		if (currentStep != 0)
			return false;

		steps [0].updateStatusChecks (act);

		Debug.Log ("Check Quest: " + this.name);
		if (steps [0].isStepFinished ()) {
			register();
			group = MasterDriver.Instance.CurrentMap.findNearestBiome(MasterDriver.Instance.CurrentArea, biomeType);
			stepQuest ();
			Debug.Log ("Start Quest: " + this.name);
			this.isStarted = true;
			PlayerCanvas.updateQuestUI = true;
			return true;
		}
		return false;
	}

	public bool isQuestStarted() {
		Debug.Log("Checking if " + this.name + " has started: " + isStarted);
		return currentStep != 0;
	}

	public bool isQuestFinished() {
		return currentStep >= steps.Length;
	}

	public bool isQuestInProgress() {
		return isQuestStarted () && !isQuestFinished ();
	}

	public string getName() {
		return this.name;
	}

	public string getCurrentStepName() {
		if(this.currentStep >= this.steps.Length) {
			return this.steps[this.currentStep-1].getName();
		} else {
			return this.steps[this.currentStep].getName();
		}
	}

	public string getCurrentStepDescription() {
		if(this.currentStep < this.steps.Length) {
			return this.steps[this.currentStep].getDescription();
		} else {
			return "Complete";
		}
	}

	/**
	 * Steps and modifies the quest steps based on the action,
	 * note: may simply reference the state
	 */
	public override void onAction (IAction action)
	{
		Debug.Log ("Action registered");

		steps[currentStep].updateStatusChecks(action);

		//return unless we need to move to the next step
		if (!steps[currentStep].isStepFinished())
			return;

		stepQuest ();
	}

	private void stepQuest() {
		currentStep++;
		
		if (currentStep >= steps.Length) {
			Debug.Log("Quest Complete!");

			deregister();
			return;
		}

		steps [currentStep].executeCommands (group);
	}

	public void setQuestState(QuestSave save) {
		currentStep = save.Step;

		if (!isQuestFinished ()) {
			steps [currentStep].setStepWithQuestInfomation (save);
		} else {

		}
	}

	public QuestSave getQuestData() {
		QuestSave.Builder builder = QuestSave.CreateBuilder ();
		builder.SetName (name);
		builder.SetStep (currentStep);

		if (!isQuestFinished ()) {
			steps [currentStep].setQuestWithStepInformation (ref builder);
		}

		return builder.Build ();
	}
}
