using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class FollowPlayer : MonoBehaviour {

	public static float rotate;
	public static bool locked = false;

	public RectTransform damaged;

	private Vector3 offset;
	private float rotateSpeed;
	private Transform centerCamRef;

	public static float zoom = 20f;

	private Player p;
	private float prevHealth = 1;

	public static float traveling;

	private bool finalBoss = false;
	private static bool finalBossDying = false;

	private static float fastMoveTime = 0f;

	// Use this for initialization
	void Start () {
		traveling = 5f;
		FMOD_StudioSystem.instance.PlayOneShot("event:/environment/portal",Player.playerPos.position, PlayerPrefs.GetFloat("MasterVolume")/2f);
		offset = this.transform.position - Player.playerPos.position;
		p = GameObject.Find("PlayerObj").GetComponent<Player>();

		if(GameObject.Find("FINALBOSS") != null) {
			finalBoss = true;
		}
		zoom = 20f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, transform.parent.eulerAngles.y + Input.GetAxis("Mouse X"), 0f)), 200*Time.deltaTime);
//		transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(new Vector3(0f, Player.playerPos.eulerAngles.y, 0f)), 2000*Time.deltaTime);

		rotate = transform.parent.eulerAngles.y;

		if(!PlayerControl.immobile) {
			if (Input.GetKey(PersistentInfo.rightKey)) {
				if(rotateSpeed <= 100f) {
					rotateSpeed = 100f;
				}
				rotateSpeed = Mathf.MoveTowards(rotateSpeed, 300, 100*Time.deltaTime);
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
			}
			if (Input.GetKey(PersistentInfo.leftKey)) {
				if(rotateSpeed >= -100f) {
					rotateSpeed = -100f;
				}
				rotateSpeed = Mathf.MoveTowards(rotateSpeed, -300, 100*Time.deltaTime);
				transform.parent.RotateAround(Player.playerPos.position, Player.playerPos.up, rotateSpeed*Time.deltaTime);
			}
		}

		if(!Input.GetKey(PersistentInfo.leftKey) && !Input.GetKey(PersistentInfo.rightKey)) {
			rotateSpeed = 0;
		}

		if (!locked) {
			transform.parent.position = Vector3.MoveTowards(transform.parent.position, Player.playerPos.position, 50*Time.deltaTime * (fastMoveTime > 0 ? 4 : 1));
		}

		if(!PlayerCanvas.inConsole) {
			if(Input.GetAxis("Mouse ScrollWheel") > 0 && zoom < 24) {
				transform.GetChild(0).transform.localPosition = Vector3.MoveTowards(transform.GetChild(0).transform.localPosition, new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y - 0.5f, transform.GetChild(0).transform.localPosition.z + 0.5f), Time.deltaTime*30f);
				zoom += 0.1f;
			} else if ((Input.GetAxis("Mouse ScrollWheel") < 0 || finalBoss) && zoom > 1f) {
				zoom -= 0.1f;
				transform.GetChild(0).transform.localPosition = Vector3.MoveTowards(transform.GetChild(0).transform.localPosition, new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y + 0.5f, transform.GetChild(0).transform.localPosition.z - 0.5f), Time.deltaTime*30f);
			} else if(finalBoss && zoom <= 1f) {
				finalBoss = false;
			}
			Vector3 lookDirection1 = (Player.playerPos.position + new Vector3(0,2,0) + Player.playerPos.forward*(10/zoom)) - transform.GetChild(0).transform.position;
			Vector3 lookDirection2 = (Player.playerPos.position + new Vector3(0,2,0) + Player.playerPos.forward*(10/zoom)) - transform.GetChild(1).transform.position;
			Quaternion lookRotation1 = Quaternion.LookRotation(lookDirection1);
			Quaternion lookRotation2 = Quaternion.LookRotation(lookDirection2);
			if(Input.GetAxis("Mouse ScrollWheel") == 0) {
				transform.GetChild(0).transform.rotation = Quaternion.RotateTowards(transform.GetChild(0).transform.rotation, lookRotation1, 0.1f * (fastMoveTime > 0 ? 10 : 1));
			} else {
				transform.GetChild(0).transform.rotation = Quaternion.RotateTowards(transform.GetChild(0).transform.rotation, lookRotation1, 1f);
			}
		}

		if(prevHealth > p.GetIntegrityPercentage()) {
			damaged.localScale = Vector3.one;
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				Camera.main.GetComponent<VignetteAndChromaticAberration>().blur = 2*(1-p.GetIntegrityPercentage());
				Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = Mathf.Max(2f, 10*(1-p.GetIntegrityPercentage())) + ((1-p.GetIntegrityPercentage())*Random.Range(-1f,5f));
			}
		} else if (prevHealth < p.GetIntegrityPercentage()) {
			damaged.localScale = Vector3.MoveTowards(damaged.localScale, Vector3.one * Mathf.Max(4f * p.GetIntegrityPercentage(), 1f), Time.deltaTime);
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				Camera.main.GetComponent<VignetteAndChromaticAberration>().blur = Mathf.MoveTowards(Camera.main.GetComponent<VignetteAndChromaticAberration>().blur, 0f, Time.deltaTime);
				Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = Mathf.MoveTowards(Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration, 2f, Time.deltaTime);
			}
		}

		if(p.GetIntegrityPercentage() == 1) {
			damaged.localScale = Vector3.one * 4f;
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				Camera.main.GetComponent<VignetteAndChromaticAberration>().blur = 0f;
				Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = 2f;
			}
		}

		prevHealth = p.GetIntegrityPercentage();

		if(traveling > 0f) {
			traveling -= Time.deltaTime*5f;
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = 100*traveling;
			}
		}

		if(finalBossDying) {
			Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = Random.value*10;
		}

		fastMoveTime -= Time.deltaTime;

	}

	/**
	 * Sets the MainCamera's position to that of the player
	 */
	public void SetToPlayer() {
		transform.parent.position = Player.playerPos.position;
	}

	public static void Travel() {
		if(CityHelp.helpMode == 3) {
			CityHelp.helpMode = -1;
		}
		traveling = 5f;
	}

	public static void FinalBossDying() {
		finalBossDying = true;
	}

	public static void MoveCamFast() {
		fastMoveTime = 10f;
	}

}
