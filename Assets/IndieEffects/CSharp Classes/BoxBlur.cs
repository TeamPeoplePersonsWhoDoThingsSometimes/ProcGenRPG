using UnityEngine;

[RequireComponent(typeof(IndieEffects))]
[AddComponentMenu("Indie Effects/Blur C#")]
public class BoxBlur : MonoBehaviour {

	public IndieEffects fxRes;
	
	private Material blurMat;
	public Shader blurShader;
	[Range(0,5)]
	public float passes;

	// Use this for initialization
	void Start () {
		fxRes = GetComponent<IndieEffects>();
		blurMat = new Material(blurShader);
	}
	
	// Update is called once per frame
	void Update () {
		blurMat.SetTexture("_MainTex", fxRes.RT);
	}

	void OnPostRender() {
		GL.PushMatrix();
		for (var i = 0; i < passes; ++i) {
			
			blurMat.SetPass(i);
			GL.LoadOrtho();
			GL.Begin(GL.QUADS);
			GL.Color(new Color(1,1,1,1));
			GL.MultiTexCoord(0, new Vector3(0,0,0));
			GL.Vertex3(0,0,0);
			GL.MultiTexCoord(0, new Vector3(0,1,0));
			GL.Vertex3(0,1,0);
			GL.MultiTexCoord(0, new Vector3(1,1,0));
			GL.Vertex3(1,1,0);
			GL.MultiTexCoord(0, new Vector3(1,0,0));
			GL.Vertex3(1,0,0);
			GL.End();
			
			if (i < blurMat.passCount - 1) {
				fxRes.RT.ReadPixels(new Rect(0,0,Screen.width,Screen.height), 0, 0);
				fxRes.RT.Apply();
			}
		}
		GL.PopMatrix();
	}
}
