using UnityEngine;
using System.Collections;

public class Weapon : Item {

	protected Attack attack;
	public GameObject attackOBJ;
	public string version = "1.0.0";
	public float critChance;

	protected bool isAttacking = false;

	// Use this for initialization
	protected void Start () {
		base.Start();
		attack = attackOBJ.GetComponent<Attack>();
		attackOBJ.GetComponent<Attack>().SetCrit(critChance);
	}
	
	// Update is called once per frame
	protected void Update () {
		base.Update();
	}

	public GameObject GetAttack() {
		return attackOBJ;
	}

	public float GetCrit() {
		return critChance;
	}

	public void StartAttack() {
		isAttacking = true;
	}

	public void StopAttack() {
		isAttacking = false;
	}
}
