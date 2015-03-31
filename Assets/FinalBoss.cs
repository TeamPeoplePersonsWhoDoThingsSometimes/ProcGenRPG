using UnityEngine;
using System.Collections;

public class FinalBoss : Boss {

	float audioTime = 0f;
	AudioSource audioRef;

	// Use this for initialization
	void Start () {
		Player p = GameObject.Find("PlayerObj").GetComponent<Player>();
		name = "Corrupted " + p.GetName().Substring(0,p.GetName().Length-6);
		base.Start();
		audioRef = GameObject.Find("Base").GetComponent<AudioSource>();
		PlayerControl.immobile = true;
		detectedPlayer = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.N) && Time.frameCount % 5 == 0) {
			audioRef.time++;
		}
		if (audioRef.time < 59) {
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*10f,0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*10f,0), Space.World);
		} else if(audioRef.time < 73) {
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*(10f+((audioRef.time - 59)*50f)),0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*(10f+((audioRef.time - 59)*50f)),0), Space.World);

			transform.GetChild(0).GetChild(4).Translate(new Vector3(Time.deltaTime, 0f, 0f), Space.World);
			transform.GetChild(0).GetChild(5).Translate(new Vector3(0f, 0f, Time.deltaTime), Space.World);
			if(Vector3.Distance(transform.GetChild(0).GetChild(6).position, transform.GetChild(0).position) < Vector3.Distance(transform.GetChild(0).GetChild(5).position, transform.GetChild(0).position)) {
				transform.GetChild(0).GetChild(6).Translate(new Vector3(-Time.deltaTime, 0f, -Time.deltaTime), Space.World);
			}

		} else {
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*200,0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*200,0), Space.World);
			transform.GetChild(0).GetChild(4).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);
			transform.GetChild(0).GetChild(5).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);
			transform.GetChild(0).GetChild(6).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);

//			Quaternion lookRot = Quaternion.LookRotation(Player.playerPos.position - this.transform.position, Vector3.up);
//			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, Time.deltaTime*2f);

//			transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.MoveTowardsAngle(transform.eulerAngles.y, 

//			if(audioRef.clip.length - audioRef.time < 0.5f) {
//				audioRef.time = 73.5f;
//			}

			PlayerControl.immobile = false;

			if(audioRef.time > 103.385f) {
				audioRef.time = 73.846f;
			}

			base.Update();
		}

		transform.GetChild(0).LookAt(Player.playerPos);
		transform.GetChild(0).eulerAngles = new Vector3(0f, transform.GetChild(0).eulerAngles.y, 0f);
	}

	protected override void DoIdle ()
	{

	}

	protected override void HandleDeath ()
	{

	}

	protected override void HandleDetectedPlayer ()
	{

	}

	protected override void HandleEffect ()
	{
		base.HandleEffect ();
	}

	protected override void HandleKnockback ()
	{

	}

	protected override void HandlePlayerDetection ()
	{
		detectedPlayer = true;
	}

	public override void PhaseAttack (string phaseName, GameObject phaseObject)
	{
		if(phaseName.Equals("SpawnMemLeaks")) {
			int numToSpawn = (int)(Random.value*4f) + 5;
			for(int i = 0; i < numToSpawn; i++) {
				Instantiate(phaseObject, transform.GetChild(0).position + new Vector3(Random.value*30f - 15,-4f,Random.value*30f - 15), Quaternion.identity);
            }
		}
	}
}
