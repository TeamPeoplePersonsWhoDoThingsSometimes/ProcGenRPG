using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	private float thisDamage;
	private float knockback;
	private float critChance;
	
	public float duration;
	public bool destroyOnImpact;

	// Use this for initialization
	protected void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Enemy>() != null) {
			if(Random.value < 0.5f) {
				other.GetComponent<Enemy>().GetDamaged(Mathf.CeilToInt(thisDamage), Random.value <= critChance); 
			} else {
				other.GetComponent<Enemy>().GetDamaged(Mathf.FloorToInt(thisDamage), Random.value <= critChance); 
			}
			other.GetComponent<Enemy>().DoKnockback(this.transform.position, knockback);
			if(destroyOnImpact) {
				Destroy(this.gameObject);
			}
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
