using UnityEngine;
using System.Collections;

public class Template_Melee : Weapon {				// Change "Template_Melee" to the weapon's desired name

	// Use this for initialization
	new void Start () {
		base.Start();
		weaponType = WeaponType.Melee;
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();
		if(isAttacking) {
			//Behavior for third attack
		}
	}
}
