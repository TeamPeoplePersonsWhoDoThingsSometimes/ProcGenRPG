using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	private float thisDamage;
	private float critChance;

	public bool damagePlayer;
	public bool damageEnemy;
	public float knockback;
	public float duration;
	public bool destroyOnImpact;

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
			other.GetComponent<Enemy>().DoKnockback(this.transform.position, knockback);
		}

		if (damagePlayer && other.GetComponent<Player>() != null) {
			if(Random.value < thisDamage - (int)thisDamage) {
				other.GetComponent<Player>().GetDamaged(Mathf.CeilToInt(thisDamage), Random.value <= critChance); 
			} else {
				other.GetComponent<Player>().GetDamaged(Mathf.FloorToInt(thisDamage), Random.value <= critChance); 
			}
		}
		if(destroyOnImpact) {
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
