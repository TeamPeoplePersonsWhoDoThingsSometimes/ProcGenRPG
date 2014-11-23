using UnityEngine;
using System.Collections;

public class SwordSlash : Attack {

	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(new Vector3(0f, 0f, 50*Time.deltaTime));
		duration -= Time.deltaTime;
		if(duration < 0) {
			Destroy(this.gameObject);
		}
	}
}
