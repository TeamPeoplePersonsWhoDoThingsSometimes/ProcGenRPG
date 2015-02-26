using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public static float rotate;
	public static bool locked = false;

	private Vector3 offset;
	private float rotateSpeed;
	private Transform centerCamRef;

	private float zoom = 2f;

	private static int damagedTime = 0;
	private bool damageDisplay = false;
	private bool waitingforscreencap = false;
	private static Texture2D screenCap;

	private float width;
	private float height;
	private float x;
	private float y;
	private float xOffset;
	private float yOffset;
	// Use this for initialization
	void Start () {
		offset = this.transform.position - Player.playerPos.position;
		screenCap = new Texture2D(Screen.width, Screen.height);
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

		if(damagedTime == 1) {
			if(!waitingforscreencap) {
				StartCoroutine(ReadScreen());
				damageDisplay = true;
			}
			damagedTime--;
		} else if(Time.frameCount % 5 == 0) {
			damageDisplay = false;
		}

		if(width == 0 || Time.frameCount % 20 == 0) {
			width = Random.Range(50,300);
			height = Random.Range(50,300);
			x = Random.Range(0,Screen.width - width);
			y = Random.Range(0,Screen.height - height);
//			xOffset = Random.Range(-0.01f,0.01f);
//			yOffset = Random.Range(-0.01f,0.01f);
			xOffset = 0;
			yOffset = 0;
			//				width = Screen.width;
			//				height = Screen.height;
			//				x = 0;
			//				y = 0;
		}

	}

	/**
	 * Sets the MainCamera's position to that of the player
	 */
	public void SetToPlayer() {
		transform.parent.position = Player.playerPos.position;
	}

	public static void PlayerDamaged() {
		damagedTime = 1;
	}

	private IEnumerator ReadScreen() {
		waitingforscreencap = true;
		yield return new WaitForEndOfFrame();
		Debug.Log("HERE" + Time.frameCount);
		screenCap.ReadPixels(new Rect(0,0, Screen.width, Screen.height), 0, 0);
		screenCap.Apply();
		waitingforscreencap = false;
//		for (int i = 0; i < screenCap.GetPixels().Length; i++) {
//			screenCap.GetPixels()[i].r *= 2;
//		}
	}

	void OnGUI() {
		if(damageDisplay) {
			GUI.DrawTextureWithTexCoords(new Rect(x,y, width, height), screenCap, new Rect(x/Screen.width + xOffset,y/Screen.height + yOffset, width/Screen.width, height/Screen.height));
//			GUI.DrawTexture(new Rect(x,y, width, height), screenCap);
		}
	}
}
