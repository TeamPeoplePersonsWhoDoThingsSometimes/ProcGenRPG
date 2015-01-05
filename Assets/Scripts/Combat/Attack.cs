using UnityEngine;
using System.Collections;

public enum Effect {
	None,
	Deteriorating,
	Slow,
	Stun
}

public class Attack : MonoBehaviour {

	private float thisDamage;
	private float critChance;

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
	protected void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (damageEnemy && other.GetComponent<Enemy>() != null) {
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

		if (damagePlayer && other.GetComponent<Player>() != null) {
			if(Random.value < thisDamage - (int)thisDamage) {
				other.GetComponent<Player>().GetDamaged(Mathf.CeilToInt(thisDamage), Random.value <= critChance); 
			} else {
				other.GetComponent<Player>().GetDamaged(Mathf.FloorToInt(thisDamage), Random.value <= critChance); 
			}
			if(hitObject != null) {
				GameObject.Instantiate(hitObject, other.gameObject.transform.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(0,FollowPlayer.rotate,0f)));
			}
			if(destroyOnImpact) {
				Destroy(this.gameObject);
			}
		}
		if (other.GetComponent<Player>() == null && other.GetComponent<Enemy>() == null && destroyOnImpact) {
			Destroy(this.gameObject);
		}
	}

//	public void MultiplyDamage(int damageMod) {
//		thisDamage = damageMod*damage;
//	}

	public void SetDamage(int damage) {
		thisDamage = damage;
	}

	public void SetCrit(float critVal) {
		critChance = critVal;
	}

	public void SetDamage(float val) {
		thisDamage = val;
	}

}
