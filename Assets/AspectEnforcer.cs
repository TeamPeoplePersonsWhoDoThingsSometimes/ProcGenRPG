using UnityEngine;
using System.Collections;

public class AspectEnforcer : MonoBehaviour {
	// set the desired aspect ratio (the values in this example are
	// hard-coded for 16:9, but you could make them into public
	// variables instead so you can set them at design time)
	float targetaspect = 1024f / 576f;
	
	// determine the game window's current aspect ratio
	float windowaspect;
	
	// current viewport height should be scaled by this amount
	float scaleheight;

	private Camera[] cams;

	public static Rect camRect;

	private Texture2D black;

	void Start() {
//		black = Resources.Load<Texture2D>("Textures/blackBox");
		black = new Texture2D(1,1);
		black.SetPixel(0,0,Color.black);
		black.Apply();
		// obtain cam component so we can modify its viewport
		cams = GameObject.FindObjectsOfType<Camera>();
		for(int i = 0; i < cams.Length; i++) {
			if(cams[i].name.Equals("MiniMapCam") || cams[i].name.Equals("WorldMapCam")) {
				cams[i] = null;
			}
		}
	}

	// Use this for initialization
	void Update () 
	{
		
		// determine the game window's current aspect ratio
		windowaspect = (float)Screen.width / (float)Screen.height;
		
		// current viewport height should be scaled by this amount
		scaleheight = windowaspect / targetaspect;
		
		// if scaled height is less than current height, add letterbox
		foreach(Camera cam in cams) {
			if(cam != null) {
				if (scaleheight < 1.0f)
				{  
					Rect rect = cam.rect;
					
					rect.width = 1.0f;
					rect.height = scaleheight;
					rect.x = 0;
					rect.y = (1.0f - scaleheight) / 2.0f;
					
					cam.rect = rect;

				}
				else // add pillarbox
				{
					float scalewidth = 1.0f / scaleheight;
					
					Rect rect = cam.rect;
					
					rect.width = scalewidth;
					rect.height = 1.0f;
					rect.x = (1.0f - scalewidth) / 2.0f;
					rect.y = 0;
					
					cam.rect = rect;
				}
			}

//			camRect = new Rect(cam.rect.x*Screen.width, cam.rect.y*Screen.height, cam.rect.width * Screen.width, cam.rect.height * Screen.height);
		}
	}

	private int tempIdx = 1;
	void OnGUI() {
		Camera cam = cams[0];
		tempIdx = 0;
		while(cam == null || tempIdx < cams.Length) {
			cam = cams[tempIdx];
			tempIdx++;
		}
		GUI.DrawTexture(new Rect(0,0,Screen.width,cam.rect.y*Screen.height), black);
		GUI.DrawTexture(new Rect(0,cam.rect.yMax*Screen.height,Screen.width,cam.rect.y*Screen.height), black);

		GUI.DrawTexture(new Rect(0,0,cam.rect.x * Screen.width,Screen.height), black);
		GUI.DrawTexture(new Rect(cam.rect.xMax*Screen.width,0,cam.rect.x * Screen.width,Screen.height), black);
	}
}
