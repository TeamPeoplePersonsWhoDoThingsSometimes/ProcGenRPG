using UnityEngine;
using System.Collections;

public class Crossbow : Weapon {

	public GameObject arrowObject;

	public override void Attack (float damage)
	{
		GameObject.Instantiate(arrowObject, Player.playerPos.position + new Vector3(0,2,0), Player.playerPos.rotation);
	}
}
