using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public List<Item> inventory = new List<Item>();
	public GameObject[] playerArmor; //0: head, 1: chest, 2: arms, 3: legs
	public Weapon activeWeapon;
	private Hack activeHack;
	private GameObject weaponRef;
	private GameObject playerInventoryRef;
	public static Transform playerPos;
	private int bytes;
	private int xpBytes;
	private int bytesToNextVersion;

	public static bool inCity;

	private int deaths = 0;

	private Vector3 respawnLoc;

	public bool rolling = false;

	public AudioClip levelUp;
	private ParticleSystem levelUpParticles;

	public int levelUpSpeedScale = 30000;

	public static string version = "1.0.0";
	private string name = "TheKiniMan";
	
	public static int strength, defense, efficiency, encryption, security;
	public static int algorithmPoints = 50;
	private float integrity, rma, maxIntegrity = 100f, maxrma = 20f;

	//Armor Refs
	private Transform leftUpLegRef, rightUpLegRef, leftLegRef, rightLegRef, leftFootRef, rightFootRef, spineRef, leftShoulderRef, rightShoulderRef;
	private Transform leftArmRef, leftForearmRef, leftHandRef, rightArmRef, rightForearmRef, rightHandRef, headRef;

	private static GameObject hitInfo;

	private float deathTimer = 0f;

	public static int getMajor(string version) {
		return int.Parse (version.Split('.') [0]);
	}

	public static int getMiddle(string version) {
		return int.Parse (version.Split ('.') [1]);
	}

	public static int getMinor(string version) {
		return int.Parse (version.Split ('.') [2]);
	}

	public DirectObject getDirectObject() {
		return new DirectObject ("Player", version);
	}

	public PlayerStatus getPlayerStatus() {
		PlayerStatus.Builder builder = PlayerStatus.CreateBuilder ();

		builder.SetName (name);

		GlobalPosition.Builder positionBuilder = GlobalPosition.CreateBuilder ();
		positionBuilder.SetAreaX (MasterDriver.Instance.CurrentArea.position.x);
		positionBuilder.SetAreaY (MasterDriver.Instance.CurrentArea.position.y);
		positionBuilder.SetLocalX ((int)gameObject.transform.position.x);
		positionBuilder.SetLocalY ((int)gameObject.transform.position.z);
		builder.SetPlayerPosition (positionBuilder.Build ());

		builder.SetRotation ((int)MasterDriver.Instance.getCamera ().rotation.eulerAngles.y);

		InventoryData.Builder inventoryBuilder = InventoryData.CreateBuilder ();
		foreach (Item i in inventory) {
			if(i.gameObject.GetComponent<Weapon>() == null) {
				inventoryBuilder.AddObject(i.getDirectObject().getDirectObjectAsProtobuf());
			} else {
				Debug.Log("SAVING WEAPON");
				inventoryBuilder.AddObject(i.getDirectObject().getDirectObjectAsProtobuf((Weapon)i));
			}
		}
		builder.SetInventory (inventoryBuilder.Build ());

		builder.SetVersion (version);

		List<Point> visitedAreas = Status.playerStatus.getVisitedAreas ();
		foreach (Point p in visitedAreas) {
			PointProto.Builder pBuilder = PointProto.CreateBuilder();
			pBuilder.SetX(p.x);
			pBuilder.SetY(p.y);
			builder.AddVisitedAreas(pBuilder.Build());
		}

		return builder.Build ();
	}

	public void setPlayerStatusWithoutPosition(PlayerStatus status) {
		name = status.Name;
		
		List<Point> visitedAreas = new List<Point> ();
		IList<PointProto> storedAreas = status.VisitedAreasList;
		foreach (PointProto p in storedAreas) {
			visitedAreas.Add(new Point(p.X, p.Y));
		}
		
		Status.playerStatus.setVisitedAreas (visitedAreas);
		
		InventoryData inv = status.Inventory;
		inventory.Clear ();
		foreach (DirectObjectProtocol item in inv.ObjectList) {
			GameObject obj = (GameObject)MasterDriver.Instance.getItemFromProtobuf(item);
			obj.GetComponent<Item>().name = item.Type;
			
			if (item.HasItemInformation && item.ItemInformation.HasSaveVersion && obj.GetComponent<Weapon>() != null) {
				obj.GetComponent<Weapon>().version = item.ItemInformation.SaveVersion;
			}
			
			PickUpItem(obj);
		}
		
		version = status.Version;
	}

	public void setPlayerStatus(PlayerStatus status) {
		name = status.Name;
//		Debug.LogError(this.GetName());
		List<Point> visitedAreas = new List<Point> ();
		IList<PointProto> storedAreas = status.VisitedAreasList;
		foreach (PointProto p in storedAreas) {
			visitedAreas.Add(new Point(p.X, p.Y));
		}

		Status.playerStatus.setVisitedAreas (visitedAreas);

		gameObject.transform.position = new Vector3 (status.PlayerPosition.LocalX, gameObject.transform.position.y, status.PlayerPosition.LocalY);

		InventoryData inv = status.Inventory;
		inventory.Clear ();
		foreach (DirectObjectProtocol item in inv.ObjectList) {
			GameObject obj = (GameObject)MasterDriver.Instance.getItemFromProtobuf(item);
			obj = GameObject.Instantiate(obj);
			obj.SetActive(false);
			obj.GetComponent<Item>().name = item.Type;

			if (item.HasItemInformation && item.ItemInformation.HasSaveVersion && obj.GetComponent<Weapon>() != null) {
				obj.GetComponent<Weapon>().version = item.ItemInformation.SaveVersion;
			}

			PickUpItem(obj);
		}

		version = status.Version;
	}

	void Start () {
		weaponRef = GameObject.Find("PlayerWeaponObj");
		if(PersistentInfo.playerName != null && !PersistentInfo.playerName.Equals("")) {
			if(PersistentInfo.playerName.Contains("\n")) {
				PersistentInfo.playerName = PersistentInfo.playerName.Substring(0,PersistentInfo.playerName.Length-1);
			}
			this.name = PersistentInfo.playerName;
		} else if(PersistentInfo.saveFile > 0 && !MasterDriver.bossLevel) {
			MasterDriver.Instance.loadGame = true;
		}
		
		levelUpParticles = GameObject.Find("LevelUpParticles").GetComponent<ParticleSystem>();

		//Need to figure out a better way to load the hitinfo prefab
		hitInfo = Resources.Load<GameObject>("Info/HitInfo");

		//setting initial integrity and rma values
		integrity = maxIntegrity;
		rma = maxrma;

		//initializing the references to the player inventory, armor points, and weaponhand
		playerInventoryRef = GameObject.Find("PlayerInventory");
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*10 + (int.Parse(version.Split('.')[1]))*50)*levelUpSpeedScale;

		//setting up inventory
		inventory = new List<Item>();
		if(weaponRef.transform.childCount != 0) {
			inventory.Add(weaponRef.transform.GetChild(0).GetComponent<Item>());
		}
		for (int i = 0; i < playerInventoryRef.transform.childCount; i++) {
			inventory.Add(playerInventoryRef.transform.GetChild(i).GetComponent<Item>());
		}

		//setting up playerarmor
		playerArmor = new GameObject[4];

		leftUpLegRef = transform.Find("Character1_Reference/Character1_Hips/Character1_LeftUpLeg").GetComponent<Transform>();
		rightUpLegRef = transform.Find("Character1_Reference/Character1_Hips/Character1_RightUpLeg").GetComponent<Transform>();
		leftLegRef = transform.Find("Character1_Reference/Character1_Hips/Character1_LeftUpLeg/Character1_LeftLeg").GetComponent<Transform>();
		rightLegRef = transform.Find("Character1_Reference/Character1_Hips/Character1_RightUpLeg/Character1_RightLeg").GetComponent<Transform>();
		leftFootRef = transform.Find("Character1_Reference/Character1_Hips/Character1_LeftUpLeg/Character1_LeftLeg/Character1_LeftFoot").GetComponent<Transform>();
		rightFootRef = transform.Find("Character1_Reference/Character1_Hips/Character1_RightUpLeg/Character1_RightLeg/Character1_RightFoot").GetComponent<Transform>();
		spineRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine").GetComponent<Transform>();
		leftShoulderRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_LeftShoulder").GetComponent<Transform>();
		rightShoulderRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_RightShoulder").GetComponent<Transform>();
		leftArmRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_LeftShoulder/Character1_LeftArm").GetComponent<Transform>();
		leftForearmRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_LeftShoulder/Character1_LeftArm/Character1_LeftForeArm").GetComponent<Transform>();
		leftHandRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_LeftShoulder/Character1_LeftArm/Character1_LeftForeArm/Character1_LeftHand").GetComponent<Transform>();
		rightArmRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_RightShoulder/Character1_RightArm").GetComponent<Transform>();
		rightForearmRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_RightShoulder/Character1_RightArm/Character1_RightForeArm").GetComponent<Transform>();
		rightHandRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_RightShoulder/Character1_RightArm/Character1_RightForeArm/Character1_RightHand").GetComponent<Transform>();
		headRef = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_Neck/Character1_Head").GetComponent<Transform>();

		//setting up initial weapon and hack (not the best way to do this since
		//it requires that the first item in the inventory prefab needs to be a hack
		if(inventory.Count > 1) {
			activeWeapon = (Weapon)inventory[0];
			activeHack = (Hack)inventory[1];
			SetActiveItem(0);
			SetActiveItem(1);
		}

		if(!Application.loadedLevel.Equals("KartikTesting")) {
			respawnLoc = transform.position;
		}

		//sets up quickaccessitems and makes the canvas update the inventory ui
		PlayerCanvas.UpdateInventory();
		Debug.LogError(GetName());
	}

	void Update () {
		playerPos = transform;
		rma += Time.deltaTime/4f * (efficiency + 1);
		integrity += Time.deltaTime/10f * (efficiency + 1);
		if(inCity) {
			integrity += Time.deltaTime*2f;
			rma += Time.deltaTime*2f;
		}
		if (rma > maxrma) {
			rma = maxrma;
		} else if (rma < 0) {
			rma = 0;
		}

		if(integrity > maxIntegrity) {
			integrity = maxIntegrity;
		} else if (integrity < 0 && deathTimer == 0) {
			Die();
			integrity = 0;
		}

		maxrma = (encryption/2f + 1)*20f;
		maxIntegrity = (security/10f + 1)*50f;

		if(deathTimer > 0) {
			deathTimer += Time.deltaTime;
			if(deathTimer > 2f) {
				PlayerControl.immobile = false;
				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(true);
				deathTimer = 0;
				integrity = maxIntegrity;
				rma = maxrma;
				if(Application.loadedLevelName.Equals("KartikTesting")) {
					xpBytes = 0;
					if(MasterDriver.Instance.fightingFinalBoss) {
						MasterDriver.Instance.dieInFinalBoss();	
					}
					MasterDriver.Instance.goToCity();
				} else {
					transform.position = respawnLoc;
				}
			}
		}
	}

	public void Attack (int combo) {
		activeWeapon.Attack(Mathf.Max(0,Random.Range(strength-2, strength)) + (activeWeapon.GetDamage() * combo));
		ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (activeWeapon.getDirectObject(), ActionType.USE_ITEM));
	}

	//Used to control gun shooting animation
	public bool CanAttack() {
		return activeWeapon.CanAttack();
	}

	//Called whenever the player presses a number to quick select
	public void SetActiveItem (int val) {
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/weaponEquip02",transform.position,PlayerPrefs.GetFloat("MasterVolume")/4f);
		if(inventory.Count >= val + 1) {
			DirectObject equiped = null;

			if(inventory[val].GetType().IsSubclassOf(typeof(Weapon))) { 
				activeWeapon = (Weapon)inventory[val];
				for(int i = 0; i < playerInventoryRef.transform.childCount; i++) {
					if(playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>() != null
					   && playerInventoryRef.transform.GetChild(i).GetComponent<Weapon>().Equals(activeWeapon)) {
						if (weaponRef.transform.childCount > 0) {
							weaponRef.transform.GetChild(0).gameObject.SetActive(false);
							weaponRef.transform.GetChild(0).transform.parent = playerInventoryRef.transform;
						}
						playerInventoryRef.transform.GetChild(i).parent = weaponRef.transform;
						weaponRef.transform.GetChild(0).gameObject.SetActive(true);
						weaponRef.transform.GetChild(0).localPosition = Vector3.zero;
						weaponRef.transform.GetChild(0).localEulerAngles = Vector3.zero;
						weaponRef.transform.GetChild(0).localScale = new Vector3(1,1,1);

						equiped = activeWeapon.getDirectObject(); //acive weapon is selected weapon?
					}
				}
			} else {
				activeHack = (Hack)inventory[val];
				equiped = activeHack.getDirectObject();
			}

//			ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (equiped, ActionType.EQUIP_ITEM));
		}
	}

	public void Hack () {
		if(activeHack != null) {
			activeHack.Call(this);
			ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (activeHack.getDirectObject(), ActionType.USE_ITEM));
		}
	}

	public float GetRMAPercentage() {
		return rma/maxrma;
	}

	public float GetIntegrityPercentage() {
		return integrity/maxIntegrity;
	}

	public string GetIntegrityRegen() {
		return "+" + ((efficiency+1)/5f).ToString("F2") + "/s";
	}

	public string GetRMARegen() {
		return "+" + ((efficiency+1)/7f).ToString("F2") + "/s";
	}

	public string GetRMAValueText() {
		return ((int)rma) + "/" + ((int)maxrma);
	}

	public string GetIntegrityValueText() {
		return ((int)integrity) + "/" + ((int)maxIntegrity);
	}

	//used to start weaponfx (the green trail on the lightstick)
	public void StartAttack() {
		activeWeapon.StartAttack();
	}

	public string GetName() {
		if(deaths == 0) {
			return name + "_" + version;
		} else {
			return name + "_" + version + "R" + deaths;
		}
	}

	public string GetBlocking() {
		return "(" + Mathf.Max(0,defense-10) + "/" + defense + ")";
	}

	public float XPPercentage() {
		return (float)xpBytes/bytesToNextVersion;
	}

	public void StopAttack() {
		if(activeWeapon != null) {
			activeWeapon.StopAttack();
		}
	}

	//Called any time the player gets bytes
	public void AddBytes(int val) {
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/XPParticle", transform.position,PlayerPrefs.GetFloat("MasterVolume"));
		bytes += val;
		xpBytes += val;
		if (activeWeapon != null) {
			activeWeapon.AddBytes(val);
		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
		while (xpBytes >= bytesToNextVersion) {
			LevelUp();
		}
	}

	public void DeleteItem(int index) {
		if(inventory[index].GetType().IsSubclassOf(typeof(Weapon))) {
			Weapon tempWeapon = (Weapon)inventory[index];
			for(int i = 0; i < (Utility.ComparableVersionInt(tempWeapon.version)/10); i++) {
				GameObject tempbyte = (GameObject) GameObject.Instantiate(Utility.GetByteObject(), transform.position + Vector3.up, Quaternion.identity);
				if(tempWeapon.RarityVal.Equals(Rarity.Common)) {
					tempbyte.GetComponent<Byte>().val = 10000;
				} else if(tempWeapon.RarityVal.Equals(Rarity.Uncommon)) {
					tempbyte.GetComponent<Byte>().val = 25000;
				} else if(tempWeapon.RarityVal.Equals(Rarity.Rare)) {
					tempbyte.GetComponent<Byte>().val = 50000;
				} else if(tempWeapon.RarityVal.Equals(Rarity.Anomaly)) {
					tempbyte.GetComponent<Byte>().val = 100000;
				}
			}
		} else if(inventory[index].GetType().IsSubclassOf(typeof(Hack))) {
			Hack tempHack = (Hack)inventory[index];
			for(int i = 0; i < 20; i++) {
				GameObject tempbyte = (GameObject) GameObject.Instantiate(Utility.GetByteObject(), transform.position + Vector3.up, Quaternion.identity);
				if(tempHack.RarityVal.Equals(Rarity.Common)) {
					tempbyte.GetComponent<Byte>().val = 10000;
				} else if(tempHack.RarityVal.Equals(Rarity.Uncommon)) {
					tempbyte.GetComponent<Byte>().val = 25000;
				} else if(tempHack.RarityVal.Equals(Rarity.Rare)) {
					tempbyte.GetComponent<Byte>().val = 50000;
				} else if(tempHack.RarityVal.Equals(Rarity.Anomaly)) {
					tempbyte.GetComponent<Byte>().val = 100000;
				}
			}
		}

		if(activeWeapon == inventory[index]) {
			activeWeapon = null;
		}
		if(activeHack == inventory[index]) {
			activeHack = null;
		}

		inventory.RemoveAt(index);
		PlayerCanvas.updateInventoryUI = true;
	}

	private void Die() {
		if(Application.loadedLevelName.Contains("Tutorial")) {
			CityHelp.helpMode = -2;
		}
		deaths++;
		PlayerControl.immobile = true;
		levelUpParticles.startColor = Color.red;
		levelUpParticles.Emit(1000000);
		levelUpParticles.startColor = Color.yellow;
		FMOD_StudioSystem.instance.PlayOneShot("event:/boss/bossAttackB", transform.position,PlayerPrefs.GetFloat("MasterVolume")/2f);
		deathTimer = 0.0001f;
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(false);
		FollowPlayer.MoveCamFast();
	}

	private void LevelUp() {
		xpBytes -= bytesToNextVersion;
		//INCREASE PLAYER STATS
		algorithmPoints += 4;
//		if(int.Parse(version.Split('.')[2]) + 1 < 10) {
//			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1]))*1) + "." + ((int.Parse(version.Split('.')[2])) + 1);
//		} else if(int.Parse(version.Split('.')[1]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + "." + ((int.Parse(version.Split('.')[1])*1) + 1) + ".0";
//		} else {
//			version = (int.Parse(version.Split('.')[0])*1 + 1) + ".0.0";
//		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (this.getDirectObject(), ActionType.LEVEL_UP));
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/playerLevelUp", transform.position,PlayerPrefs.GetFloat("MasterVolume"));
//		strength++;
//		efficiency++;
//		encryption++;
//		security++;
//		defense++;
		levelUpParticles.Emit(1000000);
	}

	public int GetBytes() {
		return bytes;
	}

	public int GetXPBytes() {
		return xpBytes;
	}

	public Weapon GetWeapon() {
		return activeWeapon;
	}

	public Hack GetHack() {
		return activeHack;
	}

	//Called by any attack that hits the player
	public void GetDamaged(float damage, bool crit) {
		if(!rolling) {
			FMOD_StudioSystem.instance.PlayOneShot("event:/player/playerDamage", transform.position,PlayerPrefs.GetFloat("MasterVolume")/2f);
			GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
			temp.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.red;
			if (crit) {
				int blocking = Random.value < 0.5f ? Mathf.Min(Mathf.Max(0,Random.Range(defense - 10, defense + 1)),(int)damage*2) : 0;
				integrity -= damage*2 - blocking;
				temp.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.black;
				temp.GetComponent<TextMesh>().text = "" + damage*2 + "!";
				if(blocking > 0) {
					GameObject tempblock = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
					tempblock.GetComponent<TextMesh>().text = "-" + blocking;
					tempblock.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.green;
					tempblock.GetComponent<HitInfoScript>().isBlockingText = true;
					tempblock.GetComponent<Transform>().parent = temp.transform;
					tempblock.GetComponent<Transform>().localPosition = Vector3.zero - (Vector3.up*2.5f);
					tempblock.GetComponent<Rigidbody>().isKinematic = true;
				}
			} else {
				int blocking = Random.value < 0.5f ? Mathf.Min(Mathf.Max(0,Random.Range(defense - 10, defense + 1)),(int)damage) : 0;
				integrity -= damage  - blocking;
				temp.GetComponent<TextMesh>().text = "" + damage;
				if(blocking > 0) {
					GameObject tempblock = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
					tempblock.GetComponent<TextMesh>().text = "-" + blocking;
					tempblock.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.green;
					tempblock.GetComponent<HitInfoScript>().isBlockingText = true;
					tempblock.GetComponent<Transform>().parent = temp.transform;
					tempblock.GetComponent<Transform>().localPosition = Vector3.zero - (Vector3.up*2.5f);
					tempblock.GetComponent<Rigidbody>().isKinematic = true;
				}
			}
		}
	}

	public void GetDamaged(float damage) {
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/playerDamage", transform.position,PlayerPrefs.GetFloat("MasterVolume")/2f);
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
		temp.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.red;
		integrity -= damage;
		temp.GetComponent<TextMesh>().text = "" + damage;
	}

	public string ToString() {
		return name + "_" + version +
			"\nStrength: " + strength +
			"\nDefense: " + defense +
			"\nEfficiency: " + efficiency +
			"\nSecurity: " + security +
			"\nEncryption: " + encryption;
	}

	public bool ExpendRMA(float amount) {
		if(rma - amount >= 0) {
			rma -= amount;
			return true;
		} else {
			if(Time.frameCount%3==0) {
				GetDamaged(1);
			}
			return false;
		}
	}

	public void PickUpItem(GameObject item) {
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/weaponEquip02",transform.position,PlayerPrefs.GetFloat("MasterVolume")/6f);
		GameObject temp = (GameObject) Instantiate(item, Vector3.zero, Quaternion.identity);
		temp.transform.parent = playerInventoryRef.transform;
		temp.transform.localPosition = Vector3.zero;
		if(inventory.Count < 10 && (temp.GetComponent<Weapon>() != null || temp.GetComponent<Hack>() != null)) {
			inventory.Add(temp.GetComponent<Item>());
		} else if (inventory.Capacity < 11) {
			inventory.Capacity = 11;
			inventory[10] = temp.GetComponent<Item>();
		} else {
			inventory.Add(temp.GetComponent<Item>());
		}

		if(temp.GetComponent<Hack>() == null) {
			temp.SetActive(false);
		} else {
			temp.SetActive(true);
		}

		PlayerCanvas.UpdateInventory();

		if(item.GetComponent<Armor>() != null) {
			EquipArmor(temp);
		}

		if(activeWeapon == null && item.GetComponent<Weapon>() != null) {
			SetActiveItem(0);
		}

		if(activeHack == null && item.GetComponent<Hack>() != null) {
			SetActiveItem(1);
		}

		ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (temp.GetComponent<Item> ().getDirectObject (), ActionType.PICKED_UP_OBJECT));
	}

	public void EquipArmor(GameObject armor) {
		if(armor.GetComponent<Armor>().armorType == ArmorType.Leg) {
			armor.SetActive(true);

			armor.transform.GetChild(0).GetChild(0).parent = leftUpLegRef.transform;
			leftUpLegRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftUpLegRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftUpLegRef.transform.GetChild(1).localScale = Vector3.one;

			armor.transform.GetChild(0).GetChild(0).parent = leftLegRef.transform;
			leftLegRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftLegRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftLegRef.transform.GetChild(1).localScale = Vector3.one;

			armor.transform.GetChild(0).GetChild(0).parent = leftFootRef.transform;
			leftFootRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftFootRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftFootRef.transform.GetChild(1).localScale = Vector3.one;

			armor.transform.GetChild(1).GetChild(0).parent = rightUpLegRef.transform;
			rightUpLegRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightUpLegRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightUpLegRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(1).GetChild(0).parent = rightLegRef.transform;
			rightLegRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightLegRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightLegRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(1).GetChild(0).parent = rightFootRef.transform;
			rightFootRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightFootRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightFootRef.transform.GetChild(1).localScale = Vector3.one;
		} else if(armor.GetComponent<Armor>().armorType == ArmorType.Chest) {
			armor.SetActive(true);

			armor.transform.GetChild(0).parent = spineRef.transform;
			spineRef.transform.GetChild(1).localPosition = Vector3.zero;
			spineRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			spineRef.transform.GetChild(1).localScale = Vector3.one;

			armor.transform.GetChild(0).parent = leftShoulderRef.transform;
			leftShoulderRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftShoulderRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftShoulderRef.transform.GetChild(1).localScale = Vector3.one;

			armor.transform.GetChild(0).parent = rightShoulderRef.transform;
			rightShoulderRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightShoulderRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightShoulderRef.transform.GetChild(1).localScale = Vector3.one;
		} else if(armor.GetComponent<Armor>().armorType == ArmorType.Arm) {
			armor.SetActive(true);
			
			armor.transform.GetChild(0).GetChild(0).parent = leftArmRef.transform;
			leftArmRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftArmRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftArmRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(0).GetChild(0).parent = leftForearmRef.transform;
			leftForearmRef.transform.GetChild(1).localPosition = Vector3.zero;
			leftForearmRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			leftForearmRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(0).GetChild(0).parent = leftHandRef.transform;
			leftHandRef.transform.GetChild(3).localPosition = Vector3.zero;
			leftHandRef.transform.GetChild(3).localEulerAngles = Vector3.zero;
			leftHandRef.transform.GetChild(3).localScale = Vector3.one;
			
			armor.transform.GetChild(1).GetChild(0).parent = rightArmRef.transform;
			rightArmRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightArmRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightArmRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(1).GetChild(0).parent = rightForearmRef.transform;
			rightForearmRef.transform.GetChild(1).localPosition = Vector3.zero;
			rightForearmRef.transform.GetChild(1).localEulerAngles = Vector3.zero;
			rightForearmRef.transform.GetChild(1).localScale = Vector3.one;
			
			armor.transform.GetChild(1).GetChild(0).parent = rightHandRef.transform;
			rightHandRef.transform.GetChild(4).localPosition = Vector3.zero;
			rightHandRef.transform.GetChild(4).localEulerAngles = Vector3.zero;
			rightHandRef.transform.GetChild(4).localScale = Vector3.one;
		} else if(armor.GetComponent<Armor>().armorType == ArmorType.Head) {
			armor.SetActive(true);

			armor.transform.GetChild(0).parent = headRef.transform;
			headRef.transform.GetChild(0).localPosition = Vector3.zero;
			headRef.transform.GetChild(0).localEulerAngles = Vector3.zero;
			headRef.transform.GetChild(0).localScale = Vector3.one;
		}
	}

	public List<Item> quickAccessItems {
		get {
			List<Item> temp = new List<Item>();
			for(int i = 0; i < 10 && i < inventory.Count; i++) {
				temp.Add(inventory[i]);
			}
//			Debug.Log(temp.Count);
//			Debug.Log(temp);
			return temp;
		}
	}

}
