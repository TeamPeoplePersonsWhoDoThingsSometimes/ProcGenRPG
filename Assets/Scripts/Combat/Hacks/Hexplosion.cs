using UnityEngine;
using System.Collections;

public class Hexplosion : Hack {

	public GameObject hexplosionparticles;

	protected override void OneShotActivated ()
	{
		base.OneShotActivated ();
		GameObject.Instantiate(hexplosionparticles, Player.playerPos.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(270f,0f,0f)));
		for(int i = 0; i < 40; i++) {
			GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(270f,360f/40f*i,0f)));
			tempAttack.GetComponent<Attack>().SetDamage(damage + (Player.strength*2));
			tempAttack.GetComponent<Attack>().SetCrit(critChance);
		}
	}
}
