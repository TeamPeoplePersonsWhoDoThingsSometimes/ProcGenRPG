using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorldMap : MonoBehaviour {

	public Camera worldMapCam;

	public GameObject tilePrefab, starPrefab, connectorPrefab, questPanelPrefab;
	public Material safe, dangerous, quest;

	private float rotateSpeed;

	private List<Area> genAreas;
	private static Dictionary<GameObject, string> questStars;

	private static WorldMap instance;

	private bool mapOpen = false;

	void OnEnable() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		genAreas = new List<Area>();
		if(MasterDriver.Instance != null) {
			genAreas.Add(MasterDriver.Instance.CurrentArea);
			GenTilesAroundArea(MasterDriver.Instance.CurrentArea, tilePrefab);
		}
	}

	void GenTilesAroundArea(Area a, GameObject g) {
		Area[] neighbors = a.getNeighborsForMapGen();
//		if((a.position.x == 5 || a.position.x == 4 || a.position.x == 6) && (a.position.y == 5 || a.position.y == 4 || a.position.y == 6)) {
//			Debug.Log(a.position + ": " + a.north + " " + a.east + " " + a.south + " " + a.west);
//		}
		if (a.north && !genAreas.Contains(neighbors[0])) {
//			if((a.position.x == 5 || a.position.x == 4 || a.position.x == 6) && (a.position.y == 5 || a.position.y == 4 || a.position.y == 6)) {
//				Debug.Log(a.position + ": placing north");
//			}
			GameObject temp = (GameObject)GameObject.Instantiate(tilePrefab);
			temp.name = neighbors[0].position.ToString();
			temp.transform.parent = tilePrefab.transform.parent;
			temp.transform.localPosition = new Vector3(g.transform.localPosition.x,0f,g.transform.localPosition.z + 5f);
			genAreas.Add(neighbors[0]);
			GenTilesAroundArea(neighbors[0],temp);
		}
		if (a.east && !genAreas.Contains(neighbors[1])) {
//			if((a.position.x == 5 || a.position.x == 4 || a.position.x == 6) && (a.position.y == 5 || a.position.y == 4 || a.position.y == 6)) {
//				Debug.Log(a.position + ": placing east");
//			}
			GameObject temp = (GameObject)GameObject.Instantiate(tilePrefab);
			temp.name = neighbors[1].position.ToString();
			temp.transform.parent = tilePrefab.transform.parent;
			temp.transform.localPosition = new Vector3(g.transform.localPosition.x + 5f,0f,g.transform.localPosition.z);
			genAreas.Add(neighbors[1]);
			GenTilesAroundArea(neighbors[1],temp);
		}
		if (a.south && !genAreas.Contains(neighbors[2])) {
//			if((a.position.x == 5 || a.position.x == 4 || a.position.x == 6) && (a.position.y == 5 || a.position.y == 4 || a.position.y == 6)) {
//				Debug.Log(a.position + ": placing south");
//			}
			GameObject temp = (GameObject)GameObject.Instantiate(tilePrefab);
			temp.name = neighbors[2].position.ToString();
			temp.transform.parent = tilePrefab.transform.parent;
			temp.transform.localPosition = new Vector3(g.transform.localPosition.x,0f,g.transform.localPosition.z - 5f);
			genAreas.Add(neighbors[2]);
			GenTilesAroundArea(neighbors[2],temp);
		}
		if (a.west && !genAreas.Contains(neighbors[3])) {
//			if((a.position.x == 5 || a.position.x == 4 || a.position.x == 6) && (a.position.y == 5 || a.position.y == 4 || a.position.y == 6)) {
//				Debug.Log(a.position + ": placing west");
//			}
			GameObject temp = (GameObject)GameObject.Instantiate(tilePrefab);
			temp.name = neighbors[3].position.ToString();
			temp.transform.parent = tilePrefab.transform.parent;
			temp.transform.localPosition = new Vector3(g.transform.localPosition.x - 5f,0f,g.transform.localPosition.z);
			genAreas.Add(neighbors[3]);
			GenTilesAroundArea(neighbors[3],temp);
		}

		if(a.north) {
			GameObject tempCon = (GameObject)GameObject.Instantiate(connectorPrefab);
			tempCon.transform.parent = g.transform;
			tempCon.transform.localPosition = new Vector3(0,-0.5f,0.5f);
			tempCon.SetActive(true);
		}
		if(a.west) {
			GameObject tempCon = (GameObject)GameObject.Instantiate(connectorPrefab);
			tempCon.transform.parent = g.transform;
			tempCon.transform.localEulerAngles = new Vector3(0,90,0);
			tempCon.transform.localPosition = new Vector3(-0.5f,-0.5f,0f);
			tempCon.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(MasterDriver.Instance != null) {
			Vector3 mapPos = new Vector3(worldMapCam.transform.localPosition.x + (-(MasterDriver.Instance.CurrentArea.position.x - 4)*5), -50f, worldMapCam.transform.localPosition.z + 10.5f + (-(MasterDriver.Instance.CurrentArea.position.y - 4)*5));
			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, mapPos, Time.deltaTime*10f);
			if(questStars != null) {
				foreach(KeyValuePair<GameObject, string> kvp in questStars) {
					if(!kvp.Key.activeSelf) {
						kvp.Key.transform.SetParent(this.transform, false);
						kvp.Key.SetActive(true);
					}
					kvp.Key.transform.Rotate(0f,Time.deltaTime*20f,0f);
				}
			}
			
			if(Input.GetKeyDown(KeyCode.M) && !PlayerCanvas.inConsole) {
				mapOpen = !mapOpen;
			}
			
			if (mapOpen && worldMapCam.rect.width < 0.7f) {
				worldMapCam.rect = new Rect(worldMapCam.rect.x + Time.deltaTime/10f, worldMapCam.rect.y - Time.deltaTime, worldMapCam.rect.width + Time.deltaTime, worldMapCam.rect.height + Time.deltaTime);
			} else if (!mapOpen && worldMapCam.rect.width > 0.13f) {
				worldMapCam.rect = new Rect(worldMapCam.rect.x - Time.deltaTime/10f, worldMapCam.rect.y + Time.deltaTime, worldMapCam.rect.width - Time.deltaTime, worldMapCam.rect.height - Time.deltaTime);
			} else if (mapOpen) {
				
			} else if (!mapOpen) {
				worldMapCam.rect = new Rect(0.01f, 0.79f, 0.13f, 0.2f);
			}
			
			RaycastHit inf;
			//		Vector3 mousePos = new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
			//		Debug.Log(worldMapCam.ViewportPointToRay(mousePos));
			Debug.DrawRay(worldMapCam.ScreenPointToRay(Input.mousePosition).origin, worldMapCam.ScreenPointToRay(Input.mousePosition).direction*20f);
			if(mapOpen && Physics.Raycast(new Ray(worldMapCam.ScreenPointToRay(Input.mousePosition).origin, worldMapCam.ScreenPointToRay(Input.mousePosition).direction*20f), out inf)) {
				if(inf.collider.gameObject.name.Contains("questStar")) {
					questPanelPrefab.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition/(Screen.width/150f) - new Vector3(0,80,0);
					questPanelPrefab.transform.GetChild(0).GetComponent<Text>().text = questStars[inf.collider.gameObject];
				}
			} else {
				questPanelPrefab.GetComponent<RectTransform>().anchoredPosition = Vector3.one*10000f;
			}
		} else {
			Vector3 mapPos = new Vector3(worldMapCam.transform.localPosition.x, -50f, worldMapCam.transform.localPosition.z + 10.5f);
			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, mapPos, Time.deltaTime*10f);
		}

		worldMapCam.transform.parent.localEulerAngles = new Vector3(0f,FollowPlayer.rotate,0f);

		worldMapCam.enabled = !PlayerCanvas.cinematicMode && !PlayerCanvas.inConsole;
		PlayerCanvas.mapMode = mapOpen;
		
	}

	public static void AddStarAt(int x, int z, string text) {
		//Debug.Log ("Place Star at: " + x + "," + z);
		
		GameObject temp = (GameObject)GameObject.Instantiate(instance.starPrefab);
		temp.transform.localPosition = new Vector3((x - 4)*5,3f,(z - 4)*5);
//		temp.transform.localPosition = new Vector3(0,3f,0);
		//Debug.Log("Placing star at: " + x + " " + z + "\nAKA: " + temp.transform.localPosition);
		if(questStars == null) {
			questStars = new Dictionary<GameObject, string>();
		}
		questStars.Add(temp,text);
	}

	/*public static void AddQuest(string s) {
		Debug.Log("ADDING: " + s);
		GameObject[] tempList = new GameObject[questStars.Keys.Count];
		questStars.Keys.CopyTo(tempList,0);
		for (int i = 0; i < tempList.Length; i++) {
			if(questStars[tempList[i]] == null) {
				questStars[tempList[i]] = s;
				break;
			}
		}
	}*/

	public static string getDescriptionForStarAt(int x, int z) {
		GameObject[] tempList = new GameObject[questStars.Keys.Count];
		questStars.Keys.CopyTo(tempList,0);
		
		for (int i = 0; i < tempList.Length; i++) {
			if(tempList[i].transform.localPosition.Equals(new Vector3((x - 4)*5,3f,(z - 4)*5))) {
				return questStars[tempList[i]];
			}
		}

		return "";
	}

	public static void RemoveStarAt(int x, int z) {
		Debug.Log("Remove star from: " + x + "," + z);
		GameObject[] tempList = new GameObject[questStars.Keys.Count];
		questStars.Keys.CopyTo(tempList,0);
		GameObject forRemove = null;

		for (int i = 0; i < tempList.Length; i++) {
			if(tempList[i].transform.localPosition.Equals(new Vector3((x - 4)*5,3f,(z - 4)*5))) {
				forRemove = tempList[i];
				break;
			}
		}

		if(forRemove != null) {
			questStars.Remove(forRemove);
			Destroy(forRemove);
		}
	}
}
