using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * ThronePls can hold subclasses of Item
 */
public class ThronePls : Interactable {

	private bool sit = false;
	// Use this for initialization
	void Start () {
	
	}


	protected override void Interact() {
		if (Player.useKey != KeyCode.None && Input.GetKeyDown(Player.useKey)) {

		}
	}


	// Update is called once per frame
	void Update () {
		if(player != null)
			transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, Quaternion.Euler(new Vector3(transform.GetChild(0).eulerAngles.x, player.transform.eulerAngles.y, transform.GetChild(0).eulerAngles.z)), Time.deltaTime*50f);
		if (this.CanInteract()) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Press " + Player.useKey + " to sit";
		} else {
			transform.GetChild(0).gameObject.SetActive(false);

		}
	}
}
