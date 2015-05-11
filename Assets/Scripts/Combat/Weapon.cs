using UnityEngine;
using System.Collections;

public enum WeaponType {
	Melee,
	Bow,
	Handgun,
	MediumGun,
	Dagger
}

public class Weapon : Item {

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
	public WeaponType weaponType;

	protected bool isAttacking = false;
	protected int bytes;
	protected int bytesToLevelUp = 1000000;
	protected float attackSpeedTime = 0f;

	// Use this for initialization
	protected void Start () {
		thisDamage = damage;
		if(version.Split('.').Length != 3) {
			version = "1.0.0";
		}
		thisDamage += levelUpScale*(int.Parse(version.Split('.')[1]));
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		bytesToLevelUp = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*(int)(levelUpSpeedScale*200000));
		attackSpeedTime += Time.deltaTime;
		while (bytes > bytesToLevelUp) {
			LevelUp();
		}
	}

	public GameObject GetAttack() {
		return attackOBJ;
	}

	public virtual bool CanAttack() {
		if(attackSpeedTime > attackSpeed) {
			attackSpeedTime = 0;
			return true;
		}
		return false;
	}

	public virtual void Attack(float damage) {
		GameObject tempAttack = (GameObject)GameObject.Instantiate(attackOBJ, Player.playerPos.position + new Vector3(0,1,0), Player.playerPos.rotation);
		tempAttack.GetComponent<Attack>().SetCrit(critChance);
		tempAttack.GetComponent<Attack>().SetDamage(damage + Player.strength);
	}

	private void LevelUp() {
		//Handle Level up
		bytes -= bytesToLevelUp;
		thisDamage += levelUpScale;

		//handle changing the version string
//		if(int.Parse(version.Split('.')[2]) + 1 < 10) {
//			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
//		} else if(int.Parse(version.Split('.')[1]) + 1 < 10) {
//			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1])*1) + 1) + ".0";
//		} else {
//			version = (int.Parse(version.Split('.')[0])*1 + 1) + ".0.0";
//		}
		version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1])*1) + 1) + ".0";

		//set new bytestolevelup variable
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

	public virtual void StartAttack() {
		isAttacking = true;
	}

	public virtual void StopAttack() {
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
		version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
	}

	public WeaponType Type() {
		return weaponType;
	}

	//used for UI
	public override string InfoString() {
		string forreturn = 
				"DMG\n" + (int)thisDamage + "-" + ((int)thisDamage+1) + " (+" + Player.strength + ")" + 
				"\n\nCRIT\n" + critChance.ToString("F2") +
				"\n\nINF\n" + description;

		if(GetAttack() != null) {
			if(GetAttack().GetComponent<Attack>().attackEffect != Effect.None) {

				forreturn += "\n\nEFX: " + GetAttack().GetComponent<Attack>().attackEffect;

				if(GetAttack().GetComponent<Attack>().attackEffect == Effect.Deteriorating) {
					forreturn += " - " + GetAttack().GetComponent<Attack>().attackEffectChance*100f + "% chance of " + 
						GetAttack().GetComponent<Attack>().attackEffectValue + " damage for " +
							GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
				} else if(GetAttack().GetComponent<Attack>().attackEffect == Effect.Bugged) {
					forreturn += " - " + GetAttack().GetComponent<Attack>().attackEffectChance*100f + "% chance of confusing for " +
							GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
				} else if(GetAttack().GetComponent<Attack>().attackEffect == Effect.Virus) {
					forreturn += " - " + GetAttack().GetComponent<Attack>().attackEffectChance*100f + "% chance of infecting, which then spreads " +
						"\"Deteriorating\" for " +
						GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
				} else {
					forreturn += " - for " + GetAttack().GetComponent<Attack>().attackEffectTime + " secs";
				}
			}
		}

		return forreturn;
	}


}
