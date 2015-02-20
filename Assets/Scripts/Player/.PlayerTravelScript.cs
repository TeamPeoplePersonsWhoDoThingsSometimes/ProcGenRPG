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
		if(other.tag.Equals("Portal")) {
			Tile t = other.GetComponent<Tile>();
			if(t.name.Equals("UpPortal")) {
				World.TravelUp();
			} else if (t.name.Equals("DownPortal")) {
				World.TravelDown();
			} else if (t.name.Equals("RightPortal")) {
				World.TravelRight();
			} else if (t.name.Equals("LeftPortal")) {
				World.TravelLeft();
			}
		}
	}

}
