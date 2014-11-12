using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	private Vector3 offset;
	// Use this for initialization
	void Start () {

		offset = this.transform.position - Player.playerPos.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, transform.parent.eulerAngles.y + Input.GetAxis("Mouse X"), 0f)), 200*Time.deltaTime);
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, Player.playerPos.eulerAngles.y, 0f)), 2000*Time.deltaTime);

//		this.transform.position = Vector3.MoveTowards(this.transform.position, Player.playerPos.position + offset, Time.deltaTime*10f);
		if (Input.GetKey(KeyCode.Q)) {
//			transform.parent.Rotate(Vector3.up, 100*Time.deltaTime, Space.World);
			transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, 100*Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.E)) {
//			transform.parent.Rotate(Vector3.up, -100*Time.deltaTime, Space.World);
			transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, -100*Time.deltaTime);
		}

		transform.parent.position = Vector3.MoveTowards(transform.parent.position, Player.playerPos.position, 50*Time.deltaTime);

//		this.transform.LookAt(Player.playerPos);
	}
}
