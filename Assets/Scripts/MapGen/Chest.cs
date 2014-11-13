using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour {

	private static readonly int SLOTS = 4;
	private List<Item> items;

	// Use this for initialization
	void Start () {
		items = new List<Item>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool addToChest(Item i) {
		if (items.Count < SLOTS) {
			items.Add(i);
			return true;
		} else {
			return false;
		}
	}

	public bool removeFromChest(Item i) {
		return items.Remove(i);
	}

	public List<Item> getList() {
		return items;
	}
}
