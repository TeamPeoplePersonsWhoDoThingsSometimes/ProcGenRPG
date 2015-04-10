using UnityEngine;
using System.Collections;

public enum Effect {
	None,
	Deteriorating,
	Slow,
	Stun,
	Weakened,
	Bugged,
	Virus
}

public class Attack : MonoBehaviour {

	protected float thisDamage;
	protected float critChance;

	public bool damagePlayer;
	public bool damageEnemy;
	public float knockback;
	public float duration;
	public bool destroyOnImpact;
	public GameObject hitObject;
	public Effect attackEffect;
	public float attackEffectChance;
	public float attackEffectValue;
	public float attackEffectTime;

	// Use this for initialization
	protected virtual void Start () {

	}
	
	// Update is called once per frame
	protected virtual void Update () {
		//If the duration is not zero, then countdown to destroy attack
		if(duration != 0) {
			duration -= Time.deltaTime;
			if(duration <= 0) {
				Destroy(this.gameObject);
			}
		}
	}


	void OnTriggerEnter(Collider other) {
		//If we want to damage an enemy and the other object has an enemy component
		if (damageEnemy && other.GetComponent<Enemy>() != null) {

			//current algorithm for what damage will be dealt
			if(Random.value < thisDamage - (int)thisDamage) {
				other.GetComponent<Enemy>().GetDamaged(Mathf.CeilToInt(thisDamage), Random.value <= critChance); 
			} else {
				other.GetComponent<Enemy>().GetDamaged(Mathf.FloorToInt(thisDamage), Random.value <= critChance); 
			}
			if (Random.value < attackEffectChance) {
				other.GetComponent<Enemy>().GetDamaged(attackEffect, attackEffectValue, attackEffectTime);
			}
			other.GetComponent<Enemy>().DoKnockback(this.transform.position, knockback);
			if(hitObject != null) {
				GameObject.Instantiate(hitObject, other.gameObject.transform.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(0,FollowPlayer.rotate,0f)));
			}
			if(destroyOnImpact) {
				Destroy(this.gameObject);
			}
		}

		//if we want to damage the player and the other object has a player component
		if (damagePlayer && other.GetComponent<Player>() != null) {

			//current algorithm for what damage will be dealt
			if(Random.value < thisDamage - (int)thisDamage) {
				other.GetComponent<Player>().GetDamaged(Mathf.CeilToInt(thisDamage), Random.value <= critChance); 
			} else {
				other.GetComponent<Player>().GetDamaged(Mathf.FloorToInt(thisDamage), Random.value <= critChance); 
			}

			//Instantiate hitobject if it isn't null
			if(hitObject != null) {
				GameObject.Instantiate(hitObject, other.gameObject.transform.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(0,FollowPlayer.rotate,0f)));
			}

			//destroy if destroyonimpact is true
			if(destroyOnImpact) {
				Destroy(this.gameObject);
			}
		}

		//destroys attack if it collides with anything else (like a wall)
		if (other.GetComponent<Player>() == null && other.GetComponent<Enemy>() == null && destroyOnImpact && !other.name.Equals(this.name)) {
			Destroy(this.gameObject);
		}
	}

//	public void MultiplyDamage(int damageMod) {
//		thisDamage = damageMod*damage;
//	}

	public void SetDamage(float damage) {
		thisDamage = damage;
	}

	public void SetCrit(float critVal) {
		critChance = critVal;
	}

	public float GetDamage() {
		return thisDamage;
	}

}
