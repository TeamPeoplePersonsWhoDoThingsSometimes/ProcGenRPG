using UnityEngine;
using System.Collections;

public class Explosion : Attack {
	public int radius;
	public bool canDamagePlayer;
	public bool canDamageEnemy;

	private Collider[] enemyHitArray;


	protected override void Update() {
		duration -= Time.deltaTime;
		transform.Translate(new Vector3(0f, 0f, 50*Time.deltaTime));
		if (duration <= 0) {
			enemyHitArray = Physics.OverlapSphere(transform.position, radius);

			Debug.Log ("Exploding");

			if (enemyHitArray.Length != 0) {
				Debug.Log ("Explosion collided with " + enemyHitArray.Length + " enemies.");
			}

			for(int i = 0; i < enemyHitArray.Length; i++) {
				if(enemyHitArray[i].gameObject.GetComponent<Player>() != null && canDamagePlayer) {
					enemyHitArray[i].gameObject.GetComponent<Player>().GetDamaged(base.GetDamage(),false);
					//player.knockback();
					Debug.Log("Explosion: DamagePlayer"); 
				} else if(enemyHitArray[i].gameObject.GetComponent<Enemy>() != null && canDamageEnemy) {
					enemyHitArray[i].gameObject.GetComponent<Enemy>().GetDamaged(base.GetDamage(),false);
					//enemy.DoKnockback();
					Debug.Log("Explosion: DamageEnemy");
				}
			}
			Destroy(this.gameObject);
		}
	}

	public override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
	}
}