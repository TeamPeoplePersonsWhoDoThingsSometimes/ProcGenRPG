using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Chests can hold subclasses of Item
 */
public class Chest : Interactable {

	/**
	 * The number of items all chests can store
	 */
	private static readonly int SLOTS = 10;
	/**
	 * List of items in the chest. Cannot be more than SLOTS
	 */
	private List<Item> items;

	// Use this for initialization
	void Start () {
		items = new List<Item>(SLOTS);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// KARTIK do the thing!
	protected override void Interact() {

	}

	/**
	 * Adds an item to the chest and returns true if there are enough slots.
	 * returns false otherwise.
	 */
	public bool AddToChest(Item i) {
		if (items.Count < SLOTS) {
			items.Add(i);
			return true;
		} else {
			return false;
		}
	}

	/**
	 * Removes an item to the chest and returns true if item was there.
	 * returns false otherwise.
	 */
	public bool RemoveFromChest(Item i) {
		return items.Remove(i);
	}

	public List<Item> GetList() {
		return items;
	}
}
