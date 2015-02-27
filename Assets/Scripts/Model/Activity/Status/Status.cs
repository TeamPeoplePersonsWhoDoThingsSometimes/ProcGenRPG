﻿using UnityEngine;
using System.Collections.Generic;

public class Status : ActionEventListener {

	public static Status playerStatus {
		get {
			//initialize the instance here if needed
			//maybe move this if neccesary
			if(instance == null) {
				instance = new Status();
			}
			return instance;
		}
	}

	/**
	 * Singleton instance for this class
	 */
	private static Status instance;

	/**
	 * Number of actions to store at once
	 */
	private const int ACTION_STORAGE = 10;
	
	/**
	 * A queue of the most recent actions, with the most distant
	 * actions on the front of the queue
	 */
	private Queue<IAction> recentActions;

	/**
	 * A dictionary mapping the player's total statistics of how many
	 * enemies he has killed by mapping the enemy type as a string to
	 * the int amount of kills
	 */
	private Dictionary<string, int> enemiesKilled;

	/**
	 * A string list of the names of the areas the player has visited
	 */
	private List<string> visitedAreas;

	/**
	 * The player's current tier
	 */
	private int tier;

	/**
	 * Privatize the constructor to revoke access per the singelton
	 * implementation
	 */
	private Status() {
		recentActions = new Queue<IAction> ();
		enemiesKilled = new Dictionary<string, int> ();
		visitedAreas = new List<string> ();
	}

	/**
	 * Handles processing of actions into modifying the player's
	 * status
	 */
	public override void onAction(IAction action) {
		Debug.Log ("Register action: " + action.getActionType () + " on " + action.getDirectObject ().getIdentifier () + " of type " + action.getDirectObject ().getTypeIdentifier ());
		recentActions.Enqueue (action);
		if (recentActions.Count > ACTION_STORAGE) {
			recentActions.Dequeue();
		}
	}

	/**
	 * Checks to see if the player has visited the area of the given name
	 */
	private bool visitedArea(string name) {
		return visitedAreas.Contains (name);
	}

	/**
	 * Gets the amount of kills for the given string name of an enemy type
	 */
	public int getKills(string enemy) {
		int kills = 0;
		enemiesKilled.TryGetValue (enemy, out kills);
		return kills;
	}

	/**
	 * Returns true if the given action occured recently
	 */
	public bool recentActionOccured(IAction action) {
		for (int i = 0; i < recentActions.Count; i++) {
			IAction recentAction = recentActions.Dequeue();
			if(recentAction.getActionType().Equals(action.getActionType()) &&
			   recentAction.getDirectObject().getIdentifier().Equals(action.getDirectObject().getIdentifier()) &&
			   recentAction.getDirectObject().getTypeIdentifier().Equals(action.getDirectObject().getTypeIdentifier()))
				return true;

		}
		return false;
	}
}
