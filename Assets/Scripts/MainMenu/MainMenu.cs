using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	private enum MenuState {Splash, Main, LoadSave, Options, Credits};

	private Image dotBG;
	private int dotBGCounter1;
	private MenuState curState = MenuState.Splash;

	// Use this for initialization
	void Start () {
		dotBG = GameObject.Find("DOTBGPREFAB").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		switch(curState) {

		case MenuState.Splash:
			if(dotBGCounter1 < 25) {
				for(int i = 0; i < 33; i++) {
					Image temp = (Image)Instantiate(dotBG,dotBG.transform.position, Quaternion.identity);
					temp.transform.parent = dotBG.transform.parent;
					temp.rectTransform.anchoredPosition = new Vector2(i*25 - 400, 300 - dotBGCounter1*25);
					temp.enabled = true;
				}
				dotBGCounter1++;
			} else {

			}
			break;


		}
	}

	void TransitionToMain() {

	}
}
