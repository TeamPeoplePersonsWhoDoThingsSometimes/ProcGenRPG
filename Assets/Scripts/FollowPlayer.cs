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
		GameObject centerCamRefObj = GameObject.Find("CenterEyeAnchor");
		if (centerCamRefObj != null) {
			centerCamRef = centerCamRefObj.GetComponent<Transform>();
		}
		offset = this.transform.position - Player.playerPos.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, transform.parent.eulerAngles.y + Input.GetAxis("Mouse X"), 0f)), 200*Time.deltaTime);
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, Player.playerPos.eulerAngles.y, 0f)), 2000*Time.deltaTime);

		rotate = transform.parent.eulerAngles.y;

//		this.transform.position = Vector3.MoveTowards(this.transform.position, Player.playerPos.position + offset, Time.deltaTime*10f);
		if (Input.GetKey(KeyCode.D)) {
			if(rotateSpeed <= 100f) {
				rotateSpeed = 100f;
			}
//			transform.parent.Rotate(Vector3.up, 100*Time.deltaTime, Space.World);
			rotateSpeed = Mathf.MoveTowards(rotateSpeed, 300, 100*Time.deltaTime);
			transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.A)) {
			if(rotateSpeed >= -100f) {
				rotateSpeed = -100f;
			}
//			transform.parent.Rotate(Vector3.up, -100*Time.deltaTime, Space.World);
			rotateSpeed = Mathf.MoveTowards(rotateSpeed, -300, 100*Time.deltaTime);
			transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
		}

		if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
			rotateSpeed = 0;
		}
//
		if(PlayerControl.PLAYINGWITHOCULUS) {
//			Debug.Log(centerCamRef.eulerAngles.y);
			if(Input.GetKey(KeyCode.Space) && centerCamRef.localEulerAngles.y > 20f && centerCamRef.localEulerAngles.y < 270f) {
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, 100*Time.deltaTime);
			} else if (Input.GetKey(KeyCode.Space) && centerCamRef.localEulerAngles.y < 340f && centerCamRef.localEulerAngles.y > 270f) {
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, -100*Time.deltaTime);
			}
		}

		if (!locked) {
			transform.parent.position = Vector3.MoveTowards(transform.parent.position, Player.playerPos.position, 50*Time.deltaTime);
		}

//		this.transform.LookAt(Player.playerPos);
	}

	/**
	 * Sets the MainCamera's position to that of the player
	 */
	public void SetToPlayer() {
		transform.parent.position = Player.playerPos.position;
	}
}
