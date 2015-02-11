using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F, rollKey = KeyCode.Space;

	public List<Item> inventory = new List<Item>();
	public GameObject[] playerArmor; //0: head, 1: chest, 2: arms, 3: legs
	public List<Item> quickAccessItems = new List<Item>();
	private Weapon activeWeapon;
	private Hack activeHack;
	private GameObject weaponRef;
	private GameObject playerInventoryRef;
	public static Transform playerPos;
	private int bytes;
	private int xpBytes;
	private int bytesToNextVersion;

	private int levelUpSpeedScale = 10000;

	public static string version = "1.0.0";
	private string name = "TheKiniMan";
	
	public static int strength, defense, efficiency, encryption, security;
	public static int algorithmPoints;
	private float integrity, rma, maxIntegrity = 100f, maxrma = 20f;

	private static GameObject hitInfo;

	void Start () {
		//Need to figure out a better way to load the hitinfo prefab
		hitInfo = Resources.Load<GameObject>("Info/HitInfo");

		//setting initial integrity and rma values
		integrity = maxIntegrity;
		rma = maxrma;

		//initializing the references to the player inventory, armor points, and weaponhand
		playerInventoryRef = GameObject.Find("PlayerInventory");
		weaponRef = GameObject.Find("PlayerWeaponObj");
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;

		//setting up inventory
		inventory = new List<Item>();
		inventory.Add(weaponRef.transform.GetChild(0).GetComponent<Item>());
		for (int i = 0; i < playerInventoryRef.transform.childCount; i++) {
			inventory.Add(playerInventoryRef.transform.GetChild(i).GetComponent<Item>());
		}

		//setting up playerarmor
		playerArmor = new GameObject[4];

		//setting up initial weapon and hack (not the best way to do this since
		//it requires that the first item in the inventory prefab needs to be a hack
		activeWeapon = (Weapon)inventory[0];
		activeHack = (Hack)inventory[1];

		//sets up quickaccessitems and makes the canvas update the inventory ui
		quickAccessItems = new List<Item>(inventory);
		PlayerCanvas.UpdateInventory();
	}

	void Update () {
		playerPos = transform;
		rma += Time.deltaTime/2f * (encryption + 1);
		integrity += Time.deltaTime/2f * (security + 1);
		if (rma > maxrma) {
			rma = maxrma;
		} else if (rma < 0) {
			rma = 0;
		}

		if(integrity > maxIntegrity) {
			integrity = maxIntegrity;
		} else if (integrity < 0) {
			integrity = 0;
		}

		maxrma = (encryption/2f + 1)*20f;
		maxIntegrity = (security/5f + 1)*100f;	
	}

	public void Attack (int combo) {
		activeWeapon.Attack(strength + (activeWeapon.GetDamage() * combo));
	}

	//Used to control gun shooting animation
	public bool CanAttack() {
		return activeWeapon.CanAttack();
	}

	//Called whenever the player presses a number to quick select
	public void SetActiveItem (int val) {
		if(quickAccessItems.Count >= val + 1) {
			if(quickAccessItems[val].GetType().IsSubclassOf(typeof(Weapon))) { 
				activeWeapon = (Weapon)quickAccessItems[val];
				for(int i = 0; i < playerInventoryRef.transform.childCount; i++) {
					if(playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>() != null
					   && playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>().Equals(activeWeapon)) {
						if (weaponRef.transform.childCount > 0) {
							weaponRef.transform.GetChild(0).gameObject.SetActive(false);
							weaponRef.transform.GetChild(0).transform.parent = playerInventoryRef.transform;
						}
						playerInventoryRef.transform.GetChild(i).parent = weaponRef.transform;
						weaponRef.transform.GetChild(0).gameObject.SetActive(true);
						weaponRef.transform.GetChild(0).localPosition = Vector3.zero;
						weaponRef.transform.GetChild(0).localEulerAngles = Vector3.zero;
						weaponRef.transform.GetChild(0).localScale = new Vector3(1,1,1);
					}
				}
			} else {
				activeHack = (Hack)quickAccessItems[val];
			}
		}
	}

	public void Hack () {
		if(activeHack != null) {
			activeHack.Call(this);
		}
	}

	public float GetRMAPercentage() {
		return rma/maxrma;
	}

	public float GetIntegrityPercentage() {
		return integrity/maxIntegrity;
	}

	//used to start weaponfx (the green trail on the lightstick)
	public void StartAttack() {
		activeWeapon.StartAttack();
	}

	public string GetName() {
		return name + "_" + version;
	}

	public float XPPercentage() {
		return (float)xpBytes/bytesToNextVersion;
	}

	public void StopAttack() {
		if(activeWeapon != null) {
			activeWeapon.StopAttack();
		}
	}

	//Called any time the player gets bytes
	public void AddBytes(int val) {
		bytes += val;
		xpBytes += val;
		if (activeWeapon != null) {
			activeWeapon.AddBytes(val);
		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		while (xpBytes >= bytesToNextVersion) {
			LevelUp();
		}
	}

	private void LevelUp() {
		xpBytes -= bytesToNextVersion;
		//INCREASE PLAYER STATS
		algorithmPoints += 2;
		if(int.Parse(version.Split('.')[2]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
		} else if(int.Parse(version.Split('.')[1]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1])*1) + 1) + ".0";
		} else {
			version = (int.Parse(version.Split('.')[0])*1 + 1) + ".0.0";
		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
	}

	public int GetBytes() {
		return bytes;
	}

	public int GetXPBytes() {
		return xpBytes;
	}

	public Weapon GetWeapon() {
		return activeWeapon;
	}

	public Hack GetHack() {
		return activeHack;
	}

	//Called by any attack that hits the player
	public void GetDamaged(float damage, bool crit) {
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
		temp.GetComponent<TextMesh>().renderer.material.color = Color.red;
		if (crit) {
			integrity -= damage*2;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.black;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
		} else {
			integrity -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
	}

	public string ToString() {
		return name + "_" + version +
			"\nStrength: " + strength +
			"\nDefense: " + defense +
			"\nEfficiency: " + efficiency +
			"\nSecurity: " + security +
			"\nEncryption: " + encryption;
	}

	public bool ExpendRMA(float amount) {
		if(rma - amount >= 0) {
			rma -= amount;
			return true;
		} else {
			return false;
		}
	}

	public void PickUpItem(GameObject item) {
		GameObject temp = (GameObject) Instantiate(item, Vector3.zero, Quaternion.identity);
		inventory.Add(temp.GetComponent<Item>());
		temp.transform.parent = playerInventoryRef.transform;
		if(quickAccessItems.Count < 10) {
			quickAccessItems.Add(temp.GetComponent<Item>());
		}
		temp.SetActive(false);
		PlayerCanvas.UpdateInventory();

		if(item.GetComponent<Armor>() != null) {
			EquipArmor(temp.GetComponent<Armor>());
		}
	}

	public void EquipArmor(Armor armor) {
		
	}

}
