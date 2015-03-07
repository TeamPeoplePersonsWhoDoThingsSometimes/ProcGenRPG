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
	public float baseHealthRegen;
	public string maxVersion;
	public string minVersion;
	public float healthScale = 1f;
	public float attackScale = 1f;
	public float attackSpeedScale = 1f;
	public float healthRegenScale = 1f;

	public Effect currentEffect;
	private float effectTime;
	private float effectValue;

	protected float tempAttackSpeed;
	protected bool detectedPlayer, retreating;
	protected bool badassSet;
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

	private float healthBarTime;

	private Vector3 lastPos;

	public DirectObject getDirectObject() {
		int modifierEnd = name.IndexOf (" ");
		return new DirectObject (name.Substring((modifierEnd == -1 ? 0: modifierEnd + 1)), (isBadass? "Badass" : "Basic" ));
	}

	private float seed1 = 0f;
	private float seed2 = 0f;
	private int count = 0;

	protected void Awake() {
		if (!badassSet) {
			setBadass (Random.value < badassChance);
		}
	}

	// Use this for initialization
	protected void Start () {

		if(hitInfo == null) {
			hitInfo = Resources.Load<GameObject>("Info/HitInfo");
		}
		if(byteObject == null) {
			byteObject = Resources.Load<GameObject>("Info/Byte");
		}

		int minversionInt = Utility.VersionToInt(minVersion);
		int maxversionInt = Utility.VersionToInt(maxVersion);
		int minRange = Mathf.Min(Mathf.Max(minversionInt, Utility.VersionToInt(Player.version) - 5), maxversionInt);
		int maxRange = Mathf.Max(Mathf.Min(Utility.VersionToInt(Player.version) + 5, maxversionInt), minversionInt);

		int versionInt = Random.Range(minRange,maxRange);
		this.maxHP *= (int)((versionInt/100f)*(healthScale+1));
		this.baseAttackDamage *= (versionInt/100f)*(attackScale+1);
		this.baseHealthRegen *= (versionInt/100f)*(healthRegenScale+1);
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

		currentEffect = Effect.None;

		PlayerCanvas.RegisterEnemyHealthBar(this.gameObject);
	}

	public string GetVersion() {
		return version;
	}

	public void SetVersion(string v) {
		version = v;
	}

	public bool IsBadass() {
		return isBadass;
	}

	public void setBadass(bool b) {
		if (b && !isBadass) {
			this.transform.localScale *= 2;
			this.maxHP *= 2;
			this.baseHealthRegen *= 2;
			this.name = "Badass " + name;
		} else if (!b && isBadass) {
			this.transform.localScale /= 2;
			this.maxHP /= 2;
			this.baseHealthRegen /= 2;
			this.name = name.Substring(7);
		}
		isBadass = b;
		badassSet = true;
	}

	public string HealthString() {
		return hp + "/" + maxHP;
	}

	// Update is called once per frame
	protected void Update () {
//		transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0f, 1f), transform.position.z);

		healthBarTime -= Time.deltaTime;

		if(hp <= 0) {
			HandleDeath();
		} else if (hp > maxHP) {
			hp = maxHP;
		}

		HandleKnockback();

		if(!detectedPlayer) {
			HandlePlayerDetection();
			DoIdle();
		} else {
			HandleDetectedPlayer();
		}

		HandleEffect();

		/*** Updates speed value in Mecanim ***/
		GetComponent<Animator>().SetFloat("Speed", Vector3.Distance(transform.position, lastPos));

		/*** Handles manual speed calculation since rigidbody.velocity doesn't work ***/
		lastPos = transform.position;

		hp += Time.deltaTime*baseHealthRegen/10f;

		if(transform.position.y < -10f) {

			//trigger kill event for this enemy
			ActionEventInvoker.primaryInvoker.invokeAction(new PlayerAction(getDirectObject(), ActionType.KILL));

			Destroy(this.gameObject);
		}
	}

	protected virtual void HandleEffect() {
		if(effectTime <= 0) {
			currentEffect = Effect.None;
		}
		if(currentEffect != Effect.None) {
			float prevEffectTime = effectTime;
			effectTime -= Time.deltaTime;

			if(currentEffect == Effect.Deteriorating
			   && (int)effectTime < (int)prevEffectTime) {
				GetDamaged(effectValue, false);
				GameObject tempbyte = (GameObject) GameObject.Instantiate(Utility.GetByteObject(), transform.position, Quaternion.identity);
				tempbyte.GetComponent<Byte>().val = (int)effectValue*100;
			}

			if (currentEffect == Effect.Slow) {
				transform.position -= (transform.position - lastPos)/2f;
			}
			if (currentEffect == Effect.Bugged) {
				if ((count % 15) == 0) { //walks in a random direction, changes direction every 15 frames
					seed1 = Random.value *  - 0.5f;
					seed2 = Random.value *  - 0.5f;
				}
				transform.position += new Vector3(seed1 * 0.6f, 0f, seed2 * 0.6f);
				count++;
			}
			if (currentEffect == Effect.Weakened) {
				GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position, hitInfo.transform.rotation);
				temp.GetComponent<TextMesh>().renderer.material.color = Color.cyan;
			}
			if (currentEffect == Effect.Virus) {
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
				int i = 0;
				while (i < hitColliders.Length) {
					if (hitColliders[i].gameObject.GetComponent<Enemy>()!=null){
							Enemy temp = (Enemy) hitColliders[i].gameObject.GetComponent<Enemy>();
							temp.GetDamaged(Effect.Virus, effectValue, effectTime);
							temp.GetDamaged(Effect.Deteriorating, effectValue, effectTime);
					}
				i++;
				}
			} 
			if (currentEffect == Effect.Stun) {
				transform.position = lastPos;
			}
		}
	}


	protected virtual void HandleDetectedPlayer() {
		/*** Makes nearby enemies aware of your presence ***/
		Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 10f);
		foreach(Collider c in nearbyColliders) {
			if(c.gameObject.GetComponent<Enemy>() != null) {
				c.gameObject.GetComponent<Enemy>().AlertEnemy();
			}
		}

		/*** If player is too far and not in the line of sight, forget player ***/
		RaycastHit hitinfo = new RaycastHit();
		if(Vector3.Distance(Player.playerPos.position, transform.position) > 50f
		   && !Physics.Raycast(transform.position, transform.forward,out hitinfo, 100f)
		   && hitinfo.collider != null && hitinfo.collider.gameObject != null
		   && hitinfo.collider.gameObject.tag.Equals("Player")) {
			detectedPlayer = false;
		}
		
		/*** Handle retreating ***/
		if(GetHealthPercentage() < 0.25f) {
			retreating = true;
			transform.LookAt(Player.playerPos.position + new Vector3(0,1,0));
			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + 180f, 0f);
			transform.Translate(new Vector3(transform.forward.x, 0f, transform.forward.z)*Time.deltaTime*2f, Space.World);
		} else {
			retreating = false;
		}

		/*** Handle Moving towards player and attacking ***/
		if (Vector3.Distance(Player.playerPos.position, transform.position) > 3f && !retreating) {
			GetComponent<Animator>().SetTrigger("PlayerSpotted");
			rigidbody.MovePosition(Vector3.MoveTowards(transform.position, Player.playerPos.position + new Vector3(0,1,0), 0.1f));
			transform.LookAt(Player.playerPos.position + new Vector3(0,1,0));
		} else if (Vector3.Distance(Player.playerPos.position, transform.position) <= 3f && !retreating) {
			transform.LookAt(Player.playerPos.position + new Vector3(0,1,0));
			tempAttackSpeed -= Time.deltaTime;
			if(tempAttackSpeed <= 0) {
				GetComponent<Animator>().SetTrigger("Attack");
				tempAttackSpeed = baseAttackSpeed;
			}
		} else {
			tempAttackSpeed = baseAttackSpeed;
		}
	}

	protected virtual void HandleDeath() {
	/*** Death ****/
		int tempByteVal = (int)maxHP*1000;
		int curByteVal = 0;
		int byteVal = Mathf.Max(tempByteVal/5, 5000);
		while (curByteVal < tempByteVal) {
			GameObject tmp = (GameObject)Instantiate(byteObject, transform.position, Quaternion.identity);
			tmp.GetComponent<Byte>().val = byteVal;
			curByteVal += byteVal;
		}
		
		/***** Handles item drops *****/
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
		DirectObject obj = getDirectObject ();
		PlayerAction action = new PlayerAction(obj, ActionType.KILL);
		ActionEventInvoker.primaryInvoker.invokeAction(action);
		
		Destroy(this.gameObject);
	}

	protected virtual void HandleKnockback() {
		/*** Handles knockback ***/
		if (knockbackTime > 0) {
			knockbackTime = 0;
			Vector3 dir = transform.position - knockbackPos;
			dir.y = 0f;
			rigidbody.AddForceAtPosition(dir*knockbackVal,knockbackPos, ForceMode.VelocityChange);
//			rigidbody.velocity = dir*knockbackVal;
		}
	}

	protected virtual void HandlePlayerDetection() {
		/*** Handles seeing the player ***/
		RaycastHit hitinfo = new RaycastHit();
		Ray r1 = new Ray(transform.position + transform.right, (transform.forward + transform.right)*20f);
//		Debug.DrawRay(r1.origin, r1.direction*10f);
		Ray r2 = new Ray(transform.position - transform.right, (transform.forward + transform.right)*20f);
//		Debug.DrawRay(r2.origin, r2.direction*10f);
		Ray r3 = new Ray(transform.position + transform.right, (transform.forward - transform.right)*20f);
//		Debug.DrawRay(r3.origin, r3.direction*10f);
		Ray r4 = new Ray(transform.position - transform.right, (transform.forward - transform.right)*20f);
//		Debug.DrawRay(r4.origin, r4.direction*10f);
		if(Physics.Raycast(transform.position, transform.forward,out hitinfo, 20f)
		   || Physics.Raycast(r1, out hitinfo, 10f)
		   || Physics.Raycast(r2, out hitinfo, 10f)
		   || Physics.Raycast(r3, out hitinfo, 10f)
		   || Physics.Raycast(r4, out hitinfo, 10f)) {
			if(hitinfo.collider.gameObject.tag.Equals("Player")) {
				detectedPlayer = true;
			}
		}
	}

	public void AlertEnemy() {
		detectedPlayer = true;
	}

	void FixedUpdate () {
		if(rigidbody.IsSleeping()) {
			rigidbody.WakeUp();
		}
	}

	protected virtual void DoIdle() {
		rigidbody.MoveRotation(Quaternion.Euler(transform.eulerAngles + new Vector3(0,10*Time.deltaTime,0f)));
	}

	public void DoKnockback(Vector3 pos, float knockback) {
		knockbackTime = 0.2f;
		knockbackVal = knockback;
		knockbackPos = pos;
	}

	public void GetDamaged(float damage, bool crit) {
		healthBarTime = 2f;
		GetComponent<Animator>().SetTrigger("Hurt");
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position, hitInfo.transform.rotation);
		if (currentEffect == Effect.Weakened) {
			damage = damage * 1.5f;
		}
		if (!detectedPlayer) {
			hp -= damage*4;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.blue;
			temp.GetComponent<TextMesh>().text = "*" + damage*4 + "*";
		} else if (crit) {
			hp -= damage*2;
			temp.GetComponent<TextMesh>().renderer.material.color = Color.yellow;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
		} else {
			hp -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
		detectedPlayer = true;
	}

	public void GetDamaged(Effect attackEffect, float effectValue, float effectTime) {
		this.effectTime = effectTime;
		currentEffect = attackEffect;
		this.effectValue = effectValue;
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

	public bool ShowHealthbar() {
		return healthBarTime > 0;
	}
	
}
