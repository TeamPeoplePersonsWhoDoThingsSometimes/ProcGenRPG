using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public List<Item> inventory = new List<Item>();
	private Weapon activeWeapon;
	public static Transform playerPos;

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
		temp.GetComponent<Attack>().MultiplyDamage(combo);
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
}
