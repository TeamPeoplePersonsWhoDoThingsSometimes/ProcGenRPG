using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentInfo : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F, rollKey = KeyCode.Space, consoleOpenKey = KeyCode.BackQuote, interactKey = KeyCode.F;
	public static KeyCode rightKey = KeyCode.D, leftKey = KeyCode.A, sprintKey = KeyCode.LeftShift, attackKey = KeyCode.Mouse0, hackKey = KeyCode.Mouse1;
	public static Dictionary<KeyCode, string> dict;

	public static string playerName;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		
		// Initialize dictionary of keys
		dict = new Dictionary<KeyCode, string> ();
		dict.Add (forwardKey, "W");
		dict.Add (backKey, "S");
		dict.Add (useKey, "F");
		dict.Add (rollKey, "<Space>");
		dict.Add (consoleOpenKey, "'");
		dict.Add (rightKey, "D");
		dict.Add (leftKey, "A");
		dict.Add (sprintKey, "LSHIFT");
		dict.Add (attackKey, "LMOUSE");
		dict.Add (hackKey, "RMOUSE");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
