using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthCanvas : MonoBehaviour {

	private static Transform camRot;
	private Image healthBar;
	private Text textName;
	private Enemy enemy;

	// Use this for initialization
	void Start () {
		if(camRot == null) {
			camRot = GameObject.Find("CamRotate").transform;
		}
		enemy = transform.parent.GetComponent<Enemy>();
		textName = transform.GetChild(2).GetComponent<Text>();
		healthBar = transform.GetChild(1).GetComponent<Image>();
		textName.text = enemy.name + "\nv" + enemy.version;
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles = camRot.eulerAngles;
		healthBar.rectTransform.localScale = new Vector3(0.03f*enemy.GetHealthPercentage() + 0.001f, 0.004f, 1f);
	}
}
