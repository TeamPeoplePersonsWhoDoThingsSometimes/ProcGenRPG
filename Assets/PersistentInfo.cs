using UnityEngine;
using System.Collections;

public class PersistentInfo : MonoBehaviour {

	public static KeyCode forwardKey = KeyCode.W, backKey = KeyCode.S, useKey = KeyCode.F, rollKey = KeyCode.Space, consoleOpenKey = KeyCode.BackQuote;
	public static KeyCode rightKey = KeyCode.D, leftKey = KeyCode.A, sprintKey = KeyCode.LeftShift, attackKey = KeyCode.Mouse0, hackKey = KeyCode.Mouse1;
	public static string playerName;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
