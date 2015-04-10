using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityStandardAssets.ImageEffects;

public class TutorialComment : MonoBehaviour {

	private static bool isPlayerTouching = false;
	public bool enableUIAfterTouching = false;
	public static bool enableUI = false;
	private bool isTouchingThis = false;
	public string text;
	public Texture textBG;

	public AudioMixer mixer;

	private static GameObject playerCanvas, minimapCam, worldMap, worldMapCam;

	public Font textFont;

	// Use this for initialization
	void Start () {
		if(playerCanvas == null) {
			playerCanvas = GameObject.Find("PlayerCanvas");
			minimapCam = GameObject.Find("MiniMapCam");
			worldMap = GameObject.Find("WorldMapContainer");
			worldMapCam = GameObject.Find("WorldMapCamRotateContainer");
			playerCanvas.SetActive(false);
			minimapCam.SetActive(false);
			worldMap.SetActive(false);
			worldMapCam.SetActive(false);
		}
//		text.Replace(
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(Vector3.Distance(this.transform.position, Player.playerPos.position));
		if(isPlayerTouching && isTouchingThis) {
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = Mathf.MoveTowards(Camera.main.GetComponent<ColorCorrectionCurves>().saturation, 0f, Time.deltaTime*5f);
			Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax, 0.5f, Time.deltaTime*5f);
			Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax, 0.5f, Time.deltaTime*5f);
			float curFreq = 0f;
			mixer.GetFloat("CutoffFreq", out curFreq);
			mixer.SetFloat("CutoffFreq",Mathf.MoveTowards(curFreq,300,Time.deltaTime*10000f));
		} else if (Vector3.Distance(this.transform.position, Player.playerPos.position) < 10 && !isPlayerTouching) {
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = Mathf.MoveTowards(Camera.main.GetComponent<ColorCorrectionCurves>().saturation, 1f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax, 0f, Time.deltaTime*2f);
			Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax, 0f, Time.deltaTime*2f);
			float curFreq = 0f;
			mixer.GetFloat("CutoffFreq", out curFreq);
			mixer.SetFloat("CutoffFreq",Mathf.MoveTowards(curFreq,22000, Time.deltaTime*10000f));
		}

		if(enableUI) {
			playerCanvas.SetActive(true);
			minimapCam.SetActive(true);
			worldMap.SetActive(true);
			worldMapCam.SetActive(true);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.GetComponent<Player>() != null) {
			isPlayerTouching = true;
			isTouchingThis = true;
			if(enableUIAfterTouching) {
				enableUI = true;
			}
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
			Rect pos = new Rect(Screen.width/4f,Screen.height/5f*3f,Screen.width - Screen.width/2f, (Screen.height - Screen.height/5f*3f)/2f);
//			GUIStyle.none.font = textFont;
			GUI.skin.label.font = textFont;
			GUI.skin.label.fontSize = (int)(Screen.width/50f);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.DrawTexture(new Rect(pos.x-20f, pos.y, pos.width + 40f, pos.height), textBG);
			GUI.Label(pos, text); 
		}
	}
}
