using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchText : MonoBehaviour {

	private string defaultText;
	public float glitchiness;

	private static AudioSource organicMusic, glitchMusic;

	// Use this for initialization
	void Start () {
		defaultText = transform.GetChild(0).GetComponent<Text>().text;
		organicMusic = GameObject.Find("OrganicMusic").GetComponent<AudioSource>();
		glitchMusic = GameObject.Find("GlitchMusic").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		int randTime = (int)(Random.Range(20,100)*(1/glitchiness));
		if(transform.childCount == 2) {
			transform.GetChild(1).GetComponent<Text>().color = new Color(0, 0, 0, Mathf.Abs(Mathf.Cos(Time.time*2f)));
		}
		if(Time.frameCount%randTime == 0) {
			GlitchAudio();
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
			if(Time.frameCount % 10 == 0) {
				glitchMusic.volume = 0;
				organicMusic.volume = 1;
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

	void GlitchAudio() {
		float tempTime = organicMusic.time;
		float tempPitch = organicMusic.pitch;
		float randVal1 = Random.Range(-1f,1f);
		float randVal2 = Random.Range(0.1f,1.1f);
		float randVal3 = Random.Range(0.95f, 1.05f);
		if(tempTime > 2) {
			organicMusic.time += randVal1;
			glitchMusic.time += randVal1;
		} else {
			organicMusic.time += randVal2;
			glitchMusic.time += randVal2;
		}
		if(tempPitch == 1 && Time.frameCount % 50 == 0) {
			organicMusic.pitch = randVal3;
			glitchMusic.pitch = randVal3;
		} else {
			organicMusic.pitch = 1f;
			glitchMusic.pitch = 1f;
		}
		if(organicMusic.volume == 1) {
			glitchMusic.volume = 1;
			organicMusic.volume = 0;
		}
	}
}
