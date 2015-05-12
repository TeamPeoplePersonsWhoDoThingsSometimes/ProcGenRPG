using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityStandardAssets.ImageEffects;
using System.IO;

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
	public Slider volumeSlider,qualitySlider;

	GameObject newChar = null;

	public AudioMixer mixer;

	public Text load1,load2,load3;

	void OnEnable() {
//		float scaleFactor = Screen.height/576f;
//		Screen.SetResolution((int)(Screen.width*scaleFactor),Screen.height,true);
//		Debug.LogError(Screen.resolutions[Screen.resolutions.Length-1].width + " " + Screen.resolutions[Screen.resolutions.Length-1].height);
		if(QualitySettings.names[QualitySettings.GetQualityLevel()].Equals("INSANE")) {
			Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length-1].width,Screen.resolutions[Screen.resolutions.Length-1].height,true);
			Debug.LogError(Screen.width + " " + Screen.height);
		}
	}

	// Use this for initialization
	void Start () {
		dotBG = GameObject.Find("DOTBGPREFAB").GetComponent<Image>();
		splashContainer = GameObject.Find("SplashContainer");
		mainContainer = GameObject.Find("MainContainer");
		chromAbbAmount = Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration;
		volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
		qualitySlider.value = QualitySettings.GetQualityLevel();

		FileStream fs;
		try {
			fs = new FileStream (MasterDriver.saveGameFile1, FileMode.Open);
			load1.text = PlayerPrefs.GetString("Load1","EMPTY");
			fs.Flush();
			fs.Close();
		} catch (IOException excep) {
			load1.text = "EMPTY";
		}
		try {
			fs = new FileStream (MasterDriver.saveGameFile2, FileMode.Open);
			load2.text = PlayerPrefs.GetString("Load2","EMPTY");
			fs.Flush();
			fs.Close();
		} catch (IOException excep) {
			load2.text = "EMPTY";
		}
		try {
			fs = new FileStream (MasterDriver.saveGameFile3, FileMode.Open);
			load3.text = PlayerPrefs.GetString("Load3","EMPTY");
			fs.Flush();
			fs.Close();
		} catch (IOException excep) {
			load3.text = "EMPTY";
		}

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

			// Initialize dictionary of key mappings
			// Was done inside PersistentInfo at first, but kept giving null reference exceptions when trying to reference it here
			// The string values were originally intended to initialize the label for the keys to display the mapping, but since it
			// does not act as a member variable anymore, this does not matter
			Dictionary<KeyCode, string> dict = new Dictionary<KeyCode, string> ();
			dict.Add (PersistentInfo.forwardKey, "Forward");
			dict.Add (PersistentInfo.backKey, "Back");
			dict.Add (PersistentInfo.useKey, "Use");
			dict.Add (PersistentInfo.rollKey, "Roll");
			dict.Add (PersistentInfo.consoleOpenKey, "Console");
			dict.Add (PersistentInfo.rightKey, "Right");
			dict.Add (PersistentInfo.leftKey, "Left");
			dict.Add (PersistentInfo.sprintKey, "Spring");
			dict.Add (PersistentInfo.attackKey, "Attack");
			dict.Add (PersistentInfo.hackKey, "Hack");

			// Check if the requested key is already mapped
			if (temp != KeyCode.None && dict.ContainsKey(temp)) {
				Debug.Log ("That key is already mapped!");
				curButtonForRemap = null;
			}
			else if (temp != KeyCode.None) {
				// Movement keys
				if(curButtonForRemap.gameObject.name.Equals("UpButton")) {
					PersistentInfo.forwardKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("DownButton")) {
					PersistentInfo.backKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("LeftButton")) {
					PersistentInfo.leftKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("RightButton")) {
					PersistentInfo.rightKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("Sprint")) {
					PersistentInfo.sprintKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("Roll")) {
					PersistentInfo.rollKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				// Combat keys
				else if (curButtonForRemap.gameObject.name.Equals("Primary")) {
					PersistentInfo.attackKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				else if (curButtonForRemap.gameObject.name.Equals("Secondary")) {
					PersistentInfo.hackKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				// Etc keys
				else if (curButtonForRemap.gameObject.name.Equals("Console")) {
					PersistentInfo.consoleOpenKey = temp;
					curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				}
				// Didn't see a key for map??
				//else if (curButtonForRemap.gameObject.name.Equals("Map")) {
				//	PersistentInfo.Key = temp;
				//	curButtonForRemap.transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
				//}

				reMapping = false;
				curButtonForRemap = null;
			}
		}

		if(Input.GetMouseButton(1) && newChar != null) {
			Color temp = newChar.GetComponent<Image>().color;
			temp = new Color(temp.r+Time.deltaTime*2f,temp.g-Time.deltaTime*2f, temp.b-Time.deltaTime*2f);
			newChar.GetComponent<Image>().color = temp;
			if(newChar.GetComponent<Image>().color.g <= 0) {
				if(newChar.gameObject.name.Equals("NewCharacter1")) {
					File.Delete(MasterDriver.saveGameFile1);
				} else if (newChar.gameObject.Equals("NewCharacter2")) {
					File.Delete(MasterDriver.saveGameFile2);
				} else {
					File.Delete(MasterDriver.saveGameFile3);
				}
				newChar.GetComponent<Image>().color = Color.white;
				newChar.transform.GetChild(0).GetComponent<Text>().text = "EMPTY";
				newChar = null;
			}
		} else if (newChar != null) {
			newChar.GetComponent<Image>().color = Color.white;
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
		mixer.SetFloat("Master",-80 + (PlayerPrefs.GetFloat("MasterVolume")*68f));
	}

	public void QualitySliderChanged(GameObject slide) {
		QualitySettings.SetQualityLevel((int)slide.GetComponent<Slider>().value, true);
		slide.transform.parent.GetChild(1).GetComponent<Text>().text = QualitySettings.names[QualitySettings.GetQualityLevel()];
	}

	public void NewGameClicked(int saveLoc) {
		PersistentInfo.saveFile = saveLoc;
		try {
			FileStream fs;
			if(saveLoc == 1) {
				fs = new FileStream (MasterDriver.saveGameFile1, FileMode.Open);
			} else if(saveLoc == 2) {
				fs = new FileStream (MasterDriver.saveGameFile2, FileMode.Open);
			} else {
				fs = new FileStream (MasterDriver.saveGameFile3, FileMode.Open);
			}
			fs.Flush();
			fs.Close();
			Application.LoadLevel(3);
		} catch (IOException excep) {
			Application.LoadLevel(1);
		}
	}

	public void CreditsClicked() {
		Application.LoadLevel(4);
	}

	public void KeyRemapButtonPressed(Button button) {
		button.transform.GetChild(0).GetComponent<Text>().text = "?";
		curButtonForRemap = button;
		reMapping = true;
	}

	public void PlayMouseOverFX() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI03", Camera.main.transform.position, PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void PlayClickFX() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI02", Camera.main.transform.position, PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void RightClickHeld(GameObject g) {
		newChar = g;
	}
}
