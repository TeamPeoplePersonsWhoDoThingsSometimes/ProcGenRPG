using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthCanvas : MonoBehaviour {

	private static Transform camRot;
	private Image healthBar;
	private Text textName;
	private Text hpText;
	private Enemy enemy;

	// Use this for initialization
	void Start () {
		if(camRot == null) {
			camRot = GameObject.Find("CamRotate").transform;
		}
		enemy = transform.parent.GetComponent<Enemy>();
		textName = transform.GetChild(3).GetComponent<Text>();
		healthBar = transform.GetChild(2).GetComponent<Image>();
		hpText = transform.GetChild(4).GetComponent<Text>();
		textName.text = enemy.name;
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles = camRot.eulerAngles;
		healthBar.rectTransform.localScale = new Vector3(0.03f*enemy.GetHealthPercentage() + 0.001f, healthBar.rectTransform.localScale.y, 1f);
		if (hpText != null) {
			hpText.text = enemy.HealthString();
		}
		GetComponent<CanvasGroup>().alpha = Mathf.Min(1, Mathf.Max((20 - Mathf.Min(20, Vector3.Distance(this.transform.position, Player.playerPos.position)))/20f + 0.1f, 0));
	}
}
