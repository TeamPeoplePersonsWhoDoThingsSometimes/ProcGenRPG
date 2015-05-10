using UnityEngine;
using System.Collections;

public class Flamethrower : Weapon {
	
	public GameObject flameAttack;
	public GameObject flameParticles;

	private bool soundPlaying;
	private float soundTime = 0f;
	
	public override void Attack (float damage)
	{
		GameObject tempAttack = (GameObject)GameObject.Instantiate(flameAttack, Player.playerPos.position + new Vector3(0,1,0), Player.playerPos.rotation);
		tempAttack.GetComponent<Attack>().SetCrit(critChance);
		tempAttack.GetComponent<Attack>().SetDamage(damage + (Player.strength));
		GameObject.Instantiate(flameParticles, transform.position + new Vector3(0,-.8f,0) + Player.playerPos.forward, Player.playerPos.rotation);
		soundTime = 0.5f;
	}

	protected override void Update ()
	{
		base.Update ();

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
