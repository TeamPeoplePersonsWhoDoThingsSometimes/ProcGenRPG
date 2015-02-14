using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchText : MonoBehaviour {

	private string text1, text2;

	// Use this for initialization
	void Start () {
		text1 = transform.GetChild(0).GetComponent<Text>().text;
		text2 = transform.GetChild(1).GetComponent<Text>().text;
	}
	
	// Update is called once per frame
	void Update () {
		int randTime = (int)Random.Range(20,100);
		transform.GetChild(1).GetComponent<Text>().color = new Color(0, 0, 0, Mathf.Abs(Mathf.Cos(Time.time*2f)));
		if(Time.frameCount%randTime == 0) {
			float randval = Random.Range(0.9f,1.1f);
			float randval2 = Random.Range(0.9f,1.1f);
			transform.GetChild(0).transform.localScale = new Vector3(randval, randval, randval);
			transform.GetChild(1).transform.localScale = new Vector3(randval2, randval2, randval2);
			transform.GetChild(0).GetComponent<Outline>().effectColor = new Color(Random.Range(0.2f,1f), Random.Range(0.2f,1f), Random.Range(0.2f,1f));
			transform.GetChild(0).GetComponent<Outline>().effectDistance = new Vector2(randval*10, -randval2*5);
			transform.GetChild(1).GetComponent<Outline>().effectColor = new Color(Random.Range(0.2f,1f), Random.Range(0.2f,1f), Random.Range(0.2f,1f));
			transform.GetChild(1).GetComponent<Outline>().effectDistance = new Vector2(randval2*5, -randval*5);
		} else {
			transform.GetChild(0).transform.localScale = Vector3.one;
			transform.GetChild(1).transform.localScale = Vector3.one;
			transform.GetChild(0).GetComponent<Outline>().effectColor = Color.white;
			transform.GetChild(0).GetComponent<Outline>().effectDistance = new Vector2(1, -1);
			transform.GetChild(1).GetComponent<Outline>().effectColor = Color.white;
			transform.GetChild(1).GetComponent<Outline>().effectDistance = new Vector2(1, -1);
		}
		if(Time.frameCount%(randTime*2) == 0) {
			transform.GetChild(0).GetComponent<Text>().text = "Save Yourself";
		} else if(Time.frameCount%(randTime*3) == 0) {
			transform.GetChild(0).GetComponent<Text>().text = "VGDEV RULES";
		} else {
			transform.GetChild(0).GetComponent<Text>().text = "Save The System";
		}
	}
}
