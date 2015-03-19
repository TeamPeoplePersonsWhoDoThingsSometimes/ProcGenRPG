using UnityEngine;
using System.Collections;

public class TutorialBlock : MonoBehaviour {

	public Enemy killEnemyToAdvance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(killEnemyToAdvance == null && transform.localPosition.y > -5) {
			transform.Translate(new Vector3(0,-Time.deltaTime*10f, 0));
		}
	}
}
