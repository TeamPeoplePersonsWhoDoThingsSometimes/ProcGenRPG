using UnityEngine;
using System.Collections;

public class MediumGun : Weapon {

	public GameObject bulletParticles;

	private bool soundPlaying;
	private float soundTime = 0f;

	public override void Attack (float damage)
	{
		//GameObject.Instantiate(bulletParticles, transform.position + new Vector3(0,0,3), Player.playerPos.rotation);
		GameObject.Instantiate(bulletParticles, Player.playerPos.position + new Vector3(0,2.5f,0), Player.playerPos.rotation);

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
		soundTime = 0.5f;
		if(this.GetName().Contains("Shotgun")) {
			FMOD_StudioSystem.instance.PlayOneShot("event:/weapons/nullBolt",transform.position,PlayerPrefs.GetFloat("MasterVolume"));
		}

	}

	protected override void Update ()
	{
		base.Update ();
		if(this.GetName().Contains("Mini")) {
			bool prevtempVal = soundPlaying;
			soundTime -= Time.deltaTime;
			
			if(soundTime > 0) {
				soundPlaying = true;
			} else {
				soundPlaying = false;
			}
			
			if(soundPlaying && !prevtempVal) {
				GetComponent<FMOD_StudioEventEmitter>().Play();
			} else if(!soundPlaying) {
				GetComponent<FMOD_StudioEventEmitter>().Stop();
			}
		}
	}

}
