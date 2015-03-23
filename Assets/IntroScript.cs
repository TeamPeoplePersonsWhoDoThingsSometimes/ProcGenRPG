using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class IntroScript : MonoBehaviour {

	public InputField input;
	public Text classText;

	private Color temp;

	private int typeIndex;

	private bool animating;

	public RectTransform loadingBar;
	private float localLoadingValue;

	private int speed = 1;
	private float animSpeed = 1;

	AsyncOperation loadNextLevel;

	private string classTextString = @"
	public List<Item> inventory = new List<Item>();
	public GameObject[] playerArmor; //0: head, 1: chest, 2: arms, 3: legs
	private Weapon activeWeapon;
	private Hack activeHack;
	private GameObject weaponRef;
	private GameObject playerInventoryRef;
	public static Transform playerPos;
	private int bytes;
	private int xpBytes;
	private int bytesToNextVersion;

	private int levelUpSpeedScale = 10000;

	public static string version = ""1.0.0"";
	
	public static int strength, defense, efficiency, encryption, security;
	public static int algorithmPoints;
	private float integrity, rma, maxIntegrity = 100f, maxrma = 20f;

	//Armor Refs
	private Transform leftUpLegRef, rightUpLegRef, leftLegRef, rightLegRef, leftFootRef, rightFootRef, spineRef, leftShoulderRef, rightShoulderRef;
	private Transform leftArmRef, leftForearmRef, leftHandRef, rightArmRef, rightForearmRef, rightHandRef, headRef;

	private static GameObject hitInfo;

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
		return new DirectObject (""Player"", version);
	}

	void Start () {
		//Need to figure out a better way to load the hitinfo prefab
		hitInfo = Resources.Load<GameObject>(""Info/HitInfo"");

		//setting initial integrity and rma values
		integrity = maxIntegrity;
		rma = maxrma;

		//initializing the references to the player inventory, armor points, and weaponhand
		playerInventoryRef = GameObject.Find(""PlayerInventory"");
		weaponRef = GameObject.Find(""PlayerWeaponObj"");
		playerPos = transform;
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;

		//setting up inventory
		inventory = new List<Item>();
		if(weaponRef.transform.childCount != 0) {
			inventory.Add(weaponRef.transform.GetChild(0).GetComponent<Item>());
			for (int i = 0; i < playerInventoryRef.transform.childCount; i++) {
				inventory.Add(playerInventoryRef.transform.GetChild(i).GetComponent<Item>());
			}
		}

		//setting up playerarmor
		playerArmor = new GameObject[4];

		//sets up quickaccessitems and makes the canvas update the inventory ui
		PlayerCanvas.UpdateInventory();
	}

	void Update () {
		playerPos = transform;
		rma += Time.deltaTime/2f * (encryption + 1);
		integrity += Time.deltaTime/2f * (security + 1);
		if (rma > maxrma) {
			rma = maxrma;
		} else if (rma < 0) {
			rma = 0;
		}

		if(integrity > maxIntegrity) {
			integrity = maxIntegrity;
		} else if (integrity < 0) {
			integrity = 0;
		}

		maxrma = (encryption/2f + 1)*20f;
		maxIntegrity = (security/5f + 1)*100f;	
	}

	public void Attack (int combo) {
		activeWeapon.Attack(strength + (activeWeapon.GetDamage() * combo));
		ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (activeWeapon.getDirectObject(), ActionType.USE_ITEM));
	}

	//Used to control gun shooting animation
	public bool CanAttack() {
		return activeWeapon.CanAttack();
	}

	//Called whenever the player presses a number to quick select
	public void SetActiveItem (int val) {
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

	//used to start weaponfx (the green trail on the lightstick)
	public void StartAttack() {
		activeWeapon.StartAttack();
	}

	public string GetName() {
		return name + ""_"" + version;
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
		bytes += val;
		xpBytes += val;
		if (activeWeapon != null) {
			activeWeapon.AddBytes(val);
		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		while (xpBytes >= bytesToNextVersion) {
			LevelUp();
		}
	}

	private void LevelUp() {
		xpBytes -= bytesToNextVersion;
		//INCREASE PLAYER STATS
		algorithmPoints += 2;
		if(int.Parse(version.Split('.')[2]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + ""."" + ((int.Parse(version.Split('.')[1]))*1) + ""."" + ((int.Parse(version.Split('.')[2])) + 1);
		} else if(int.Parse(version.Split('.')[1]) + 1 < 10) {
			version = ((int.Parse(version.Split('.')[0]))*1) + ""."" + ((int.Parse(version.Split('.')[1])*1) + 1) + "".0"";
		} else {
			version = (int.Parse(version.Split('.')[0])*1 + 1) + "".0.0"";
		}
		bytesToNextVersion = ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])))*levelUpSpeedScale;
		ActionEventInvoker.primaryInvoker.invokeAction (new PlayerAction (this.getDirectObject(), ActionType.LEVEL_UP));
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
		GameObject temp = (GameObject)Instantiate(hitInfo,this.transform.position + new Vector3(0,1,0), hitInfo.transform.rotation);
		temp.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.red;
		if (crit) {
			integrity -= damage*2;
			temp.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = Color.black;
			temp.GetComponent<TextMesh>().text = "" + damage*2 + ""!"";
		} else {
			integrity -= damage;
			temp.GetComponent<TextMesh>().text = "" + damage;
		}
	}

	public string ToString() {
		return name + ""_"" + version +
			""\nStrength: "" + strength +
			""\nDefense: "" + defense +
			""\nEfficiency: "" + efficiency +
			""\nSecurity: "" + security +
			""\nEncryption: "" + encryption;
	}

	public bool ExpendRMA(float amount) {
		if(rma - amount >= 0) {
			rma -= amount;
			return true;
		} else {
			return false;
		}
	}

	public void PickUpItem(GameObject item) {
		GameObject temp = (GameObject) Instantiate(item, Vector3.zero, Quaternion.identity);
		temp.transform.parent = playerInventoryRef.transform;
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
			return temp;
		}
	};";

	// Use this for initialization
	void Start () {
		temp = input.GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update () {
		if(animating && Time.frameCount % Mathf.Max(1,10-(int)(typeIndex/2f)) == 0) {
			typeIndex++;
			if(typeIndex > 50) {
				typeIndex++;
				speed = 2;
			}

			if(typeIndex > 100) {
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				speed = 50;
			}

			if(typeIndex > 500) {
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
				typeIndex++;
			}
			speed = 300;
			classText.text = classTextString.Substring(0,typeIndex);
		}
		animSpeed = Mathf.MoveTowards(animSpeed, speed, Time.deltaTime*(typeIndex/500f));
		if(animating) {
			classText.GetComponent<RectTransform>().parent.Translate(Vector3.up/7f*animSpeed);
			Camera.main.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = animSpeed/7f;
			Camera.main.GetComponent<MotionBlur>().blurAmount = animSpeed/50f;
		}

		if(animating && classText.text.Length > 8000) {
			animating = false;
			classText.text = "";
			classText.GetComponent<RectTransform>().parent.GetComponent<CanvasGroup>().alpha = 0;
			classText.GetComponent<RectTransform>().parent.parent.GetChild(1).GetComponent<CanvasGroup>().alpha = 1;
			Camera.main.GetComponent<MotionBlur>().blurAmount = 0.5f;
		}

		if(!animating && classText.GetComponent<RectTransform>().parent.parent.GetChild(1).GetComponent<CanvasGroup>().alpha == 1) {
			Camera.main.fieldOfView -= Time.deltaTime/2f;
		}

		if(Camera.main.fieldOfView < 60f) {
			if(loadNextLevel == null) {
				loadNextLevel = Application.LoadLevelAsync(2);
			} else {
				loadNextLevel.allowSceneActivation = false;
				localLoadingValue = Mathf.MoveTowards(localLoadingValue, loadNextLevel.progress, Time.deltaTime/2f);
				loadingBar.localScale = new Vector3(localLoadingValue, 1f,1f);
				if(localLoadingValue >= 0.9f) {
					loadNextLevel.allowSceneActivation = true;
				}
			}
		}
	}

	public void PressedEnter() {
		if(input.text.Length > 2) {
			animating = true;
		}
	}
}
