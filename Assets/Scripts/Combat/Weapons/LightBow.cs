using UnityEngine;
using System.Collections;

public class LightBow : Weapon {

	// Use this for initialization
	new void Start () {
		base.Start();
		weaponType = WeaponType.Bow;
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();
	}
}
