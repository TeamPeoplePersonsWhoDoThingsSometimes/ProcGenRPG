using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	private enum MenuState {Splash, Main, LoadSave, Options, Credits};
	private bool transitioningToMain;

	private Image dotBG;
	private int dotBGCounter1;
	private MenuState curState = MenuState.Splash;
	private GameObject splashContainer, mainContainer;

	// Use this for initialization
	void Start () {
		dotBG = GameObject.Find("DOTBGPREFAB").GetComponent<Image>();
		splashContainer = GameObject.Find("SplashContainer");
		mainContainer = GameObject.Find("MainContainer");
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
			if(Camera.main.GetComponent<ChromaticAbberation>().scale > 2) {
				Camera.main.GetComponent<ChromaticAbberation>().scale--;
			} else {
				Camera.main.GetComponent<ChromaticAbberation>().scale = 0.3f;
			}
			break;


		}
	}

	void TransitionToMain() {
		Camera.main.GetComponent<ChromaticAbberation>().scale++;
		if(Camera.main.GetComponent<ChromaticAbberation>().scale > 30) {
			curState = MenuState.Main;
		}
	}
}
