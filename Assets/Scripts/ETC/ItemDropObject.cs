using UnityEngine;
using System.Collections;

public class ItemDropObject : MonoBehaviour {

	public GameObject item;

	private Vector3[] scales;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().AddForce(Vector3.up*Random.Range(8,10),ForceMode.VelocityChange);
		scales = new Vector3[transform.childCount];
		for(int i = 0; i < scales.Length; i++) {
			scales[i] = transform.GetChild(i).localScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(item.GetComponent<Item>().RarityVal.Equals(Rarity.Anomaly)) {
			this.GetComponent<Light>().intensity = Random.value*5 + 3;
			this.GetComponent<Light>().range = Random.value*5 + 3;
			this.GetComponent<Light>().color = new Color(Random.value + Random.value, Random.value + Random.value, Random.value + Random.value);
			this.GetComponent<Renderer>().sharedMaterial.color = new Color(Random.value + Random.value, Random.value + Random.value, Random.value + Random.value);
			for(int i = 0; i < this.transform.childCount; i++) {
				this.transform.GetChild(i).localScale = scales[i]*Random.Range(1f,1.2f);
			}
			for(int i = 0; i < this.transform.childCount; i++) {
				this.transform.GetChild(i).localEulerAngles = new Vector3(Mathf.Sin(Time.time)*360 + i*45f, Mathf.Sin(Time.time)*360 + i*45f, Mathf.Sin(Time.time)*360 + i*45f);
			}
		} else {
			for(int i = 0; i < this.transform.childCount; i++) {
				this.transform.GetChild(i).localEulerAngles = new Vector3(Mathf.Sin(Time.time)*180 + i*45f, Mathf.Sin(Time.time)*180 + i*45f, Mathf.Sin(Time.time)*180 + i*45f);
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag.Equals("Player")) {
			other.gameObject.GetComponent<Player>().PickUpItem(item);
			//TODO: Figure out how to deal with this?!
//			Destroy(this.item);
			Destroy(this.gameObject);
		}
	}
}
