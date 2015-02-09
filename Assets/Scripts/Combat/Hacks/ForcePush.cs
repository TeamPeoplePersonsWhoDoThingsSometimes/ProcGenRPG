using UnityEngine;
using System.Collections;

public class ForcePush : Hack {

	protected override void OneShotActivated ()
	{
		base.OneShotActivated ();
//		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + ((attack.transform.localScale.x/2f)*Player.playerPos.right), Quaternion.LookRotation(Player.playerPos.right));
		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position, Player.playerPos.rotation);
		tempAttack.GetComponent<Attack>().SetDamage(damage);
		tempAttack.GetComponent<Attack>().SetCrit(critChance);
	}
}
