using UnityEngine;
using System.Collections;

public class NullBeam : Hack {

	public bool shootRight = true;

	private bool soundPlaying;
	private float soundTime = 0f;

	protected override void OneShotActivated ()
	{
		base.OneShotActivated ();
//		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + ((attack.transform.localScale.x/2f)*Player.playerPos.right), Quaternion.LookRotation(Player.playerPos.right));
		GameObject tempAttack = (GameObject)Instantiate(attack, Player.playerPos.position + Player.playerPos.forward*attack.transform.localScale.x/1.95f + new Vector3(0f, 2f), Quaternion.LookRotation(shootRight ? Player.playerPos.right : Player.playerPos.forward));
		tempAttack.GetComponent<Attack>().SetDamage(damage + (Player.strength*2));
		tempAttack.GetComponent<Attack>().SetCrit(critChance);

		if(name.Equals("NullBolt")) {
			FMOD_StudioSystem.instance.PlayOneShot("event:/weapons/nullBolt", Player.playerPos.position,PlayerPrefs.GetFloat("MasterVolume")/2f);
		} else {
			soundTime = 0.1f;
		}
	}

	protected override void Update ()
	{
		base.Update ();

		if(this != null && this.GetComponent<Hack>().name.Contains("Beam")) {
			bool prevtempVal = soundPlaying;
			soundTime -= Time.deltaTime;
			
			if(soundTime > 0) {
				soundPlaying = true;
			} else {
				soundPlaying = false;
			}
			
			if(soundPlaying && !prevtempVal) {
//				Debug.Log("HERE");
				GetComponent<FMOD_StudioEventEmitter>().Play();
			} else if(!soundPlaying) {
				GetComponent<FMOD_StudioEventEmitter>().Stop();
			}
		}
	}
}
