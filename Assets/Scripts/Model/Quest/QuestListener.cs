using UnityEngine;
using System.Collections.Generic;

/**
 * This class initializes and stores quests
 */
public class QuestListener : ActionEventListener {
	
	/**
	 * The default file that holds all the game's quests
	 */
	private const string QuestFile = "./Assets/Resources/out.quest";
	
	/**
	 * All the quests operated by this listener
	 */
	private Dictionary<string, Quest> quests;
	private List<Quest> activeQuests;

	/**
	 * Default constructor, registers the quest listener and loads all the quests from the quest file
	 */
	public QuestListener() {
		register();
		activeQuests = new List<Quest> ();

		loadQuests ();
	}

	public QuestListener (List<QuestSave> questSaves) {
		register ();
		activeQuests = new List<Quest> ();

		loadQuests ();

		setQuestData (questSaves);
	}

	private void loadQuests() {
		quests = new Dictionary<string, Quest> ();

		//first read the package from the file, then unwrap it
		System.IO.FileStream fs = new System.IO.FileStream (QuestFile, System.IO.FileMode.Open);
		QuestPackage package = QuestPackage.ParseFrom (fs);
		
		List<QuestProtocol> questProtocols = new List<QuestProtocol>();
		questProtocols.AddRange(package.QuestsList);
		foreach (QuestProtocol q in questProtocols) {
			quests.Add(q.Name, new Quest(q));
		}
	}
	
	public void setQuestData(List<QuestSave> questSaves) {
		foreach (QuestSave questSave in questSaves) {
			quests[questSave.Name].setQuestState(questSave);
		}

		foreach (Quest quest in quests.Values) {
			if (quest.isQuestInProgress()) {
				activeQuests.Add(quest);
			}
		}
	}

	public void initializeQuests() {
		foreach (Quest q in quests.Values) {
			q.executeInitialQuestCommand();
			if(q.isQuestInProgress()) {
				activeQuests.Add(q);
			}
		}
		PlayerCanvas.updateQuestUI = true;
	}

	public List<Quest> getActiveQuests() {
		return activeQuests;
	}
	
	// Update is called once per frame
	public override void onAction (IAction action) {
		List<Quest> started = new List<Quest> ();
		foreach (Quest q in quests.Values) {
			if (q.startQuestIfMetByAction(action)) {
				started.Add (q);
				activeQuests.Add (q);
			}
		}
		foreach (Quest q in started) {
			quests.Remove (q.getName());
		}
	}

	public List<QuestSave> getQuestData() {
		List<QuestSave> questData = new List<QuestSave> ();

		foreach (Quest q in quests.Values) {
			questData.Add (q.getQuestData());
		}

		return questData;
	}
}