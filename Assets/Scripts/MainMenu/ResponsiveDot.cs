using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResponsiveDot : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("New State")) {
			Vector3 mousePos = Camera.main.ViewportToWorldPoint(new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height, transform.position.z));
			Vector3 temp = Mathf.Min(4f,Mathf.Max(50f/(Vector3.Distance(transform.position,new Vector3(mousePos.x, mousePos.y, transform.position.z))+1),2f)) * Vector3.one;
			transform.localScale = new Vector3(temp.x, temp.y, temp.z);
			GetComponent<RectTransform>().sizeDelta = new Vector2(1f,1f);
		}


		if(Mathf.Abs(transform.position.x - 102.7f) < 2f && Mathf.Abs(transform.position.y - 51.3f) < 2f) {
//			Debug.Log(200f/(Vector3.Distance(transform.position, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x*Screen.width, Camera.main.ScreenToWorldPoint(Input.mousePosition).y*Screen.height, transform.position.z))+1) * Vector3.one);
//			Debug.Log(Camera.main.ViewportToWorldPoint(new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height, transform.position.z))*4.5f);
//			Debug.Log(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
//			Debug.Log(transform.localScale);
		}
	}
}
