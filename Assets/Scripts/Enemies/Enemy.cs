﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float maxHP;
	public string name;
	public string version;
	public float badassChance;
	public GameObject enemyAttack;

	protected bool detectedPlayer;
	protected bool isBadass;

	private float knockbackVal;
	private float knockbackTime;
	private Vector3 knockbackPos;
	private float hp;

	private static GameObject hitInfo;
	private static GameObject byteObject;

	// Use this for initialization
	protected void Start () {
		if(Random.value < badassChance) {
			isBadass = true;
			this.transform.localScale *= 2;
			this.transform.GetChild(2).localScale /= 1.5f;
			this.transform.GetChild(2).localPosition += new Vector3(0f, 1f);
			this.maxHP *= 2;
			this.name = "Badass " + name;
		} else {
			this.name = "Basic " + name;
		}

		hp = maxHP;
		if(hitInfo == null) {
			hitInfo = Resources.Load<GameObject>("Info/HitInfo");
		}
		if(byteObject == null) {
			byteObject = Resources.Load<GameObject>("Info/Byte");
		}
	}
	
	// Update is called once per frame
	protected void Update () {
		if (hp < 0) {
			int tempByteVal = (int)maxHP*1000;
			int curByteVal = 0;
			int byteVal = Mathf.Max(tempByteVal/5, 5000);
			while (curByteVal < tempByteVal) {
				GameObject tmp = (GameObject)Instantiate(byteObject, transform.position, Quaternion.identity);
				tmp.GetComponent<Byte>().val = byteVal;
				curByteVal += byteVal;
			}

			Destroy(this.gameObject);
		}
		if (knockbackTime > 0) {
			knockbackTime -= Time.deltaTime;
			Vector3 dir = transform.position - knockbackPos;
			dir.y = 0f;
			rigidbody.AddForceAtPosition(dir*knockbackVal,knockbackPos, ForceMode.Impulse);
		}

		RaycastHit hitinfo = new RaycastHit();
		Debug.DrawRay(transform.position, transform.forward * 5f);
		if(Physics.Raycast(transform.position, transform.forward,out hitinfo, 10f)) {
			if(hitinfo.collider.gameObject.tag.Equals("Player")) {
				detectedPlayer = true;
			}
		} else {
			detectedPlayer = false;
		}

		if (detectedPlayer) {
			GetComponent<Animator>().SetTrigger("PlayerSpotted");
			GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(rigidbody.velocity.magnitude));
			transform.position = Vector3.MoveTowards(transform.position, Player.playerPos.position + new Vector3(0,1,0), 0.1f);
		} else {
			transform.Rotate(0,20*Time.deltaTime,0);
		}
	}

	void FixedUpdate () {
		if(rigidbody.IsSleeping()) {
			rigidbody.WakeUp();
		}
	}

	public void DoKnockback(Vector3 pos, float knockback) {
		knockbackTime = 0.2f;
		knockbackVal = knockback;
		knockbackPos = pos;
	}

	public void GetDamaged(float damage, bool crit) {
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position, hitInfo.transform.rotation);
		if (crit) {
			hp -= damage*2;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.yellow;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
		} else {
			hp -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag.Equals("PlayerAttack")) {
			Attack attack = other.gameObject.GetComponent<Attack>();
			GetComponent<Animator>().SetTrigger("Hurt");
		}
	}

	public float GetHealthPercentage() {
		return hp/maxHP;
	}

}
