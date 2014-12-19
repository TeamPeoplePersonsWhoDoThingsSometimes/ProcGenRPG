using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour {

	public GameObject trackingEnemy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(trackingEnemy != null) {
			Vector3 tempPos = Camera.main.WorldToViewportPoint(trackingEnemy.transform.position);
			this.GetComponent<RectTransform>().anchoredPosition = new Vector2(11.612f*tempPos.x + 0.25f, 0.25f-6.53f*(1-tempPos.y));
			float tempScale = Mathf.Min(1f, 15f/Vector3.Distance(Player.playerPos.position,trackingEnemy.transform.position));
			this.GetComponent<RectTransform>().localScale = new Vector3(tempScale, tempScale, tempScale);
		} else {
			Destroy(this.gameObject);
		}
	}
}
