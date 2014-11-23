using UnityEngine;
using System.Collections;

public class Weapon : Item {

	protected Attack attack;
	public GameObject attackOBJ;
	public string version = "1.0.0";
	public float critChance;
	public float levelUpScale;
	public int levelUpSpeedScale;
	public float damage;
	public float knockback;
	protected float thisDamage;
	protected float thisKnockback;
	protected bool isMelee;

	protected bool isAttacking = false;
	protected int bytes;
	protected int bytesToLevelUp = 1000000;

	// Use this for initialization
	protected void Start () {
		attack = attackOBJ.GetComponent<Attack>();
		attackOBJ.GetComponent<Attack>().SetCrit(critChance);
		thisDamage = damage;
	}
	
	// Update is called once per frame
	protected void Update () {
		bytesToLevelUp = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale*10000;
		while (bytes > bytesToLevelUp) {
			LevelUp();
		}
	}

	public bool IsMelee() {
		return isMelee;
	}

	public GameObject GetAttack() {
		return attackOBJ;
	}

	private void LevelUp() {
		bytes -= bytesToLevelUp;
		thisDamage += levelUpScale;
		if(int.Parse(version.Split('.')[2]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
		} else if(int.Parse(version.Split('.')[1]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1])*1) + 1) + ".0";
		} else {
			version = (int.Parse(version.Split('.')[0])*1 + 1) + ".0.0";
		}
		bytesToLevelUp = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale*10000;
	}

	public float GetCrit() {
		return critChance;
	}

	public string GetName() {
		return name + "_" + version;
	}

	public float GetVersionPercent() {
		return (float)bytes/bytesToLevelUp;
	}

	public void StartAttack() {
		isAttacking = true;
	}

	public void StopAttack() {
		isAttacking = false;
	}

	public float GetDamage() {
		return thisDamage;
	}

	public int GetBytes() {
		return bytes;
	}

	public string ToString() {
		return name + "_" + version +
			"\n   Base Damage: " + thisDamage.ToString("F2") +
				"\n   Knockback: " + knockback.ToString("F2") +
				"\n   Crit Chance: " + critChance.ToString("F2");
	}

	public void AddBytes(int val) {
		bytes += val;
	}
}
