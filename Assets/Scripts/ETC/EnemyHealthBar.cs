using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour {

	public GameObject trackingEnemy;

	// Use this for initialization
	void Start () {
		if(trackingEnemy != null) {
			this.transform.GetChild(4).GetComponent<Text>().text = trackingEnemy.GetComponent<Enemy>().name;
			this.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = trackingEnemy.GetComponent<Enemy>().GetVersion();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(trackingEnemy != null) {
			/*** Setting color of version number ***/
			if(Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) < Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.green;
			} else if (Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) == Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.yellow;
			}  else if (Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) > Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.red;
			}

			/*** Sets position and scale of healthbar on screenspace ***/
			Vector3 tempPos = Camera.main.WorldToViewportPoint(trackingEnemy.transform.position);
			float tempScale = Mathf.Clamp(15f/Vector3.Distance(Player.playerPos.position,trackingEnemy.transform.position), 1f, 1.75f);
			this.GetComponent<RectTransform>().anchoredPosition = new Vector2(11.612f*tempPos.x - tempScale*0.7f, 0.5f-6.53f*(1-tempPos.y));
			this.GetComponent<RectTransform>().localScale = new Vector3(tempScale, tempScale, tempScale);
			this.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(trackingEnemy.GetComponent<Enemy>().GetHealthPercentage(), 1,1);

			/*** Mouseover checking and opacity handling ***/
			RaycastHit hitinfo = new RaycastHit();
			if(trackingEnemy.GetComponent<Enemy>().ShowHealthbar()
			   || (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitinfo, 2000000f)
			   && hitinfo.collider != null && hitinfo.collider.gameObject != null
			    && hitinfo.collider.gameObject.Equals(trackingEnemy))) {
				this.GetComponent<CanvasGroup>().alpha = 1f;
			} else {
				this.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
				if (this.GetComponent<CanvasGroup>().alpha <= 0.1f) {
					this.GetComponent<CanvasGroup>().alpha = 0.1f;
				}
			}

			/*** Destroy healthbar if tracking enemy is null ***/
		} else {
			Destroy(this.gameObject);
		}
	}
}
