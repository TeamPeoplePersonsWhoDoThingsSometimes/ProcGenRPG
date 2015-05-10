using UnityEngine;
using UnityEngine.UI;
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
	private static int minBytes = 500000;
	private static int maxBytes = 5000000;

	private bool dropItems = false;
	/**
	 * List of items in the chest. Cannot be more than SLOTS
	 */
	private List<Item> items;

	private static GameObject byteObject;
	// Use this for initialization
	void Start () {
		if(byteObject == null) {
			byteObject = Resources.Load<GameObject>("Info/Byte");
		}
		items = new List<Item>(SLOTS);
		minBytes = Utility.ComparableVersionInt(Player.version)*100;
		maxBytes = Utility.ComparableVersionInt(Player.version)*1000;
	}
	
	// Update is called once per frame
	void Update () {
		transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, Quaternion.Euler(new Vector3(transform.GetChild(0).eulerAngles.x, FollowPlayer.rotate, transform.GetChild(0).eulerAngles.z)), Time.deltaTime*50f);

		if (this.CanInteract()) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Press " + PersistentInfo.useKey + " to open";
		} else {
			transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// KARTIK do the thing!
	protected override void Interact() {
		if (PersistentInfo.useKey != KeyCode.None && Input.GetKeyDown(PersistentInfo.useKey)) {
			transform.GetChild(1).GetComponent<Animator>().SetTrigger("Open");

			int tempByteVal = Random.Range(minBytes,maxBytes);
			int curByteVal = 0;
			int byteVal = Mathf.Max(tempByteVal/5, 5000);
			while (curByteVal < tempByteVal) {
				GameObject tmp = (GameObject)Instantiate(byteObject, transform.position+Vector3.up, Quaternion.identity);
				tmp.GetComponent<Byte>().val = byteVal;
				curByteVal += byteVal;
			}

			if (Random.value<0.1f) {
				GameObject tmp = null;
				GameObject drop = null;
				if(Random.value<0.9f) {
					if(Random.value<0.5f) {
						int tempIdx = (int)(Random.value*MasterDriver.Instance.weapons.Length);
						tmp = MasterDriver.Instance.weapons[tempIdx];
						while(!tmp.GetComponent<Item>().RarityVal.Equals(Rarity.Common)) {
							if(tempIdx==MasterDriver.Instance.weapons.Length-1) {
								tempIdx = -1;
							}
							tmp = MasterDriver.Instance.weapons[++tempIdx];
						}
					} else {
						int tempIdx = (int)(Random.value*MasterDriver.Instance.weapons.Length);
						tmp = MasterDriver.Instance.weapons[tempIdx];
						while(tmp.GetComponent<Item>().RarityVal.Equals(Rarity.Common)) {
							if(tempIdx==MasterDriver.Instance.weapons.Length-1) {
								tempIdx = -1;
							}
							tmp = MasterDriver.Instance.weapons[++tempIdx];
						}
					}
				} else {
					tmp = MasterDriver.Instance.hacks[(int)(Random.value*(MasterDriver.Instance.hacks.Length-1))+1];
				}
				drop = Utility.GetItemDrop(tmp);
				GameObject.Instantiate(drop, transform.position+(Vector3.up*2f), Quaternion.identity);
			}

			Destroy(transform.GetChild(0).gameObject);
			Destroy(this);
		}
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

	/**
	 * Gets the Collections.Generic.List of items in the chest
	 */
	public List<Item> GetList() {
		return items;
	}
}
