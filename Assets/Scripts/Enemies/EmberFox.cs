using UnityEngine;
using System.Collections;

public class EmberFox : Enemy {

	private bool patrolForward = true, turningLeft = false, turningRight = false;
	private float distanceTurned = 0f, distancePatrolled = 0f;

	public bool detectOnSpawn;
	public bool patrol;

	// Use this for initialization
	void Start () {
		base.Start();
		detectedPlayer = detectOnSpawn;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
		if(transform.position.y > 2) {
			transform.position = new Vector3(transform.position.x,2,transform.position.z);
		}
	}

	public void Attack() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/enemy/enemySpinAttack",transform.position, PlayerPrefs.GetFloat("MasterVolume")/2f);
		GameObject temp = (GameObject)Instantiate(enemyAttack, transform.position + new Vector3(0,1f,0), transform.localRotation);
		temp.GetComponent<Attack>().SetDamage(baseAttackDamage);
		temp.GetComponent<Attack>().SetCrit(0.1f);
	}

	protected override void DoIdle ()
	{
//		Debug.DrawRay(transform.position, transform.forward);
		if(patrol) {
			if (Physics.Raycast(transform.position, transform.forward, 3f) || turningLeft || turningRight) {
				if (!turningLeft && !turningRight) {
					patrolForward = false;
					distancePatrolled = 0f;
					turningLeft = Random.value < 0.5f;
					turningRight = !turningLeft;
				}
				if (turningLeft && distanceTurned < 90f) {
					transform.Rotate(new Vector3(0, Time.deltaTime*20f, 0f));
					distanceTurned += Time.deltaTime*20f;
				} else if (turningRight && distanceTurned < 90f) {
					transform.Rotate(new Vector3(0, -Time.deltaTime*20f, 0f));
					distanceTurned += Time.deltaTime*20f;
				} else if (distanceTurned >= 90f && !Physics.Raycast(transform.position, transform.forward, 3f)) {
					distanceTurned = 0f;
					distancePatrolled = 0f;
					turningLeft = false;
					turningRight = false;
					patrolForward = true;
				}
			} else {
				if(patrolForward && distancePatrolled < 5f) {
					transform.Translate(Vector3.Scale(new Vector3(1f, 0f, 1f), transform.forward) * Time.deltaTime * 2f, Space.World);
					distancePatrolled += Time.deltaTime;
				} else if (distancePatrolled >= 5f) {
					patrolForward = false;
					turningLeft = Random.value < 0.5f;
					turningRight = !turningLeft;
					distancePatrolled = 0f;
				}
			}
		}
	}
}
