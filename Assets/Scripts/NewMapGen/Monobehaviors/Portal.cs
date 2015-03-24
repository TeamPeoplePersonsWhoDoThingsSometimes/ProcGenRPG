using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Portal : MonoBehaviour {

    public Direction dir;
	private AsyncOperation loadScene;
	private float loadAmount;

    void OnTriggerEnter(Collider other)
    {
		Debug.Log(Application.loadedLevel);
		if(other.tag.Equals("Player") && Application.loadedLevel == 2) {
			loadScene = Application.LoadLevelAsync(3);
			loadScene.allowSceneActivation = false;
		} else if (other.tag.Equals("Player")) {
            MasterDriver.Instance.moveArea(dir);
        }
    }

	void Update() {
		if (loadScene != null) {
			loadAmount = Mathf.MoveTowards(loadAmount, loadScene.progress, Time.deltaTime);
			Camera.main.GetComponent<ColorCorrectionCurves>().redChannel.MoveKey(0,new Keyframe(0,loadAmount*1.1f));
			Camera.main.GetComponent<ColorCorrectionCurves>().greenChannel.MoveKey(0,new Keyframe(0,loadAmount*1.1f));
			Camera.main.GetComponent<ColorCorrectionCurves>().blueChannel.MoveKey(0,new Keyframe(0,loadAmount*1.1f));
			Camera.main.GetComponent<ColorCorrectionCurves>().UpdateParameters();
				Debug.Log(Camera.main.GetComponent<ColorCorrectionCurves>().redChannel.keys[1].value + " " + Camera.main.GetComponent<ColorCorrectionCurves>().redChannel.keys[0].value);
			if(loadAmount >= 0.9f) {
				loadScene.allowSceneActivation = true;
			}
		}
	}

}
