using UnityEngine;
using System.Collections;

public class TestMelee : Weapon {				// Change "Template_Melee" to the weapon's desired name

	private float lightSwipeTime;					// Used for TrailRenderer component of GameObject (I think?)

	// Use this for initialization
	new void Start () {
		base.Start();
		isMelee = true;								// "true" for melee weapons, "false" otherwise
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();
		if(isAttacking) {
			lightSwipeTime = 0.5f;
		}
		if(lightSwipeTime > 0) {
			lightSwipeTime -= Time.deltaTime;
			GetComponent<TrailRenderer>().time = 0.5f;
		} else {
			GetComponent<TrailRenderer>().time = 0f;
		}
	}
}
