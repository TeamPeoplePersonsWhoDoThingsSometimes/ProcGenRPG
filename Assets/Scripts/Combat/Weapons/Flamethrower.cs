using UnityEngine;
using System.Collections;

public class Flamethrower : Weapon {
	
	public GameObject flameAttack;
	public GameObject flameParticles;
	
	public override void Attack (float damage)
	{
		GameObject.Instantiate(flameAttack, Player.playerPos.position + new Vector3(0,2,0), Player.playerPos.rotation);
		GameObject.Instantiate(flameParticles, transform.position + new Vector3(0,-.8f,0), Player.playerPos.rotation);
	}
}
