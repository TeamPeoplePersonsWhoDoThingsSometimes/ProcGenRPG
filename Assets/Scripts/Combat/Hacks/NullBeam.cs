using UnityEngine;
using System.Collections;

public class NullBeam : Hack {

	public bool shootRight = true;

	protected override void OneShotActivated ()
	{
		base.OneShotActivated ();
//		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + ((attack.transform.localScale.x/2f)*Player.playerPos.right), Quaternion.LookRotation(Player.playerPos.right));
		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + Player.playerPos.forward*attack.transform.localScale.x/1.95f + new Vector3(0f, 2f), Quaternion.LookRotation(shootRight ? Player.playerPos.right : Player.playerPos.forward));
		tempAttack.GetComponent<Attack>().SetDamage(damage);
		tempAttack.GetComponent<Attack>().SetCrit(critChance);
	}
}
