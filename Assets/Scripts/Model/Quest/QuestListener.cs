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
	private List<Quest> quests;
	private List<Quest> activeQuests;
	
	/**
	 * Default constructor, registers the quest listener and loads all the quests from the quest file
	 */
	public QuestListener() {
		register();
		quests = new List<Quest> ();
		activeQuests = new List<Quest> ();
		//first read the package from the file, then unwrap it
		System.IO.FileStream fs = new System.IO.FileStream (QuestFile, System.IO.FileMode.Open);
		QuestPackage package = QuestPackage.ParseFrom (fs);
		
		List<QuestProtocol> questProtocols = new List<QuestProtocol>();
		questProtocols.AddRange(package.QuestsList);
		quests = new List<Quest> ();
		foreach (QuestProtocol q in questProtocols) {
			quests.Add(new Quest(q));
		}
		
	}

	public void initializeQuests() {
		foreach (Quest q in quests) {
			q.executeInitialQuestCommand();
		}
	}

	public List<Quest> getActiveQuests() {
		return activeQuests;
	}
	
	// Update is called once per frame
	public override void onAction (IAction action) {
		List<Quest> started = new List<Quest> ();
		foreach (Quest q in quests) {
			if (q.startQuestIfMetByAction(action)) {
				started.Add (q);
				activeQuests.Add (q);
			}
		}
		foreach (Quest q in started) {
			quests.Remove (q);
		}
	}
}