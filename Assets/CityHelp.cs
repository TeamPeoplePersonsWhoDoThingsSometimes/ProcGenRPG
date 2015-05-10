using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CityHelp : MonoBehaviour {

	public static int helpMode;
	public bool tutorialLevel;

	// Use this for initialization
	void Start () {
		if(tutorialLevel) {
			helpMode = -1;
		} else {
			helpMode = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(this.GetComponent<Text>() != null) {
			switch(helpMode) {
				case -2:
					this.GetComponent<Text>().text = "If an enemy is too strong, just run!";
					break;
				case -1:
					this.GetComponent<Text>().text = "";
					break;
				case 0:
					this.GetComponent<Text>().text = "Talk to NetExplorer";
					break;
				case 1:
					this.GetComponent<Text>().text = "Press '~' or 'P' to open the Console";
					break;
				case 2:
					this.GetComponent<Text>().text = "Click on 'Quest' to see details about your quests";
					break;
				case 3:
					this.GetComponent<Text>().text = "Go through the portals near NetExplorer to explore the overworld";
					break;
				case 4:
					this.GetComponent<Text>().text = "Use the map (M) to navigate back to the yellow block, the location of the city";
					break;
				case 5:
					this.GetComponent<Text>().text = "Talk to NetExplorer to finish your first quest!";
					break;
				case 6:
					this.GetComponent<Text>().text = "You got a weapon from NetExplorer, open your console and go to your inventory";
					break;
				case 7:
					this.GetComponent<Text>().text = "Here, you can drag and drop items to re-arrange, and hold right click to delete";
					break;
				case 8:
					this.GetComponent<Text>().text = "Find Norton in the city, your minimap may help...";
					break;
			}
		}
	}
}
