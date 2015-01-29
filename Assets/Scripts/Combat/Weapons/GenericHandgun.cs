using UnityEngine;
using System.Collections;

public class GenericHandgun : Weapon {

	public override void Attack (float damage)
	{
		RaycastHit hitInfo;
		if(Physics.Raycast(new Ray(Player.playerPos.position + Player.playerPos.forward + new Vector3(0,1,0),Player.playerPos.forward), out hitInfo)) {
			if(hitInfo.collider.gameObject.GetComponent<Enemy>() != null) {
				Enemy temp = hitInfo.collider.gameObject.GetComponent<Enemy>();
				temp.GetDamaged(damage, Random.value < critChance);
				temp.DoKnockback(hitInfo.point, knockback);

				if(attackOBJ != null) {
					if(Random.value < attackOBJ.GetComponent<Attack>().attackEffectChance) {
						temp.GetDamaged(attackOBJ.GetComponent<Attack>().attackEffect,
						                attackOBJ.GetComponent<Attack>().attackEffectValue,
						                attackOBJ.GetComponent<Attack>().attackEffectTime);
					}
				}
			}
		}

	}

}
