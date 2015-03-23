using UnityEngine;
using System.Collections;

public class ElkCloner : Boss {
	
	private float smallAttackingTime;
	private GameObject smallAttack;

	private float angryTime;

	protected override void DoIdle () {
		base.DoIdle ();
	}

	protected override void HandleDeath () {
		if (maxHP > 500f) {
			GameObject elk1 = (GameObject)Instantiate(this.gameObject, transform.position + transform.right, Quaternion.identity);
			GameObject elk2 = (GameObject)Instantiate(this.gameObject, transform.position - transform.right, Quaternion.identity);
			elk1.GetComponent<Enemy>().maxHP = this.maxHP/5f;
			elk2.GetComponent<Enemy>().maxHP = this.maxHP/5f;
			elk1.transform.localScale = this.transform.localScale/1.4f;
			elk2.transform.localScale = this.transform.localScale/1.4f;
			elk1.GetComponent<Enemy>().baseAttackDamage = 1f;
			elk2.GetComponent<Enemy>().baseAttackDamage = 1f;
			elk1.GetComponent<Enemy>().maxVersion = Utility.IntToVersion(Utility.VersionToInt(this.version) - 10);
			elk2.GetComponent<Enemy>().maxVersion = Utility.IntToVersion(Utility.VersionToInt(this.version) - 10);
			elk1.GetComponent<Enemy>().minVersion = elk1.GetComponent<Enemy>().maxVersion;
			elk2.GetComponent<Enemy>().minVersion = elk2.GetComponent<Enemy>().maxVersion;
		}
		base.HandleDeath();
	}

	protected void Update() {
		base.Update();

		if(!GetComponent<Rigidbody>().constraints.Equals(RigidbodyConstraints.FreezeAll)) {
			GetComponent<Rigidbody>().AddForce(Vector3.down*1000f);
		}
	
		if(smallAttackingTime > 0) {
			if(Time.frameCount % 10 == 0) {
				GameObject temp = (GameObject)Instantiate(smallAttack, transform.position + transform.forward*Random.value*5f + transform.right*Random.value*5f, Quaternion.identity);
				temp.GetComponent<Attack>().SetDamage(this.baseAttackDamage);
			}
			smallAttackingTime -= Time.deltaTime;
		}
	}

	protected override void HandleDetectedPlayer () {
		transform.LookAt(Player.playerPos.position + new Vector3(0,2,0));
		transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
	}

	protected override void HandleEffect ()
	{
		base.HandleEffect ();
	}

	protected override void HandleKnockback ()
	{
		base.HandleKnockback ();
	}

	protected override void HandlePlayerDetection ()
	{
		Collider[] localColliders = Physics.OverlapSphere(transform.position, 50f);
		foreach(Collider c in localColliders) {
			if(c.GetComponent<Player>() != null) {
				detectedPlayer = true;
			}
		}
	}

	public override void PhaseAttack (string phaseName, GameObject phaseObject)
	{
		if (phaseName.Equals("SmallAttack")) {
			smallAttack = phaseObject;
			smallAttackingTime = 0.5f;
		}
	}

	public override void PhaseMove (string phaseName)
	{
		
	}

	public override void PhaseSpawn (string phaseName, GameObject phaseObject)
	{
		
	}

	public override void PhaseOther (string phaseName, GameObject phaseObject)
	{
		if(phaseName.Equals("GetMad")) {

		}
	}

	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag.Equals("Ground")) {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
	}
}
