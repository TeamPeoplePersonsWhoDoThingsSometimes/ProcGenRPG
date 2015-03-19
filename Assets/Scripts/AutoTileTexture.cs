using UnityEngine;
using System.Collections;

public class AutoTileTexture : MonoBehaviour {

	public float scaleValue = 2f;
	
	// Use this for initialization
	void Start () {
		Material temp = new Material(this.gameObject.GetComponent<Renderer>().sharedMaterial);
		temp.SetTextureScale("_MainTex",new Vector2(this.gameObject.transform.lossyScale.x*scaleValue,this.gameObject.transform.lossyScale.z*scaleValue));
		this.gameObject.GetComponent<Renderer>().sharedMaterial = temp;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnDrawGizmos()
	{

	}
}
