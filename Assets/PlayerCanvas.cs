using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCanvas : MonoBehaviour {

	private Animator playerAnim;

	private Image bg, a1, a2, cursor;

	// Use this for initialization
	void Start () {
		playerAnim = transform.parent.parent.GetChild(0).GetComponent<Animator>();
		a1 = transform.GetChild(1).GetComponent<Image>();
		a2 = transform.GetChild(2).GetComponent<Image>();
		bg = transform.GetChild(0).GetComponent<Image>();
		cursor = transform.GetChild(3).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		AnimatorStateInfo info = playerAnim.GetCurrentAnimatorStateInfo(0);
		Debug.Log(info.normalizedTime);
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
}
