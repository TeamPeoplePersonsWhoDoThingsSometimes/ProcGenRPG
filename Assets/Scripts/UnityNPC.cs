using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnityNPC : Interactable {

	private bool talking = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, Quaternion.Euler(new Vector3(transform.GetChild(0).eulerAngles.x, FollowPlayer.rotate, transform.GetChild(0).eulerAngles.z)), Time.deltaTime*50f);
		if (this.CanInteract()) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Press " + Player.useKey + " to talk";
		} else {
			transform.GetChild(0).gameObject.SetActive(false);
		}

	}

	protected override void Interact ()
	{
		talking = !talking;
		return;
	}
}
