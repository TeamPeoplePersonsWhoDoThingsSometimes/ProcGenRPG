using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	
	public float x, z;

	public float y;//height to place the tile at

	public float size;
	
	public string name;
	
	public bool ground;
	
	public bool slow;
	
	private Texture tex;
	
	public Tile() {	
		
	}
	
	public void Init() {
		x = transform.position.x;
		z = transform.position.z;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public float Z {
		get {
			return z;
		}
	}
	
	public float X {
		get {
			return x;
		}
	}
	
}
