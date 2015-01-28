using UnityEngine;
using System.Collections;

public class LightStick : Weapon {

	private float lightSwipeTime;

	// Use this for initialization
	new void Start () {
		base.Start();
		weaponType = WeaponType.Melee;
	}
	
	// Update is called once per frame
	protected override void Update () {
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

	public override void StartAttack ()
	{
		base.StartAttack ();
	}

	public override void StopAttack ()
	{
		base.StopAttack ();
	}
}
