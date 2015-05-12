using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class FinalBoss : Boss {

	float audioTime = 0f;
	AudioSource audioRef;
	GameObject bossCam;

	float postDeath = 0f;
	float dyingTime = 0f;

	bool readyToFall = false, fallen = false;
	float fallentime = 0f;
	List<Enemy> spawnedMemLeaks;
	public static int memLeaksCount;

	Player playerref;

	// Use this for initialization
	void Start () {
		playerref = GameObject.Find("PlayerObj").GetComponent<Player>();
		name = "Corrupted " + playerref.name;
		base.Start();
		audioRef = GameObject.Find("Base").GetComponent<AudioSource>();
		PlayerControl.immobile = true;
		detectedPlayer = true;
		bossCam = GameObject.Find("BOSSCAM");
		spawnedMemLeaks = new List<Enemy>();
	}
	
	// Update is called once per frame
	void Update () {
		if (audioRef.time < 59) {
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*10f,0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*10f,0), Space.World);
			if(Input.GetKey(KeyCode.N) && Time.frameCount % 5 == 0) {
				audioRef.time++;
			}
			audioRef.time = 59;
		} else if(audioRef.time < 73) {
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*(10f+((audioRef.time - 59)*50f)),0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*(10f+((audioRef.time - 59)*50f)),0), Space.World);

			transform.GetChild(0).GetChild(4).Translate(new Vector3(Time.deltaTime, 0f, 0f), Space.World);
			transform.GetChild(0).GetChild(5).Translate(new Vector3(0f, 0f, Time.deltaTime), Space.World);
			if(Vector3.Distance(transform.GetChild(0).GetChild(6).position, transform.GetChild(0).position) < Vector3.Distance(transform.GetChild(0).GetChild(5).position, transform.GetChild(0).position)) {
				transform.GetChild(0).GetChild(6).Translate(new Vector3(-Time.deltaTime, 0f, -Time.deltaTime), Space.World);
			}

		} else {
			Destroy(bossCam);
			transform.GetChild(0).GetChild(2).Rotate(new Vector3(0,Time.deltaTime*200,0), Space.World);
			transform.GetChild(0).GetChild(3).Rotate(new Vector3(0,-Time.deltaTime*200,0), Space.World);
			if(fallen) {
				if(transform.GetChild(0).GetChild(4).position.y > -0.3f) {
					transform.GetChild(0).GetChild(4).position -= Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(4).GetComponent<BoxCollider>().enabled = true;

					transform.GetChild(0).GetChild(5).position -= Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(5).GetComponent<BoxCollider>().enabled = true;

					transform.GetChild(0).GetChild(6).position -= Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(6).GetComponent<BoxCollider>().enabled = true;
				}
				fallentime += Time.deltaTime;
			} else {
				if(transform.GetChild(0).GetChild(4) != null && transform.GetChild(0).GetChild(4).localPosition.y < 0f) {
					transform.GetChild(0).GetChild(4).position += Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(4).GetComponent<BoxCollider>().enabled = true;

					transform.GetChild(0).GetChild(5).position += Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(5).GetComponent<BoxCollider>().enabled = true;

					transform.GetChild(0).GetChild(6).position += Vector3.up*Time.deltaTime*10f;
					transform.GetChild(0).GetChild(6).GetComponent<BoxCollider>().enabled = true;
				}
				transform.GetChild(0).GetChild(4).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);
				transform.GetChild(0).GetChild(5).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);
				transform.GetChild(0).GetChild(6).RotateAround(transform.GetChild(0).position, Vector3.up, Time.deltaTime*20f);
			}

			if(fallentime > 10f) {
				fallen = false;
				fallentime = 0f;
			}

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

			if(readyToFall) {
				bool gonnaFall = true;
				memLeaksCount = 0;
				for(int i = 0; i < spawnedMemLeaks.Count; i++) {
					if(spawnedMemLeaks[i] != null) {
						gonnaFall = false;
						memLeaksCount++;
					}
				}
				if(gonnaFall) {
					readyToFall = false;
					fallen = true;
				}
			}

			if(dyingTime > 0f) {
				MusicManager.FadeOutAudio();
				dyingTime -= Time.deltaTime;
				if(dyingTime <= 0) {
					transform.GetChild(0).gameObject.SetActive(false);
				}
			} else {
				postDeath += Time.deltaTime;
				if(postDeath < 2 && postDeath > 1) {
					playerref.GetDamaged(20000);
				}
			}

			base.Update();
		}

		if(!fallen) {
			transform.GetChild(0).LookAt(Player.playerPos);
		}
		transform.GetChild(0).eulerAngles = new Vector3(0f, transform.GetChild(0).eulerAngles.y, 0f);
	}

	protected override void DoIdle ()
	{

	}

	protected override void HandleDeath ()
	{
		if(dyingTime == 0f) {
			transform.GetChild(1).gameObject.SetActive(true);
			dyingTime = 8.5f;
			FollowPlayer.FinalBossDying();
		}
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
		if(phaseName.Equals("SpawnMemLeaks") && !fallen && dyingTime == 0f) {
			readyToFall = true;
			int numToSpawn = (int)(Random.value*4f) + 5;
			for(int i = 0; i < numToSpawn; i++) {
				spawnedMemLeaks.Add(((GameObject)GameObject.Instantiate(phaseObject, new Vector3(transform.GetChild(0).position.x + Random.value*30f - 15,-3f,transform.GetChild(0).position.z + Random.value*30f - 15), Quaternion.identity)).GetComponent<Enemy>());
            }
		}
	}
}
