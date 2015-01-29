using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F, rollKey = KeyCode.Space;

	public List<Item> inventory = new List<Item>();
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

	// Use this for initialization
	void Start () {
		hitInfo = Resources.Load<GameObject>("Info/HitInfo");

		integrity = maxIntegrity;
		rma = maxrma;

//		GameObject temp = (GameObject)Instantiate (inventory [0].gameObject);
//		activeWeapon = temp.GetComponent<Weapon> ();
//		
//		temp = (GameObject)Instantiate (inventory [1].gameObject);
//		activeHack = temp.GetComponent<Hack> ();



		playerInventoryRef = GameObject.Find("PlayerInventory");
		weaponRef = GameObject.Find("PlayerWeaponObj");
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;

		inventory = new List<Item>();
		inventory.Add(weaponRef.transform.GetChild(0).GetComponent<Item>());

		for (int i = 0; i < playerInventoryRef.transform.childCount; i++) {
			inventory.Add(playerInventoryRef.transform.GetChild(i).GetComponent<Item>());
		}

		activeWeapon = (Weapon)inventory[0];
		activeHack = (Hack)inventory[1];

		quickAccessItems = new List<Item>(inventory);
		PlayerCanvas.UpdateInventory();
//		activeWeapon.gameObject.transform.parent = weaponRef.transform;
//		activeWeapon.gameObject.transform.localPosition = Vector3.zero;
//		activeWeapon.gameObject.transform.localEulerAngles = Vector3.zero;
//		activeWeapon.gameObject.transform.localScale = Vector3.one;
	}
	
	// Update is called once per frame
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
//		if (Time.timeScale == 0f) {
//			Debug.Log(meleeTimeFreeze);
//			meleeTimeFreeze -= 0.5f;
//		} else if(Time.timeScale < 1) {
//			Time.timeScale = 1f;
//			meleeTimeFreeze = 1f;
//		}
//
//		if(meleeTimeFreeze <= 0) {
//			Time.timeScale = 0.001f;
//		

	}

	public void Attack (int combo) {
		activeWeapon.Attack(strength + (activeWeapon.GetDamage() * combo));
	}

	public bool CanAttack() {
		return activeWeapon.CanAttack();
	}

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
	}

}
