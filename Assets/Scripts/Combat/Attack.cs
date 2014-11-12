using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public float damage;
	protected float thisDamage;

	private float critChance;
	private bool isCrit;
	
	public float duration;
	public bool destroyOnImpact;
	public float knockback;

	// Use this for initialization
	protected void Start () {

	}

	void OnEnable() {
		thisDamage = damage;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Enemy>() != null) {
			other.GetComponent<Enemy>().GetDamaged(thisDamage, Random.value <= critChance);
			other.GetComponent<Enemy>().DoKnockback(this.transform.position, knockback);
			if(destroyOnImpact) {
				Destroy(this.gameObject);
			}
		}
	}

	public void MultiplyDamage(int damageMod) {
		thisDamage = damageMod*damage;
	}

	public void SetCrit(float critVal) {
		critChance = critVal;
	}

}
