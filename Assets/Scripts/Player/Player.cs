using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F;

	public List<Item> inventory = new List<Item>();
	private Weapon activeWeapon;
	public static Transform playerPos;
	private int bytes;
	private int xpBytes;
	private int bytesToNextVersion;

	private int levelUpSpeedScale = 10000;

	private string version = "1.0.0";
	private string name = "TheKiniMan";


	public static int strength, defense, efficiency, encryption, security;
	public static int algorithmPoints;
	private float integrity, rma;

	// Use this for initialization
	void Start () {
		activeWeapon = (Weapon)inventory[0];
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
	}
	
	// Update is called once per frame
	void Update () {
		playerPos = transform;
	}

	public void Attack (int combo) {
		GameObject temp = (GameObject)Instantiate(activeWeapon.GetAttack(), transform.position + new Vector3(0,1f,0), transform.localRotation);
		temp.GetComponent<Attack>().SetDamage(strength + (activeWeapon.GetDamage() * combo));
		temp.GetComponent<Attack>().SetCrit(activeWeapon.GetCrit());
//		temp.GetComponent<SwordSlash>().thisDamage = 200;
//		Debug.Log(temp.GetComponent<SwordSlash>().thisDamage + " " + temp.name);
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
		activeWeapon.StopAttack();
	}

	public void AddBytes(int val) {
		bytes += val;
		xpBytes += val;
		activeWeapon.AddBytes(val);
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

	public string ToString() {
		return name + "_" + version +
			"\nStrength: " + strength +
			"\nDefense: " + defense +
			"\nEfficiency: " + efficiency +
			"\nSecurity: " + security +
			"\nEncryption: " + encryption +
			"\n\nWeapon: " + activeWeapon.ToString();
	}

}
