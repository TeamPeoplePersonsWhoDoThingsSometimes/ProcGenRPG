using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchText : MonoBehaviour {

	private string defaultText;
	public float glitchiness;

	// Use this for initialization
	void Start () {
		defaultText = transform.GetChild(0).GetComponent<Text>().text;
	}
	
	// Update is called once per frame
	void Update () {
		int randTime = (int)(Random.Range(20,100)*(1/glitchiness));
		if(transform.childCount == 2) {
			transform.GetChild(1).GetComponent<Text>().color = new Color(0, 0, 0, Mathf.Abs(Mathf.Cos(Time.time*2f)));
		}
		if(Time.frameCount%randTime == 0) {
			float randval = Random.Range(0.9f,1.1f);
			float randval2 = Random.Range(0.9f,1.1f);
			transform.GetChild(0).transform.localScale = new Vector3(randval, randval, randval);
			transform.GetChild(0).GetComponent<Outline>().effectColor = new Color(Random.Range(0.2f,1f), Random.Range(0.2f,1f), Random.Range(0.2f,1f));
			transform.GetChild(0).GetComponent<Outline>().effectDistance = new Vector2(randval*10, -randval2*5);
			if(transform.childCount == 2) {
				transform.GetChild(1).GetComponent<Outline>().effectColor = new Color(Random.Range(0.2f,1f), Random.Range(0.2f,1f), Random.Range(0.2f,1f));
				transform.GetChild(1).GetComponent<Outline>().effectDistance = new Vector2(randval2*5, -randval*5);
				transform.GetChild(1).transform.localScale = new Vector3(randval2, randval2, randval2);
			}
		} else {
			transform.GetChild(0).transform.localScale = Vector3.one;
			transform.GetChild(0).GetComponent<Outline>().effectColor = Color.white;
			transform.GetChild(0).GetComponent<Outline>().effectDistance = new Vector2(1, -1);
			if(transform.childCount == 2) {
				transform.GetChild(1).GetComponent<Outline>().effectColor = Color.white;
				transform.GetChild(1).GetComponent<Outline>().effectDistance = new Vector2(1, -1);
				transform.GetChild(1).transform.localScale = Vector3.one;
			}
		}
		if(Time.frameCount%(randTime*2) == 0) {
			if(defaultText.Equals("Save The System")) {
				transform.GetChild(0).GetComponent<Text>().text = "Save Yourself";
			}
		} else if(Time.frameCount%(randTime*3) == 0) {
			if(defaultText.Equals("Save The System")) {
				transform.GetChild(0).GetComponent<Text>().text = "VGDEV RULES";
			}
		} else {
			transform.GetChild(0).GetComponent<Text>().text = defaultText;
		}
	}
}
