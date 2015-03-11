using UnityEngine;
using System.Collections;

class AnimatedTiledTexture : MonoBehaviour
{
	public int columns = 2;
	public int rows = 2;
	public float framesPerSecond = 10f;
	public bool randFrame = false;
	public bool randStart = false;
	
	//the current frame to display
	private int index = 0;
	
	void Start() {
		if(randStart) {
			index = (int) (Random.value*columns*rows);
		}

		StartCoroutine(updateTiling());
		
		//set the tile size of the texture (in UV units), based on the rows and columns
		Vector2 size = new Vector2(1f / columns, 1f / rows);
		GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", size);
	}
	
	private IEnumerator updateTiling() {
		while(true) {
			if (!randFrame) {
				//move to the next index
				index++;
				if (index >= rows * columns)
					index = 0;
			} else {
				index = (int) (Random.value*columns*rows);
			}
			//split into x and y indexes
			Vector2 offset = new Vector2(((float)index / columns), ((float)(columns - 1f)/columns) - ((index / columns) * (1f/columns)));
			
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			
			yield return new WaitForSeconds(1f / framesPerSecond);
		}
	}
}
