using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public static float rotate;
	public static bool locked = false;

	public RectTransform damaged;

	private Vector3 offset;
	private float rotateSpeed;
	private Transform centerCamRef;

	private float zoom = 2f;

	private Player p;
	// Use this for initialization
	void Start () {
		offset = this.transform.position - Player.playerPos.position;
		p = GameObject.Find("PlayerObj").GetComponent<Player>();
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

		if(!PlayerCanvas.inConsole) {
			if(Input.GetAxis("Mouse ScrollWheel") > 0 && zoom < 4f) {
				transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y - 0.5f, transform.GetChild(0).transform.localPosition.z + 0.5f);
				transform.GetChild(1).transform.localPosition = new Vector3(transform.GetChild(1).transform.localPosition.x, transform.GetChild(1).transform.localPosition.y - 0.5f, transform.GetChild(1).transform.localPosition.z + 0.5f);
				zoom += 0.1f;
			} else if (Input.GetAxis("Mouse ScrollWheel") < 0 && zoom > 1f) {
				zoom -= 0.1f;
				transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y + 0.5f, transform.GetChild(0).transform.localPosition.z - 0.5f);
				transform.GetChild(1).transform.localPosition = new Vector3(transform.GetChild(1).transform.localPosition.x, transform.GetChild(1).transform.localPosition.y + 0.5f, transform.GetChild(1).transform.localPosition.z - 0.5f);
			}
			Vector3 lookDirection1 = (Player.playerPos.position + new Vector3(0,2,0) + Player.playerPos.forward*(1/zoom)) - transform.GetChild(0).transform.position;
			Vector3 lookDirection2 = (Player.playerPos.position + new Vector3(0,2,0) + Player.playerPos.forward*(1/zoom)) - transform.GetChild(1).transform.position;
			Quaternion lookRotation1 = Quaternion.LookRotation(lookDirection1);
			Quaternion lookRotation2 = Quaternion.LookRotation(lookDirection2);
			if(Input.GetAxis("Mouse ScrollWheel") == 0) {
				transform.GetChild(0).transform.rotation = Quaternion.RotateTowards(transform.GetChild(0).transform.rotation, lookRotation1, 0.1f);
				transform.GetChild(1).transform.rotation = Quaternion.RotateTowards(transform.GetChild(1).transform.rotation, lookRotation2, 0.1f);
			} else {
				transform.GetChild(0).transform.rotation = Quaternion.RotateTowards(transform.GetChild(0).transform.rotation, lookRotation1, 1f);
				transform.GetChild(1).transform.rotation = Quaternion.RotateTowards(transform.GetChild(1).transform.rotation, lookRotation2, 1f);
			}
		}

		damaged.localScale = Vector3.one * Mathf.Max(4f * p.GetIntegrityPercentage(), 1f);

	}

	/**
	 * Sets the MainCamera's position to that of the player
	 */
	public void SetToPlayer() {
		transform.parent.position = Player.playerPos.position;
	}

}
