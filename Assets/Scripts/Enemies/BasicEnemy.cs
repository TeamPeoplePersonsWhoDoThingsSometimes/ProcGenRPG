using UnityEngine;
using System.Collections;

public class BasicEnemy : Enemy {

	// Use this for initialization
	void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
		RaycastHit hitinfo = new RaycastHit();
		Debug.DrawRay(transform.position, transform.forward * 5f);
		if(Physics.Raycast(transform.position, transform.forward,out hitinfo, 10f)) {
			if(hitinfo.collider.gameObject.tag.Equals("Player")) {
				detectedPlayer = true;
			}
		} else {
			detectedPlayer = false;
		}
		if(!detectedPlayer) {
			transform.Rotate(0,20*Time.deltaTime,0);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, Player.playerPos.position + new Vector3(0,1,0), 0.1f);
		}
	}
}
