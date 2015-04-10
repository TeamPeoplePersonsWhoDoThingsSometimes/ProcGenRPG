using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnityNPC : Interactable {

	public bool talking = false;

	public string name = "Nobody";
	public string startConvID;

	public Text nameUI1, nameUI2;
	public Text curText;
	public GameObject buttonPrefab;
	public GameObject buttonHolder;

	private uConversation conv;
	private uConversationNode curNode;

	private bool cityNotbuiltyet = true;

	// Use this for initialization
	void Start () {
//		foreach(uConversation c in LoadResources.Instance.Conversations) {
//			if(c.getName().Equals(name)) {
//				conv = c;
//				break;
//			}
//		}

//		for(int i = 0; i < conv.getNodeList().Count; i++) {
//			uConversationNode node = conv.getNodeList()[i];
//			Debug.Log(i + ". " + node.getText());
//		}

		curNode = uConversationNode.getNodeByStringID(startConvID);
		nameUI1.text = name;
		nameUI2.text = name;
		curText.text = curNode.getText();
		int numResponses = 0;
		foreach(string s in curNode.getAlternativeStrings()) {
			Debug.Log(s);
			if(numResponses > 0) {
				GameObject newButton = (GameObject)Instantiate(buttonPrefab);
				newButton.GetComponent<RectTransform>().SetParent(buttonHolder.transform,false);
				newButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero - new Vector2(0, 18 + (numResponses-1)*55);
				newButton.GetComponent<RectTransform>().localScale = Vector3.one;
				newButton.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
				newButton.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = s;
			} else {
				buttonPrefab.transform.GetChild(0).GetComponent<Text>().text = s;
			}
			numResponses++;
		}

	}

	public void ResponseClicked(GameObject responseChosen) {
//		Debug.Log(curNode.getAlternativeStrings()[responseChosen.transform.GetSiblingIndex()]);
		if(responseChosen.transform.GetChild(0).GetComponent<Text>().text.Equals("Goodbye")) {
			curNode = uConversationNode.getNodeByStringID(startConvID);
			UpdateUI();
			talking = false;
		} else {
			curNode = curNode.GoToAlternative(curNode.getAlternativeStrings()[responseChosen.transform.GetSiblingIndex()]);
			ActionEventInvoker.primaryInvoker.invokeAction(new PlayerAction(curNode.getDirectObject(), ActionType.CONVERSATION_NODE_HIT));
			UpdateUI();
		}
	}

	private void UpdateUI() {
		curText.text = curNode.getText();
		int numResponses = 0;

		//INEFFICIENT
		for(int i = 1; i < buttonHolder.transform.childCount; i++) {
			Destroy(buttonHolder.transform.GetChild(i).gameObject);
		}

		if(curNode.getAlternativeStrings().Count != 0) {
			foreach(string s in curNode.getAlternativeStrings()) {
				if(numResponses > 0) {
					GameObject newButton = (GameObject)Instantiate(buttonPrefab);
					newButton.GetComponent<RectTransform>().SetParent(buttonHolder.transform,false);
					newButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero - new Vector2(0, 18 + (numResponses-1)*55);
					newButton.GetComponent<RectTransform>().localScale = Vector3.one;
					newButton.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
					newButton.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = s;
				} else {
					buttonPrefab.transform.GetChild(0).GetComponent<Text>().text = s;
				}
				numResponses++;
			}
		} else {
			buttonPrefab.transform.GetChild(0).GetComponent<Text>().text = "Goodbye";
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(cityNotbuiltyet && !Player.playerPos.position.Equals(Vector3.zero)) {
			transform.position = Player.playerPos.position + Vector3.forward*2f;
			cityNotbuiltyet = false;
		}

		if (this.CanInteract()) {
			nameUI2.enabled = true;
		} else {
			nameUI2.enabled = false;
		}

		PlayerControl.immobile = talking;

		if(talking) {
			if(transform.GetChild(0).GetComponent<Camera>() != null) {
				transform.GetChild(0).GetComponent<Camera>().enabled = true;
				transform.GetChild(1).GetComponent<Camera>().enabled = true;
				transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
				transform.GetChild(2).eulerAngles = new Vector3(0,180f,0f);
			}
			nameUI2.enabled = false;
			PlayerCanvas.cinematicMode = true;
		} else {
			if(transform.GetChild(0).GetComponent<Camera>() != null) {
				transform.GetChild(0).GetComponent<Camera>().enabled = false;
				transform.GetChild(1).GetComponent<Camera>().enabled = false;
			}
			transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(2).rotation = Quaternion.RotateTowards(transform.GetChild(2).rotation, Quaternion.Euler(new Vector3(transform.GetChild(2).eulerAngles.x, FollowPlayer.rotate, transform.GetChild(2).eulerAngles.z)), Time.deltaTime*50f);
			PlayerCanvas.cinematicMode = false;
		}

	}

	protected override void Interact ()
	{
		talking = !talking;
//		curNode = conv.
		return;
	}
}
