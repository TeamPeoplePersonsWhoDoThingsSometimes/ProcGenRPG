using UnityEngine;
using System.Collections;

public enum WeaponType {
	Melee,
	Bow,
	Gun
}

public class Weapon : Item {

	protected Attack attack;
	public GameObject attackOBJ;
	public string version = "1.0.0";
	public float critChance;
	public float levelUpScale;
	public float levelUpSpeedScale;
	public float damage;
	public float knockback;
	public float attackSpeed;
	protected float thisDamage;
	protected float thisKnockback;
	protected WeaponType weaponType;

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
		bytesToLevelUp = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*(int)(levelUpSpeedScale*10000);
		while (bytes > bytesToLevelUp) {
			LevelUp();
		}
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
		bytesToLevelUp = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*(int)(levelUpSpeedScale*10000);
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
		return name + "_" + version;
	}

	public void AddBytes(int val) {
		bytes += val;
	}

	public WeaponType Type() {
		return weaponType;
	}

	public string InfoString() {
		string forreturn = "Type: " + Type() +
				"\nRarity: " + this.RarityVal +
				"\nBase Damage: " + thisDamage.ToString("F2") +
				"\nKnockback: " + knockback.ToString("F2") +
				"\nCrit Chance: " + critChance.ToString("F2");

		if(GetAttack().GetComponent<Attack>().attackEffect != Effect.None) {

			forreturn += "\nEffect: " + GetAttack().GetComponent<Attack>().attackEffect;

			if(GetAttack().GetComponent<Attack>().attackEffect == Effect.Deteriorating) {
				forreturn += " - " + GetAttack().GetComponent<Attack>().attackEffectChance*100f + "% chance of " + 
					GetAttack().GetComponent<Attack>().attackEffectValue + " damage for " +
						GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
			} else {
				forreturn += " - for " + GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
			}
		}

		return forreturn;
	}
}
