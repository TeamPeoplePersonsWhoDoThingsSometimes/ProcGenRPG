using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public List<Item> inventory = new List<Item>();
	private Weapon activeWeapon;
	public static Transform playerPos;
	private int bytes;

	private string version = "1.0.0";
	private string name = "NotSkyrim";

	// Use this for initialization
	void Start () {
		activeWeapon = (Weapon)inventory[0];
		playerPos = transform;
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

	public void StopAttack() {
		activeWeapon.StopAttack();
	}

	public void AddBytes(int val) {
		bytes += val;
		activeWeapon.AddBytes(val);
	}

	public int GetBytes() {
		return bytes;
	}

	public string ToString() {
		return name + "_" + version +
			"\n\nWeapon: " + activeWeapon.ToString();
	}

}
