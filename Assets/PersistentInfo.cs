using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentInfo : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F, rollKey = KeyCode.Space, consoleOpenKey = KeyCode.BackQuote, interactKey = KeyCode.F;
	public static KeyCode rightKey = KeyCode.D, leftKey = KeyCode.A, sprintKey = KeyCode.LeftShift, attackKey = KeyCode.Mouse0, hackKey = KeyCode.Mouse1, mapKey = KeyCode.M;
	public static string playerName;
	public static int saveFile = 0;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
