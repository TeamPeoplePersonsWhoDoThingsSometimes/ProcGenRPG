﻿using UnityEngine;
using System.Collections;

public class HitInfoScript : MonoBehaviour {

	private float localTime = 0f;
	private static Transform camRot;

	public bool isBlockingText;
	// Use this for initialization
	void Start () {
		if(camRot == null) {
			camRot = GameObject.Find("CamRotate").transform;
		}
		if(isBlockingText) {
			GetComponent<TextMesh>().color = Color.green;
			GetComponent<TextMesh>().fontSize = 40;
		}
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.eulerAngles = camRot.eulerAngles;
		if(!isBlockingText) {
			localTime += Time.deltaTime;
			if(localTime < 0.1f) {
				GetComponent<Rigidbody>().AddForce(new Vector3(Random.value, 2f, Random.value), ForceMode.VelocityChange);
			}
			transform.Rotate(0f,50*Time.deltaTime,0f);
			GetComponent<TextMesh>().color = new Color(1,1,1,Mathf.Abs(Mathf.Min(-2, -localTime) + 4)/2f);
			if(localTime > 4) {
				Destroy(this.gameObject);
			}
		}
	}
}
