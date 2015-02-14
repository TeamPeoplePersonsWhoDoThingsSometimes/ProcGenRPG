using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResponsiveDot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = Mathf.Min(2f,Mathf.Max(200f/(Vector3.Distance(transform.position, new Vector3(Camera.main.ScreenToViewportPoint(Input.mousePosition).x*Screen.width, Camera.main.ScreenToViewportPoint(Input.mousePosition).y*Screen.height))+1),0.5f)) * Vector3.one;
		transform.localScale = new Vector3(temp.x, temp.y, temp.z);
//		Debug.Log (1f/(Vector3.Distance(transform.position, new Vector3(Camera.main.ScreenToViewportPoint(Input.mousePosition).x*Screen.width, Camera.main.ScreenToViewportPoint(Input.mousePosition).y*Screen.height))+1));
	}
}
