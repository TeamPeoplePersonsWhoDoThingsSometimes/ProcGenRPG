using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	private static GameObject[] enemies;

	private static AudioSource normal, combat;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		FindEnemies();
		normal = transform.GetChild(0).GetComponent<AudioSource>();
		combat = transform.GetChild(1).GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
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
		normal.volume = Mathf.MoveTowards(normal.volume, Mathf.Min(1, Mathf.Max(0.25f,tempDist/35f)), 0.01f);
		combat.volume = Mathf.MoveTowards(combat.volume, Mathf.Min(1, Mathf.Max(0.1f,1 - tempDist/35f)), 0.01f);

		if(Time.frameCount % 500 == 0) {
			Debug.Log("HERE" + Time.frameCount);
			FindEnemies();
		}
	}

	public static void FindEnemies() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}
}
