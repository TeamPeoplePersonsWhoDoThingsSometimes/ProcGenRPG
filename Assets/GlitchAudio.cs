using UnityEngine;
using System.Collections;

public class GlitchAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<AudioSource>().isPlaying && Random.value < 0.75f) {
			GetComponent<AudioSource>().time += Random.value*10 - 5;
			GetComponent<AudioSource>().pitch = Random.value + 0.5f;
		}
	}
}
