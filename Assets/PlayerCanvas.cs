using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCanvas : MonoBehaviour {

	private Animator playerAnim;

	private Player playerRef;

	private CanvasGroup inGameGUI;
	private Image bg, a1, a2, cursor;
	private Text byteText;

	private CanvasGroup consoleGUI;
	private Text consoleText;

	private bool inConsole = false;

	// Use this for initialization
	void Start () {
		playerAnim = transform.parent.parent.GetChild(0).GetComponent<Animator>();
		playerRef = transform.parent.parent.GetChild(0).GetComponent<Player>();
		inGameGUI = transform.GetChild(0).GetComponent<CanvasGroup>();
		consoleGUI = transform.GetChild(1).GetComponent<CanvasGroup>();
		consoleText = transform.GetChild(1).GetChild(2).GetComponent<Text>();
		a1 = transform.GetChild(0).GetChild(1).GetComponent<Image>();
		a2 = transform.GetChild(0).GetChild(2).GetComponent<Image>();
		bg = transform.GetChild(0).GetChild(0).GetComponent<Image>();
		cursor = transform.GetChild(0).GetChild(3).GetComponent<Image>();
		byteText = transform.GetChild(0).GetChild(4).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		AnimatorStateInfo info = playerAnim.GetCurrentAnimatorStateInfo(0);
		if (info.IsName("Base.Slash1")) {
			bg.enabled = true;
			a1.enabled = true;
			a2.enabled = false;
			cursor.enabled = true;
			cursor.rectTransform.localPosition = new Vector3((info.normalizedTime)*4 - 2, cursor.rectTransform.localPosition.y, cursor.rectTransform.localPosition.z);
		} else if (info.IsName("Base.Slash2")) {
			bg.enabled = true;
			a1.enabled = false;
			a2.enabled = true;
			cursor.enabled = true;
			cursor.rectTransform.localPosition = new Vector3((info.normalizedTime)*4 - 2, cursor.rectTransform.localPosition.y, cursor.rectTransform.localPosition.z);
		} else {
			bg.enabled = false;
			a1.enabled = false;
			a2.enabled = false;
			cursor.enabled = false;
		}
	}

	void Update () {
		byteText.text = "Bytes: " + Utility.ByteToString(playerRef.GetBytes());

		consoleText.text = playerRef.ToString();

		if(Input.GetKeyDown(KeyCode.BackQuote)) {
			inConsole = !inConsole;
		}

		if(inConsole) {
			inGameGUI.alpha = 0f;
			transform.parent.GetChild(1).camera.enabled = false;
			consoleGUI.alpha = 1f;
		} else {
			inGameGUI.alpha = 1f;
			transform.parent.GetChild(1).camera.enabled = true;
			consoleGUI.alpha = 0f;
		}
	} 
}
 