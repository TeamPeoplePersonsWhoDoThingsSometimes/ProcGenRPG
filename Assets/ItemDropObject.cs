using UnityEngine;
using System.Collections;

public class ItemDropObject : MonoBehaviour {

	public GameObject item;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < this.transform.childCount; i++) {
			this.transform.GetChild(i).localScale = new Vector3(Mathf.Sin(Time.time) + 1, Mathf.Sin(Time.time) + 1, Mathf.Sin(Time.time) + 1);
		}
	}

	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag.Equals("Player")) {
			other.gameObject.GetComponent<Player>().PickUpItem(item);
			Destroy(this.gameObject);
		}
	}
}
