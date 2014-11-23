using UnityEngine;
using System.Collections;

public class NullBeamAttack : Attack {

	// Update is called once per frame
	void FixedUpdate () {
		duration -= Time.deltaTime;
		if(duration < 0) {
			Destroy(this.gameObject);
		}
	}
}
