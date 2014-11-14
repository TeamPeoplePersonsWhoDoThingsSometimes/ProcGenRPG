using UnityEngine;
using System.Collections;

public class Byte : MonoBehaviour {

	public int val;

	private float timeOffset;

	private static Player playerRef;

	// Use this for initialization
	void Start () {
		if(playerRef == null) {
			playerRef = GameObject.Find("PlayerObj").GetComponent<Player>();
		}

		float scale = Mathf.Min(0.0002f*val, 0.4f);
		this.transform.localScale = new Vector3(scale, scale, scale);
	}
	
	// Update is called once per frame
	void Update () {
		timeOffset += Time.deltaTime;

		if(timeOffset < 0.1f) {
			rigidbody.AddForce(new Vector3(Random.value*8f - 4, Random.value*2f, Random.value*8f - 4), ForceMode.VelocityChange);
		}

		if ((timeOffset > 1 && Vector3.Distance(Player.playerPos.position, this.transform.position) < 10f) || timeOffset > 2) {
			this.transform.position = Vector3.MoveTowards(this.transform.position, Player.playerPos.position + new Vector3(0,1,0),20*Time.deltaTime);
			this.rigidbody.useGravity = false;
			this.collider.enabled = false;
		}

		if(timeOffset > 1 && Vector3.Distance(Player.playerPos.position + new Vector3(0,1,0), this.transform.position) < 1f) {
			playerRef.AddBytes(val);
			Destroy(this.gameObject);
		}

	}
}
