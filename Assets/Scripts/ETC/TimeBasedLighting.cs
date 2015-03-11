using UnityEngine;
using System.Collections;

public class TimeBasedLighting : MonoBehaviour {

	public Color morning, night;
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0f, speed * 10f * Time.deltaTime, 0f), Space.World);
		float t = Mathf.PingPong(Time.time, speed * 10f) / (10f/speed);
		GetComponent<Light>().color = Color.Lerp(morning, night,t);
	}
}
