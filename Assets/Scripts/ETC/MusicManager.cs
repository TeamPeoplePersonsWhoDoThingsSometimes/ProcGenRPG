using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	private static GameObject[] enemies;

	private static AudioSource normal, combat;


	private static bool fadeOut = false;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		FindEnemies();
		normal = transform.GetChild(0).GetComponent<AudioSource>();
		if(transform.childCount > 1) {
			combat = transform.GetChild(1).GetComponent<AudioSource>();
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(combat != null) {
			GameObject closestEnemy;
			float tempDist = 1000000000f;
			foreach(GameObject g in enemies) {
				if(g != null) {
					if(Vector3.Distance(Player.playerPos.position, g.transform.position) < tempDist) {
						closestEnemy = g;
						tempDist = Vector3.Distance(Player.playerPos.position, g.transform.position);
					}
				}
			}
			if(!fadeOut) {
				normal.volume = Mathf.MoveTowards(normal.volume, Mathf.Min(0.4f, Mathf.Max(0.1f,tempDist/35f)), 0.01f);
				combat.volume = Mathf.MoveTowards(combat.volume, Mathf.Min(0.7f, Mathf.Max(0f,1 - tempDist/35f)), 0.01f);
			} else {
				if(normal.volume > 0) {
					normal.volume -= Time.deltaTime;
				}
				if(combat.volume > 0) {
						combat.volume -= Time.deltaTime;
				}
			}

			if(Time.frameCount % 500 == 0) {
				FindEnemies();
			}
		}
		if(fadeOut && normal.volume > 0) {
			normal.volume -= Time.deltaTime;
		}
	}

	public static void FindEnemies() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}

	public static void FadeOutAudio() {
		fadeOut = true;
	}
}
