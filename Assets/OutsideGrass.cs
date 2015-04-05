using UnityEngine;
using System.Collections;

public class OutsideGrass : MonoBehaviour {

	private float[] randomVals;

	// Use this for initialization
	void Start () {
		randomVals = new float[this.GetComponent<MeshFilter>().mesh.vertices.Length];
		for(int i = 0; i < randomVals.Length; i++) {
			randomVals[i] = Random.value*2f - 2f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3[] vertices = this.GetComponent<MeshFilter>().mesh.vertices;
		
		if(Time.frameCount%40 == 0) {
			for(int i = 0; i < randomVals.Length; i++) {
				randomVals[i] = Random.value*2f - 2f;
			}
		}

		for(int i = 0; i < vertices.Length; i++) {
			vertices[i] = new Vector3(vertices[i].x,Mathf.MoveTowards(vertices[i].y,randomVals[i],Time.deltaTime*(Mathf.Abs(vertices[i].y-randomVals[i]))), vertices[i].z);
		}
		this.GetComponent<MeshFilter>().mesh.vertices = vertices;
		this.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		this.GetComponent<MeshFilter>().mesh.RecalculateNormals();
	}
}
