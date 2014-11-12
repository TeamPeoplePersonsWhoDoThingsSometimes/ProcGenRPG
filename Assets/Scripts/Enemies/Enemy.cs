using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float maxHP;
	public string name;
	public string version;
	private float hp;

	protected bool detectedPlayer;

	private float knockbackVal;
	private float knockbackTime;
	private Vector3 knockbackPos;

	private static GameObject hitInfo;

	// Use this for initialization
	protected void Start () {
		hp = maxHP;
		if(hitInfo == null) {
			hitInfo = Resources.Load<GameObject>("Info/HitInfo");
		}
	}
	
	// Update is called once per frame
	protected void Update () {
		if (hp < 0) {
			Destroy(this.gameObject);
		}
		if(knockbackTime > 0) {
			knockbackTime -= Time.deltaTime;
			Vector3 dir = transform.position - knockbackPos;
			dir.y = 0f;
			rigidbody.AddForceAtPosition(dir*knockbackVal,knockbackPos, ForceMode.Impulse);
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
			temp.GetComponent<TextMesh>().renderer.material.color = Color.red;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
		} else {
			hp -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag.Equals("PlayerAttack")) {
			Attack attack = other.gameObject.GetComponent<Attack>();
		}
	}

	public float GetHealthPercentage() {
		return hp/maxHP;
	}
}
