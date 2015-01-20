using UnityEngine;
using System.Collections;

public class HexplosionAttack: Attack {

	protected void Start () {
		Color c = new Color(Random.value, Random.value, Random.value);
		renderer.material.color = c;
		GetComponent<LineRenderer>().SetColors(c, Color.white);
	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(transform.forward/2f);

		duration -= Time.deltaTime;
		if(duration < 0) {
			Destroy(this.gameObject);
		}
	}
}
