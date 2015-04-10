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
			if(trackingEnemy.GetComponent<FinalBoss>() != null) {
				GameObject temp = Instantiate(this.transform.GetChild(4).gameObject);
				temp.GetComponent<RectTransform>().SetParent(this.transform, false);
				temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition.x,this.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition.y - 0.1f);
				temp.GetComponent<Text>().text = "MemLeaks: " + 0;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(trackingEnemy != null && trackingEnemy.GetComponent<Enemy>() != null) {
			/*** Setting color of version number ***/
			if(Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) < Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.green;
			} else if (Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) == Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.yellow;
			}  else if (Utility.VersionToInt(trackingEnemy.GetComponent<Enemy>().GetVersion()) > Utility.VersionToInt(Player.version)) {
				this.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.red;
			}

			/*** Sets position and scale of healthbar on screenspace ***/
			if(trackingEnemy.GetComponent<Boss>() == null) {
				Vector3 tempPos = Camera.main.WorldToViewportPoint(trackingEnemy.transform.position);
				float tempScale = Mathf.Clamp(15f/Vector3.Distance(Player.playerPos.position,trackingEnemy.transform.position)*(FollowPlayer.zoom/20f), 1.5f, 3f);
				this.GetComponent<RectTransform>().anchoredPosition = new Vector2(13.62f*tempPos.x - tempScale*0.7f, 0.5f-7.4f*(1-tempPos.y));
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
			} else {
				this.GetComponent<RectTransform>().anchoredPosition = new Vector2(4f,-0.4f);
				this.GetComponent<RectTransform>().localScale = new Vector3(3.5f,3f,2f);
				this.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(trackingEnemy.GetComponent<Enemy>().GetHealthPercentage(), 1,1);
			}

			if (trackingEnemy.GetComponent<FinalBoss>() != null) {
				transform.GetChild(5).GetComponent<Text>().text = "MemLeaks: " + FinalBoss.memLeaksCount;
			}

			/*** Destroy healthbar if tracking enemy is null ***/
		} else {
			Destroy(this.gameObject);
		}
	}
}
