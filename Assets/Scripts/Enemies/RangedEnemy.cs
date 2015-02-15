using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy {
	public float minDistanceFromPlayer = 10f;
	public float maxDistanceFromPlayer = 15f;

	private bool trackingPlayer;
	private Vector3 lastKnownPlayerPos;

	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}
	
	public void Attack() {
		GameObject temp = (GameObject)Instantiate(enemyAttack, transform.position + new Vector3(0,1f,0), transform.localRotation);
		temp.GetComponent<Attack>().SetDamage(baseAttackDamage);
		temp.GetComponent<Attack>().SetCrit(0.1f);
	}

	protected override void HandleDetectedPlayer() {
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
		if (!retreating) {
			if (Vector3.Distance(Player.playerPos.position, transform.position) > maxDistanceFromPlayer) {
				GetComponent<Animator>().SetTrigger("PlayerSpotted");
				// Get direction to move enemy in, as the direction towards the player
				Vector3 dir = (Player.playerPos.position - transform.position).normalized;
				// Get the value of the separation b/w player & enemy
				float dist = (Player.playerPos.position - transform.position).magnitude;
				// Move enemy towards player, but keep at a distance
				rigidbody.MovePosition(Vector3.MoveTowards(transform.position, dir * (dist - (maxDistanceFromPlayer - minDistanceFromPlayer) / 2), 0.1f));
				transform.LookAt(dir * (dist - (maxDistanceFromPlayer - minDistanceFromPlayer)));
			} else if (Vector3.Distance(Player.playerPos.position, transform.position) > minDistanceFromPlayer) {
				transform.LookAt(Player.playerPos.position);
				tempAttackSpeed -= Time.deltaTime;
				if(tempAttackSpeed <= 0) {
					GetComponent<Animator>().SetTrigger("Attack");
					tempAttackSpeed = baseAttackSpeed;
				}
			} else {
				// Get direction to move enemy in, as the direction away from the player
				Vector3 dir = (transform.position - Player.playerPos.position).normalized;
				// Get the value of the separation b/w player & enemy
				float dist = (transform.position - Player.playerPos.position).magnitude;
				// Move enemy away from player, until within acceptable range
				rigidbody.MovePosition(Vector3.MoveTowards(transform.position, dir * ((maxDistanceFromPlayer - minDistanceFromPlayer) - dist), 0.2f));
				transform.LookAt(Player.playerPos.position);
			}
		} else {
			tempAttackSpeed = baseAttackSpeed;
		}
	}
}
