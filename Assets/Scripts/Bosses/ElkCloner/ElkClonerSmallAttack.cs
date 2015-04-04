using UnityEngine;
using System.Collections;

public class ElkClonerSmallAttack : Attack {

	private float timer = 0.1f;
	private float randomVal;
	public bool followPlayer = true;

	protected override void Start ()
	{
		base.Start ();
		randomVal = Random.value;
	}

	protected override void Update ()
	{
		timer -= Time.deltaTime;
		if(followPlayer) {
			if(timer > 0f) {
				transform.LookAt(Vector3.up);
				GetComponent<Rigidbody>().AddForce(new Vector3(randomVal*2f, 100f, randomVal*2f));
			} else {
				transform.LookAt(Player.playerPos.position + new Vector3(0,1,0));
				GetComponent<Rigidbody>().AddForce(transform.forward*200f, ForceMode.Acceleration);
			}
		} else {
			GetComponent<Rigidbody>().AddForce(transform.forward*200f, ForceMode.Acceleration);
		}
	}

}
