using UnityEngine;
using System.Collections;

public class LightMace : Weapon {

	public GameObject spawnedLight;

	public override void Attack (float damage)
	{
		base.Attack (damage);
//		GameObject.Instantiate(spawnedLight, transform.position, Quaternion.identity);
	}

}
