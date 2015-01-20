using UnityEngine;
using System.Collections;

public class EmberFoxGenerator : Enemy {

	public GameObject emberfoxprefab;

	// Use this for initialization
	void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();

	}

	public void Attack() {
		GameObject temp = (GameObject)Instantiate(emberfoxprefab, transform.position + transform.forward*2f, transform.localRotation);
	}

	protected override void HandleDeath ()
	{
		base.HandleDeath ();
	}

	protected override void HandleDetectedPlayer ()
	{
		Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 10f);
		foreach(Collider c in nearbyColliders) {
			if(c.gameObject.GetComponent<Enemy>() != null) {
				c.gameObject.GetComponent<Enemy>().AlertEnemy();
			}
		}

		RaycastHit hitinfo = new RaycastHit();
		if(Vector3.Distance(Player.playerPos.position, transform.position) > 50f
		   && !Physics.Raycast(transform.position, transform.forward,out hitinfo, 100f)
		   && hitinfo.collider != null && hitinfo.collider.gameObject != null
		   && hitinfo.collider.gameObject.tag.Equals("Player")) {
			detectedPlayer = false;
		}

		if (Vector3.Distance(Player.playerPos.position, transform.position) <= 30f) {
			tempAttackSpeed -= Time.deltaTime;
			if(tempAttackSpeed <= 0) {
				Attack();
				tempAttackSpeed = baseAttackSpeed;
			}
		} else {
			tempAttackSpeed = baseAttackSpeed;
		}
	}

	protected override void HandleKnockback ()
	{
		return;
	}

	protected override void HandlePlayerDetection ()
	{
		Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 30f);
		foreach(Collider c in nearbyColliders) {
			if(c.gameObject.tag.Equals("Player")) {
				detectedPlayer = true;
			}
		}
	}
}
