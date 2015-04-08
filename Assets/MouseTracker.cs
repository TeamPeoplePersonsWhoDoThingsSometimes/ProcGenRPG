using UnityEngine;
using System.Collections;

public class MouseTracker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit inf = new RaycastHit();
		if(Physics.Raycast(new Ray(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100f),out inf)) {
			float tempDist = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, inf.distance)), Player.playerPos.position);
//			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, inf.distance*10f/20f));
			transform.position = Player.playerPos.position + Player.playerPos.forward*tempDist + Vector3.up/20f;
//			Debug.DrawRay(Player.playerPos.position + Vector3.up, Player.playerPos.forward*10f);
			Debug.Log(tempDist);
		}
	}
}
