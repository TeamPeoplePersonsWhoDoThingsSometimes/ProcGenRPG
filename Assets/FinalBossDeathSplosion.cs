using UnityEngine;
using System.Collections;

public class FinalBossDeathSplosion : MonoBehaviour {

	float timeCounter = 0f;
	int soundsDone = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeCounter += Time.deltaTime;
		for(int i = soundsDone; i < transform.childCount; i++) {
			if(timeCounter >= transform.GetChild(i).GetComponent<ParticleSystem>().startDelay) {
				soundsDone++;
				if(i != transform.childCount - 1) {
					FMOD_StudioSystem.instance.PlayOneShot("event:/weapons/hexplosion",Player.playerPos.position, PlayerPrefs.GetFloat("MasterVolume")/2f);
				} else {
					FMOD_StudioSystem.instance.PlayOneShot("event:/boss/bossAttackB",Player.playerPos.position, PlayerPrefs.GetFloat("MasterVolume")/2f);
				}
				break;
			}
		}
	}
}
