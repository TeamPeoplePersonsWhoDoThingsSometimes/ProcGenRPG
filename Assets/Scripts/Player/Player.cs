using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F;

	public List<Item> inventory = new List<Item>();
	private List<Item> quickAccessItems = new List<Item>();
	private Weapon activeWeapon;
	private Hack activeHack;
	private GameObject weaponRef;
	private GameObject playerInventoryRef;
	public static Transform playerPos;
	private int bytes;
	private int xpBytes;
	private int bytesToNextVersion;

	private int levelUpSpeedScale = 10000;

	private string version = "1.0.0";
	private string name = "TheKiniMan";
	
	public static int strength, defense, efficiency, encryption, security;
	public static int algorithmPoints;
	private float integrity, rma, maxIntegrity = 20f, maxrma = 20f;

	private static GameObject hitInfo;
	// Use this for initialization
	void Start () {
		hitInfo = Resources.Load<GameObject>("Info/HitInfo");

		integrity = maxIntegrity;
		rma = maxrma;

		activeWeapon = (Weapon)inventory[0];
		activeHack = (Hack)inventory[1];

		playerInventoryRef = GameObject.Find("PlayerInventory");
		weaponRef = GameObject.Find("PlayerWeaponObj");
		quickAccessItems.Add(activeWeapon);
		quickAccessItems.Add(activeHack);
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
	}
	
	// Update is called once per frame
	void Update () {
		playerPos = transform;
		rma += Time.deltaTime * (encryption + 1);
		if (rma > maxrma) {
			rma = maxrma;
		} else if (rma < 0) {
			rma = 0;
		}

		maxrma = (encryption + 1)*20f;
	}

	public void Attack (int combo) {
		GameObject temp = (GameObject)Instantiate(activeWeapon.GetAttack(), transform.position + new Vector3(0,1f,0), transform.localRotation);
		temp.GetComponent<Attack>().SetDamage(strength + (activeWeapon.GetDamage() * combo));
		temp.GetComponent<Attack>().SetCrit(activeWeapon.GetCrit());
	}

	public void SetActiveItem (int val) {
		if(inventory[val].GetType().IsSubclassOf(typeof(Weapon))) { 
			activeWeapon = (Weapon)inventory[val];
			for(int i = 0; i < playerInventoryRef.transform.childCount; i++) {
				if(playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>() != null
				   && playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>().GetName().Equals(activeWeapon.GetName())) {
					if (weaponRef.transform.childCount > 0) {
						weaponRef.transform.GetChild(0).gameObject.SetActive(false);
						weaponRef.transform.GetChild(0).transform.parent = playerInventoryRef.transform;
					}
					playerInventoryRef.transform.GetChild(i).parent = weaponRef.transform;
					weaponRef.transform.GetChild(0).gameObject.SetActive(true);
					weaponRef.transform.GetChild(0).position = Vector3.zero;
					weaponRef.transform.GetChild(0).eulerAngles = Vector3.zero;
				}
			}
		} else {
			activeHack = (Hack)inventory[val];
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
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position, hitInfo.transform.rotation);
		if (crit) {
			integrity -= damage*2;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.yellow;
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

}
