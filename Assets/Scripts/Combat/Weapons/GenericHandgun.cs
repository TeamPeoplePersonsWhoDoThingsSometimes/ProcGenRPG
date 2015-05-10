using UnityEngine;
using System.Collections;

public class GenericHandgun : Weapon {

	public override void Attack (float damage)
	{

		FMOD_StudioSystem.instance.PlayOneShot("event:/weapons/pistol",transform.position,PlayerPrefs.GetFloat("MasterVolume"));
		RaycastHit hitInfo;
		if(Physics.Raycast(new Ray(Player.playerPos.position + Player.playerPos.forward + new Vector3(0,1,0),Player.playerPos.forward), out hitInfo)) {
			if(hitInfo.collider.gameObject.GetComponent<Enemy>() != null) {
				Enemy temp = hitInfo.collider.gameObject.GetComponent<Enemy>();
				temp.GetDamaged(damage + (Player.strength), Random.value < critChance);
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
		Instantiate(attackOBJ,transform.position,transform.rotation);

	}

}
