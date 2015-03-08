using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnityNPC : Interactable {

	private bool talking = false;

	public string name = "Nobody";
	public string startConvID;

	public Text nameUI1, nameUI2;
	public Text curText;
	public GameObject buttonPrefab;
	public GameObject buttonHolder;

	private uConversation conv;
	private uConversationNode curNode;

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
	}
	
	// Update is called once per frame
	void Update () {
		if (this.CanInteract()) {
			nameUI2.enabled = true;
		} else {
			nameUI2.enabled = false;
		}

		PlayerControl.immobile = talking;

		if(talking) {
			transform.GetChild(0).GetComponent<Camera>().enabled = true;
			transform.GetChild(1).GetComponent<Camera>().enabled = true;
			transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "";
			transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(2).eulerAngles = new Vector3(0,180f,0f);
			PlayerCanvas.cinematicMode = true;
		} else {
			transform.GetChild(0).GetComponent<Camera>().enabled = false;
			transform.GetChild(1).GetComponent<Camera>().enabled = false;
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
