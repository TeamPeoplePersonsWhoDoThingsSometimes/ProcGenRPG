using UnityEngine;
using System.Collections;

public class PlayerTravelScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("enter");
		if(other.tag.Equals("Portal")) {
			Tile t = other.GetComponent<Tile>();
			Debug.Log("Portal");
			if(t.name.Equals("UpPortal")) {
				Debug.Log("up");
				World.TravelUp();
			} else if (t.name.Equals("DownPortal")) {
				Debug.Log("down");
				World.TravelDown();
			} else if (t.name.Equals("RightPortal")) {
				Debug.Log("right");
				World.TravelRight();
			} else if (t.name.Equals("LeftPortal")) {
				Debug.Log("left");
				World.TravelLeft();
			}
		}
	}

}
