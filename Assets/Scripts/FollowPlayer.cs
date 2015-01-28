using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public static float rotate;
	public static bool locked = false;

	private Vector3 offset;
	private float rotateSpeed;
	private Transform centerCamRef;
	// Use this for initialization
	void Start () {
		offset = this.transform.position - Player.playerPos.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, transform.parent.eulerAngles.y + Input.GetAxis("Mouse X"), 0f)), 200*Time.deltaTime);
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, Player.playerPos.eulerAngles.y, 0f)), 2000*Time.deltaTime);

		rotate = transform.parent.eulerAngles.y;

		if(!PlayerControl.immobile) {
			if (Input.GetKey(KeyCode.D)) {
				if(rotateSpeed <= 100f) {
					rotateSpeed = 100f;
				}
				rotateSpeed = Mathf.MoveTowards(rotateSpeed, 300, 100*Time.deltaTime);
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.A)) {
				if(rotateSpeed >= -100f) {
					rotateSpeed = -100f;
				}
				rotateSpeed = Mathf.MoveTowards(rotateSpeed, -300, 100*Time.deltaTime);
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
			}
		}

		if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
			rotateSpeed = 0;
		}

		if (!locked) {
			transform.parent.position = Vector3.MoveTowards(transform.parent.position, Player.playerPos.position, 50*Time.deltaTime);
		}

		if(Input.GetAxis("Mouse ScrollWheel") > 0) {
			transform.GetChild(0).camera.fieldOfView = Mathf.Clamp(++transform.GetChild(0).camera.fieldOfView, 30f, 60f);
			transform.GetChild(1).camera.fieldOfView = Mathf.Clamp(++transform.GetChild(1).camera.fieldOfView, 30f, 60f);
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			transform.GetChild(0).camera.fieldOfView = Mathf.Clamp(--transform.GetChild(0).camera.fieldOfView, 30f, 60f);
			transform.GetChild(1).camera.fieldOfView = Mathf.Clamp(--transform.GetChild(1).camera.fieldOfView, 30f, 60f);
		}

//		this.transform.LookAt(Player.playerPos);
	}

	/**
	 * Sets the MainCamera's position to that of the player
	 */
	public void SetToPlayer() {
		transform.parent.position = Player.playerPos.position;
	}

	public void ZoomOut() {

	}
}
