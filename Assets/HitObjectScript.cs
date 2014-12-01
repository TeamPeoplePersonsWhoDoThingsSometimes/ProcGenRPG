using UnityEngine;
using System.Collections;

public class HitObjectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Done")) {
			Destroy(this.gameObject);
		}
	}
}
