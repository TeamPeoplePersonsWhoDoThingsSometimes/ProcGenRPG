using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public float maxHP;
	public string name;
	public float badassChance;
	public GameObject enemyAttack;
	public float baseAttackSpeed;
	public float baseAttackDamage;
	public string maxVersion;
	public string minVersion;
	public float healthScale = 1f;
	public float attackScale = 1f;
	public float attackSpeedScale = 1f;

	protected float tempAttackSpeed;
	protected bool detectedPlayer;
	protected bool isBadass;
	protected string version;

	private float knockbackVal;
	private float knockbackTime;
	private Vector3 knockbackPos;
	private float hp;
	private Dictionary<GameObject, float> itemDrops = new Dictionary<GameObject, float>();
	public List<GameObject> possibleItemDrops;
	public List<float> possibleItemDropsChance;

	private static GameObject hitInfo;
	private static GameObject byteObject;

	private Vector3 lastPos;

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

		if(hitInfo == null) {
			hitInfo = Resources.Load<GameObject>("Info/HitInfo");
		}
		if(byteObject == null) {
			byteObject = Resources.Load<GameObject>("Info/Byte");
		}

		int minversionInt = Utility.VersionToInt(minVersion);
		int maxversionInt = Utility.VersionToInt(maxVersion);

		int versionInt = Random.Range(Mathf.Min(Mathf.Max(minversionInt, Utility.VersionToInt(Player.version) - 5), maxversionInt),Mathf.Min(Utility.VersionToInt(Player.version) + 5, maxversionInt));
		this.maxHP *= (int)((versionInt/100f)*(healthScale+1));
		this.baseAttackDamage *= (versionInt/100f)*(attackScale+1);
		this.baseAttackSpeed /= (versionInt/100f)*(attackSpeedScale+1);
		this.version = Utility.IntToVersion(versionInt);
		hp = maxHP;

		if(possibleItemDrops.Count != possibleItemDropsChance.Count) {
			Debug.LogWarning("Hey dummy! You need to have equal number of item drops and item drop chances!");
		} else {
			for(int i = 0; i < possibleItemDrops.Count; i++) {
				itemDrops.Add(possibleItemDrops[i], possibleItemDropsChance[i]);
			}
		}
	}

	public string GetVersion() {
		return version;
	}

	public string HealthString() {
		return hp + "/" + maxHP;
	}
	
	// Update is called once per frame
	protected void Update () {
		if (hp <= 0) {
			int tempByteVal = (int)maxHP*1000;
			int curByteVal = 0;
			int byteVal = Mathf.Max(tempByteVal/5, 5000);
			while (curByteVal < tempByteVal) {
				GameObject tmp = (GameObject)Instantiate(byteObject, transform.position, Quaternion.identity);
				tmp.GetComponent<Byte>().val = byteVal;
				curByteVal += byteVal;
			}

			foreach(KeyValuePair<GameObject, float> kvp in itemDrops) {
				if(Random.value < kvp.Value) {
					GameObject temp = null;
					switch(kvp.Key.GetComponent<Item>().RarityVal) {
					case Rarity.Common:
						temp = (GameObject)Instantiate(Utility.GetCommonItemDrop(), this.transform.position, Quaternion.identity);
						break;
					case Rarity.Uncommon:
						temp = (GameObject)Instantiate(Utility.GetUncommonItemDrop(), this.transform.position, Quaternion.identity);
						break;
					}
					if (temp != null) {
						temp.GetComponent<ItemDropObject>().item = kvp.Key;
						Weapon tempweapon = temp.GetComponent<ItemDropObject>().item.GetComponent<Weapon>();
						if (tempweapon != null) {
							tempweapon.version = version;
						}
					}
					break;
				}
			}

			//We should figure out how to handle death in a way that more closely ties player attacks to the death of the enemy
			//to provide for more complex action tracking capailities, also, I'll move this into the backend
			//when I move everything else that should be in the model as well
			DirectObject obj = new DirectObject("N/A", name);
			PlayerAction action = new PlayerAction(obj, ActionType.KILL);
			ActionEventInvoker.primaryInvoker.invokeAction(action);

			Destroy(this.gameObject);
		}

		if (knockbackTime > 0) {
			knockbackTime -= Time.deltaTime;
			Vector3 dir = transform.position - knockbackPos;
			dir.y = 0f;
//			rigidbody.AddForceAtPosition(dir*knockbackVal,knockbackPos, ForceMode.Impulse);
			rigidbody.velocity = -transform.forward*knockbackVal;
		}

		RaycastHit hitinfo = new RaycastHit();
		Debug.DrawRay(transform.position, transform.forward * 5f);
		if(Physics.Raycast(transform.position, transform.forward,out hitinfo, 10f)) {
			if(hitinfo.collider.gameObject.tag.Equals("Player")) {
				detectedPlayer = true;
			}
		} else {
			DoIdle();
		}

		GetComponent<Animator>().SetFloat("Speed", Vector3.Distance(transform.position, lastPos));

		lastPos = transform.position;

		if (detectedPlayer && Vector3.Distance(Player.playerPos.position, transform.position) > 3f) {
			GetComponent<Animator>().SetTrigger("PlayerSpotted");
			rigidbody.MovePosition(Vector3.MoveTowards(transform.position, Player.playerPos.position + new Vector3(0,1,0), 0.1f));
			transform.LookAt(Player.playerPos.position + new Vector3(0,1,0));
		} else if (Vector3.Distance(Player.playerPos.position, transform.position) <= 3f) {
			tempAttackSpeed -= Time.deltaTime;
			if(tempAttackSpeed <= 0) {
				GetComponent<Animator>().SetTrigger("Attack");
				tempAttackSpeed = baseAttackSpeed;
			}
		} else {
			tempAttackSpeed = baseAttackSpeed;
		}
	}

	void FixedUpdate () {
		if(rigidbody.IsSleeping()) {
			rigidbody.WakeUp();
		}

	}

	protected void DoIdle() {
		rigidbody.MoveRotation(Quaternion.Euler(transform.eulerAngles + new Vector3(0,10*Time.deltaTime,0f)));
	}

	public void DoKnockback(Vector3 pos, float knockback) {
		knockbackTime = 0.2f;
		knockbackVal = knockback;
		knockbackPos = pos;
	}

	public void GetDamaged(float damage, bool crit) {
		GetComponent<Animator>().SetTrigger("Hurt");
		detectedPlayer = true;
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position, hitInfo.transform.rotation);
		if (crit) {
			hp -= damage*2;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.yellow;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
		} else {
			hp -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
		PlayerCanvas.RegisterEnemyHealthBar(this.gameObject);
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag.Equals("PlayerAttack")) {
			Attack attack = other.gameObject.GetComponent<Attack>();
			if(attack.damageEnemy) {
//				Time.timeScale = 0f;
			}
		}
	}

	public float GetHealthPercentage() {
		return hp/maxHP;
	}

}
