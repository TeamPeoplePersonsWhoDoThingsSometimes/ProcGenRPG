using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public static KeyCode forwardKey, backKey, useKey;

	public List<Item> inventory = new List<Item>();
	private Weapon activeWeapon;
	public static Transform playerPos;
	private int bytes;
	private int bytesToNextVersion;

	private int levelUpSpeedScale = 10000;

	private string version = "1.0.0";
	private string name = "NotSkyrim";

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
		temp.GetComponent<Attack>().SetDamage(activeWeapon.GetDamage() * combo);
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
		return (float)bytes/bytesToNextVersion;
	}

	public void StopAttack() {
		activeWeapon.StopAttack();
	}

	public void AddBytes(int val) {
		bytes += val;
		activeWeapon.AddBytes(val);
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		Debug.Log(XPPercentage());
		while (bytes > bytesToNextVersion) {
			Debug.Log("here");
			LevelUp();
		}
	}

	private void LevelUp() {
		bytes -= bytesToNextVersion;
		//INCREASE PLAYER STATS

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

	public string ToString() {
		return name + "_" + version +
			"\n\nWeapon: " + activeWeapon.ToString();
	}

}
