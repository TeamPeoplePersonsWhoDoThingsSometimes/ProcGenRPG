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

	public bool spawnERROR;
	private GameObject ERROR;
	private float ERRORTIME;

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
			Cursor.visible = false;
		}
//		GetComponent<FMOD_StudioEventEmitter>().GetEvent().setVolume(PlayerPrefs.GetFloat("MasterVolume"));
//		Debug.Log(PlayerPrefs.GetFloat("MasterVolume"));
//		GetComponent<FMOD_StudioEventEmitter>().StartEvent();
//		text.Replace(

		gameObject.AddComponent<FMOD_StudioEventEmitter>();
		GetComponent<FMOD_StudioEventEmitter>().path = "event:/environment/tutorialWindow";
		GetComponent<FMOD_StudioEventEmitter>().GetEvent().setVolume(PlayerPrefs.GetFloat("MasterVolume"));
		GetComponent<FMOD_StudioEventEmitter>().startEventOnAwake = true;

		if(spawnERROR) {
			ERROR = GameObject.Find("STACKOVERFLOW");
			ERROR.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlayerTouching && isTouchingThis) {
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = Mathf.MoveTowards(Camera.main.GetComponent<ColorCorrectionCurves>().saturation, 0f, Time.deltaTime*5f);
			Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().grainIntensityMax, 0.5f, Time.deltaTime*5f);
			Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax = Mathf.MoveTowards(Camera.main.GetComponent<NoiseAndScratches>().scratchIntensityMax, 0.5f, Time.deltaTime*5f);
			float curFreq = 0f;
			mixer.GetFloat("CutoffFreq", out curFreq);
			mixer.SetFloat("CutoffFreq",Mathf.MoveTowards(curFreq,1000,Time.deltaTime*10000f));
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

		if(ERRORTIME > 0) {
			ERRORTIME += Time.deltaTime;
			if(ERRORTIME > 0.5f) {
				ERROR.SetActive(true);
				text = "/* PRESS %SPRINTKEY% TO RUN */";
				ERRORTIME = 0f;
			}
		}

//		FMOD.Studio.EventDescription desc = null;
//		desc = FMODEditorExtension.GetEventDescription(GetComponent<FMOD_StudioEventEmitter>().asset.id);
//		if(desc != null) {
//			desc.
//		}

		
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.GetComponent<Player>() != null) {
			isPlayerTouching = true;
			isTouchingThis = true;
			if(enableUIAfterTouching) {
				enableUI = true;
			}
			if(spawnERROR) {
				ERRORTIME = 0.001f;
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
			GUI.skin.label.fontSize = (int)(Screen.width/90f);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.DrawTexture(new Rect(pos.x-20f, pos.y, pos.width + 40f, pos.height), textBG);
			GUI.Label(pos, text); 
		}
	}
}
