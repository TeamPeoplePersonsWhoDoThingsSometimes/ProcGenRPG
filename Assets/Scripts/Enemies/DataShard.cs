using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataShard : Enemy {

	public GameObject deathSplosion;

	private static Boss finalBoss;

	void Start() {
		base.Start();
		if (finalBoss == null) {
			finalBoss = GameObject.Find("FINALBOSS").GetComponent<Boss>();
		}
	}
	
	void FixedUpdate () {

	}

	protected override void DoIdle ()
	{

	}

	protected override void HandleDeath ()
	{
		GameObject.Instantiate(deathSplosion, transform.position, Quaternion.identity);
		finalBoss.GetDamaged(1,false);
		GetComponent<MeshRenderer>().enabled = false;
		Destroy(this);
	}

	protected override void HandleDetectedPlayer ()
	{

	}

	protected override void HandleEffect ()
	{

	}

	protected override void HandleKnockback ()
	{

	}

	protected override void HandlePlayerDetection ()
	{

	}

	
}
