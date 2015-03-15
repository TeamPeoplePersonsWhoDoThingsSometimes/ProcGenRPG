using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class TutorialComment : MonoBehaviour {

	private static bool isPlayerTouching = false;
	private bool isTouchingThis = false;
	public string text;

	public Font textFont;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlayerTouching && isTouchingThis) {
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = Mathf.MoveTowards(Camera.main.GetComponent<ColorCorrectionCurves>().saturation, 0f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax, 0.5f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax, 0.5f, Time.deltaTime*2f);
		} else if (!isPlayerTouching) {
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = Mathf.MoveTowards(Camera.main.GetComponent<ColorCorrectionCurves>().saturation, 1f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax, 0f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax, 0f, Time.deltaTime*2f);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.GetComponent<Player>() != null) {
			isPlayerTouching = true;
			isTouchingThis = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.gameObject.GetComponent<Player>() != null) {
			isPlayerTouching = false;
			isTouchingThis = false;
		}
	}

	void OnGUI() {
		if(isTouchingThis) {
//			GUIStyle.none.font = textFont;
			GUI.skin.label.font = textFont;
			GUI.skin.label.fontSize = (int)(Screen.width/50f);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(Screen.width/4f,Screen.height/3f*2f,Screen.width - Screen.width/2f, 100f), text); 
		}
	}
}
