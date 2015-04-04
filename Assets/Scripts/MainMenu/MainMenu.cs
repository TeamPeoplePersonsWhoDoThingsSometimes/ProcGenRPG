using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class MainMenu : MonoBehaviour {

	private enum MenuState {Splash, Main, LoadSave, Options, Credits};
	private bool transitioningToMain;

	private Image dotBG;
	private int dotBGCounter1;
	private MenuState curState = MenuState.Splash;
	private GameObject splashContainer, mainContainer;

	private float chromAbbAmount;

	private bool reMapping;
	private Button curButtonForRemap;

	// Use this for initialization
	void Start () {
		dotBG = GameObject.Find("DOTBGPREFAB").GetComponent<Image>();
		splashContainer = GameObject.Find("SplashContainer");
		mainContainer = GameObject.Find("MainContainer");
		chromAbbAmount = Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration;
	}
	
	// Update is called once per frame
	void Update () {
		switch(curState) {

		case MenuState.Splash:
			if(Time.frameCount % 2 == 0) {
				if(dotBGCounter1 < 25) {
					for(int i = 0; i < 33; i++) {
						Image temp = (Image)Instantiate(dotBG,dotBG.transform.position, Quaternion.identity);
						temp.rectTransform.SetParent(dotBG.transform.parent);
						temp.rectTransform.anchoredPosition = new Vector2(i*25 - 400, 300 - dotBGCounter1*25);
						temp.enabled = true;
					}
					dotBGCounter1++;
				}
			}
			if(dotBGCounter1 >= 25) {
				if(Input.anyKey) {
					transitioningToMain = true;
				}
				if(transitioningToMain) {
					TransitionToMain();
				}
			}
			mainContainer.SetActive(false);
			splashContainer.SetActive(true);
			break;
		case MenuState.Main:
			mainContainer.SetActive(true);
			splashContainer.SetActive(false);
			if(chromAbbAmount > 5) {
				chromAbbAmount -= Time.deltaTime*5000;
			} else {
				chromAbbAmount = 5;
			}
			break;


		}

		if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
			Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = chromAbbAmount;
		}

		if(reMapping) {
			KeyCode temp = KeyCode.None;
			foreach(KeyCode k in (KeyCode[])System.Enum.GetValues(typeof(KeyCode))) {
				if(Input.GetKey(k)) {
					temp = k;
				}
			}
			if(temp != KeyCode.None) {
				if(curButtonForRemap.gameObject.name.Equals("UpButton")) {
					PersistentInfo.forwardKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				reMapping = false;
				curButtonForRemap = null;
			}
		}
	}

	void TransitionToMain() {
		if(chromAbbAmount < 1000) {
			chromAbbAmount += Time.deltaTime*10000;
		} else {
			curState = MenuState.Main;
			transitioningToMain = false;
		}
	}

	public void PlayPressed() {
		GetComponent<Animator>().SetTrigger("GoToLoad");
	}

	public void BackButtonOnLoad() {
		GetComponent<Animator>().SetTrigger("LoadToMain");
	}

	public void OptionsPressed() {
		GetComponent<Animator>().SetTrigger("GoToOptions");
	}

	public void ControlsPressed() {
		GetComponent<Animator>().SetTrigger("GoToControls");
	}

	public void BackButtonOnOptions() {
		GetComponent<Animator>().SetTrigger("OptionsToMain");
	}

	public void BackButtonOnControls() {
		GetComponent<Animator>().SetTrigger("GoToOptions");
	}

	public void VolumeSliderChanged(GameObject slide) {
		PlayerPrefs.SetFloat("MasterVolume", slide.GetComponent<Slider>().value);
		slide.transform.parent.GetChild(1).GetComponent<Text>().text = ((int)(PlayerPrefs.GetFloat("MasterVolume")*100)).ToString() + "%";
	}

	public void QualitySliderChanged(GameObject slide) {
		QualitySettings.SetQualityLevel((int)slide.GetComponent<Slider>().value, true);
		slide.transform.parent.GetChild(1).GetComponent<Text>().text = QualitySettings.names[QualitySettings.GetQualityLevel()];
	}

	public void NewGameClicked() {
		Application.LoadLevel(1);
	}

	public void CreditsClicked() {
		GetComponent<Animator>().SetTrigger("MainToCredits");
	}

	public void KeyRemapButtonPressed(Button button) {
		button.transform.GetChild(0).GetComponent<Text>().text = "?";
		curButtonForRemap = button;
		reMapping = true;
	}
}
