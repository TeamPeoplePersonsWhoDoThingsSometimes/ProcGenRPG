using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class PlayerHidden : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit inf = new RaycastHit();
		if(Physics.Linecast(this.transform.position, Player.playerPos.position + new Vector3(0,2f,0), out inf)) {
//			Debug.Log(inf.collider.gameObject.name);
			if(!inf.collider.tag.Equals("Player")) {
				this.transform.GetChild(0).GetComponent<Camera>().enabled = true;
				this.transform.GetChild(0).GetComponent<ColorCorrectionCurves>().saturation = this.GetComponent<ColorCorrectionCurves>().saturation;
			} else {
				this.transform.GetChild(0).GetComponent<Camera>().enabled = false;
			}
		}

		if(TutorialComment.enableUI) {
			this.transform.GetChild(0).GetComponent<VignetteAndChromaticAberration>().enabled = false;
		} else {
			this.transform.GetChild(0).GetComponent<VignetteAndChromaticAberration>().enabled = true;
		}
	}
}
