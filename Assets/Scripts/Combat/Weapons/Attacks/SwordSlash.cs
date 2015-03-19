using UnityEngine;
using System.Collections;

public class SwordSlash : Attack {

	public float speedScale = 1f;

	// Update is called once per frame
	protected override void Update () {
		base.Update();
		transform.Translate(new Vector3(0f, 0f, 50*Time.deltaTime*speedScale));
	}
}
