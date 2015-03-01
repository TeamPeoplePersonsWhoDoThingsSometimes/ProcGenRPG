using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerCanvas : MonoBehaviour {

	public static bool cinematicMode;

	public Sprite common, uncommon, rare, anomaly;

	public static List<GameObject> enemieswithhealthbars;
	public static bool updateInventoryUI;

	public GameObject enemyhealthbarprefab;

	private Animator playerAnim;

	private Player playerRef;

	private CanvasGroup inGameGUI;
	private Image bg, a1, a2, cursor;
	private Image byteXP, byteXPSmooth;
	private Text byteText, playerName;

	private CanvasGroup consoleGUI;

	private RectTransform weaponXPGroup;
	private Text curWeapon;
	private Text weaponXPPercentage;
	private Image weaponXPImg;
	private float weaponXPTimeOffset;
	private int tempWeaponXPVal;

	private Button strengthButton, defenseButton, efficiencyButton, securityButton, encryptionButton;
	private Text playerStrengthText, playerDefenseText, playerEfficiencyText, playerSecurityText, playerEncryptionText, algorithmPointsText, weaponStatsInfo, hackStatsInfo;

	private Text integrityPercentage, RMAPercentage;
	private Image integrityBar, RMABar;

	public static bool inConsole = false;

	private GameObject minimap, mainCam, mainCamWithEffects, uiCam;
	private Vector3 playerCanvasOffset;

	private RectTransform quickAccessBar, activeWeaponIcon, activeHackIcon;

	private GameObject VRCursor;

	private GameObject enemyHealthBars;

	private GameObject inventoryItemContainer, mouseOverInfo;
	
	private Vector2 dragDelta = Vector2.zero;
	private Vector2 mousePressedLocation;
	private Vector2 dragStart;

	public static void RegisterEnemyHealthBar(GameObject enemy) {
		if(enemieswithhealthbars == null) {
			enemieswithhealthbars = new List<GameObject>();
		}
		if(!enemieswithhealthbars.Contains(enemy)) {
			enemieswithhealthbars.Add(enemy);
		}
	}

	// Use this for initialization
	void Start () {
		playerCanvasOffset = this.transform.position - Player.playerPos.position;

		minimap = GameObject.Find("MiniMapCam");
		mainCam = GameObject.Find("Main Camera");
		uiCam = GameObject.Find("UICam");
		mainCamWithEffects = GameObject.Find("Main Camera With Effects");

		playerAnim = GameObject.Find("PlayerObj").GetComponent<Animator>();
		playerRef = GameObject.Find("PlayerObj").GetComponent<Player>();

		inGameGUI = transform.GetChild(0).GetComponent<CanvasGroup>();
		consoleGUI = transform.GetChild(1).GetComponent<CanvasGroup>();
	
		a1 = GameObject.Find("Attack1").GetComponent<Image>();
		a2 = GameObject.Find("Attack2").GetComponent<Image>();
		bg = GameObject.Find("AttackBG").GetComponent<Image>();
		cursor = GameObject.Find("AttackPointer").GetComponent<Image>();

		byteText = GameObject.Find("ByteText").GetComponent<Text>();
		playerName = GameObject.Find("PlayerName").GetComponent<Text>();
		playerName.text = playerRef.GetName();
		byteXP = GameObject.Find("ByteXP").GetComponent<Image>();
		byteXPSmooth = GameObject.Find("ByteXPSmooth").GetComponent<Image>();

		weaponXPGroup = GameObject.Find("WeaponXP").GetComponent<RectTransform>();
		curWeapon = GameObject.Find("WeaponName").GetComponent<Text>();
		weaponXPImg = GameObject.Find("WeaponByteXP").GetComponent<Image>();
		tempWeaponXPVal = playerRef.GetWeapon().GetBytes();
		curWeapon.text = playerRef.GetWeapon().GetName();
		weaponXPPercentage = GameObject.Find("WeaponXPPercentage").GetComponent<Text>();

		strengthButton = GameObject.Find("StrengthButton").GetComponent<Button>();
		defenseButton = GameObject.Find("DefenseButton").GetComponent<Button>();
		efficiencyButton = GameObject.Find("EfficiencyButton").GetComponent<Button>();
		securityButton = GameObject.Find("SecurityButton").GetComponent<Button>();
		encryptionButton = GameObject.Find("EncryptionButton").GetComponent<Button>();

		playerStrengthText = GameObject.Find("PlayerStrengthText").GetComponent<Text>();
		playerDefenseText= GameObject.Find("PlayerDefenseText").GetComponent<Text>();
		playerEfficiencyText = GameObject.Find("PlayerEfficiencyText").GetComponent<Text>();
		playerSecurityText = GameObject.Find("PlayerSecurityText").GetComponent<Text>();
		playerEncryptionText = GameObject.Find("PlayerEncryptionText").GetComponent<Text>();
		algorithmPointsText = GameObject.Find("AlgorithmPointsText").GetComponent<Text>();
		weaponStatsInfo = GameObject.Find("WeaponStatInfo").GetComponent<Text>();
		hackStatsInfo = GameObject.Find("HackStatInfo").GetComponent<Text>();

		integrityBar = GameObject.Find("IntegrityBar").GetComponent<Image>();
		integrityPercentage = GameObject.Find("IntegrityPercentText").GetComponent<Text>();
		RMABar = GameObject.Find("RMABar").GetComponent<Image>();
		RMAPercentage = GameObject.Find("RMAPercentText").GetComponent<Text>();

		quickAccessBar = GameObject.Find("QuickAccessBar").GetComponent<RectTransform>();
		activeWeaponIcon = GameObject.Find("ActiveWeaponIcon").GetComponent<RectTransform>();
		activeHackIcon = GameObject.Find("ActiveHackIcon").GetComponent<RectTransform>();

		enemyHealthBars = GameObject.Find("EnemyHealthBars");

		VRCursor = GameObject.Find("VRCursor");

		inventoryItemContainer = GameObject.Find("InventoryItemContainer");
		mouseOverInfo = GameObject.Find("MouseOverInfo");

		playerName.text = playerRef.GetName();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		AnimatorStateInfo info = playerAnim.GetCurrentAnimatorStateInfo(0);
		if (info.IsName("Base.Slash1")) {
			bg.enabled = true;
			a1.enabled = true;
			a2.enabled = false;
			cursor.enabled = true;
			cursor.rectTransform.localPosition = new Vector3((info.normalizedTime)*4 - 2, cursor.rectTransform.localPosition.y, cursor.rectTransform.localPosition.z);
		} else if (info.IsName("Base.Slash2")) {
			bg.enabled = true;
			a1.enabled = false;
			a2.enabled = true;
			cursor.enabled = true;
			cursor.rectTransform.localPosition = new Vector3((info.normalizedTime)*4 - 2, cursor.rectTransform.localPosition.y, cursor.rectTransform.localPosition.z);
		} else {
			bg.enabled = false;
			a1.enabled = false;
			a2.enabled = false;
			cursor.enabled = false;
		}
	}

	public static void UpdateInventory() {
		updateInventoryUI = true;
	}

	private Sprite GetSprite(Rarity r) {
		switch(r) {
			case Rarity.Common:
				return common;
			case Rarity.Uncommon:
				return uncommon;
			case Rarity.Rare:
				return rare;
			case Rarity.Anomaly:
				return anomaly;
			default:
				return common;
		}
	}

	void Update () {
		/*** Updates QuickAccessItems ***/
		for(int i = 0; i < playerRef.quickAccessItems.Count; i++) {
			if(playerRef.quickAccessItems[i] != null) {
				quickAccessBar.transform.GetChild(i).GetComponent<Image>().sprite = GetSprite(playerRef.quickAccessItems[i].RarityVal);
				if(playerRef.GetWeapon().Equals(playerRef.quickAccessItems[i])) {
					activeWeaponIcon.SetParent(quickAccessBar.GetChild(i), false);
				}
				if(playerRef.GetHack().Equals(playerRef.quickAccessItems[i])) {
					activeHackIcon.SetParent(quickAccessBar.GetChild(i), false);
				}
				quickAccessBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = playerRef.quickAccessItems[i].icon;
			}
		}

		/*** Handles instantiation of enemyhealthbars ***/
		if(enemieswithhealthbars != null) {
			foreach (GameObject g in enemieswithhealthbars) {
				if(g != null) {
					GameObject temp = (GameObject) Instantiate(enemyhealthbarprefab, Vector3.zero, Quaternion.identity);
					temp.SetActive(true);
					temp.GetComponent<RectTransform>().SetParent(enemyHealthBars.GetComponent<RectTransform>(),false);
					temp.GetComponent<EnemyHealthBar>().trackingEnemy = g;
				}
			}
			enemieswithhealthbars.Clear();
		}

		/*** adds inventory icons to UI when bool is true ***/
		if(updateInventoryUI) {
			int i;
			for (i = 0; i < playerRef.inventory.Count && i < 21; i++) {
				inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().sprite = GetSprite(playerRef.inventory[i].RarityVal);
				inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().color = Color.white;
				inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = playerRef.inventory[i].icon;
				inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
				inventoryItemContainer.transform.GetChild(i).GetComponent<EventTrigger>().enabled = true;
			}
			if(i < 21)
			{
				for(;i<21;i++)
				{
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().color = Color.black;
					inventoryItemContainer.transform.GetChild(i).GetComponent<EventTrigger>().enabled = false;
				}
			}
			updateInventoryUI = false;
		}

		/*** Shows WeaponXP bar ***/
		if(playerRef.GetWeapon() != null) {
			if(tempWeaponXPVal != playerRef.GetWeapon().GetBytes()) {
				curWeapon.text = playerRef.GetWeapon().GetName();
				GetComponent<Animator>().SetTrigger("ShowWeaponXP");
				tempWeaponXPVal = playerRef.GetWeapon().GetBytes();
			} else {
				GetComponent<Animator>().ResetTrigger("ShowWeaponXP");
			}
			weaponXPImg.rectTransform.localScale = new Vector3(playerRef.GetWeapon().GetVersionPercent(), 1, 1);
			weaponXPPercentage.text = (playerRef.GetWeapon().GetVersionPercent()*100).ToString("F2") + "%";
		}

		/*** draws and updates RMA and Integrity bar and bytes accumulated ***/
		RMABar.rectTransform.localScale = new Vector3(playerRef.GetRMAPercentage(), 1f);
		RMAPercentage.text = (playerRef.GetRMAPercentage()*100).ToString("F2") + "%";
		integrityBar.rectTransform.localScale = new Vector3(playerRef.GetIntegrityPercentage(),1f);
		integrityPercentage.text = (playerRef.GetIntegrityPercentage()*100).ToString("F2") + "%";
		byteText.text = "Bytes: " + Utility.ByteToString(playerRef.GetBytes());

		/*** Updates xp bar and draws background yellow xp scale ***/
		byteXP.rectTransform.localScale = new Vector3(playerRef.XPPercentage(), 1f, 1f);
		if(byteXPSmooth.rectTransform.localScale.x < playerRef.XPPercentage()) {
			byteXPSmooth.rectTransform.localScale = new Vector3(Mathf.MoveTowards(byteXPSmooth.rectTransform.localScale.x,playerRef.XPPercentage(),Time.deltaTime/10f), 1f, 1f);
		} else {
			byteXPSmooth.rectTransform.localScale = new Vector3(playerRef.XPPercentage(), 1f, 1f);
		}

		/*** Opens Console and sets animator control ***/
		if(Input.GetKeyDown(KeyCode.BackQuote)) {
			inConsole = !inConsole;
		}
		GetComponent<Animator>().SetBool("ShowingConsole", inConsole);

		/*** Handles logic when in console ***/
		if(inConsole) {
			/*** Disables minimap and allows interaction with UI ***/
			minimap.SetActive(false);
			consoleGUI.interactable = true;

			/*** Handles algorithm point allocation ***/
			if (Player.algorithmPoints > 0) {
				algorithmPointsText.text = "Algorithm Points Available: " + Player.algorithmPoints;
				defenseButton.interactable = true;
				strengthButton.interactable = true;
				efficiencyButton.interactable = true;
				encryptionButton.interactable = true;
				securityButton.interactable = true;
			} else {
				algorithmPointsText.text = Utility.ByteToString((int)(playerRef.GetXPBytes()/playerRef.XPPercentage()) - playerRef.GetXPBytes()) + " To Level Up"; 
				defenseButton.interactable = false;
				strengthButton.interactable = false;
				efficiencyButton.interactable = false;
				encryptionButton.interactable = false;
				securityButton.interactable = false;
			}
			/*** Updates algorithm points values ***/
			playerDefenseText.text = "Defense: " + Player.defense;
			playerStrengthText.text = "Strength: " + Player.strength;
			playerEfficiencyText.text = "Efficiency: " + Player.efficiency;
			playerEncryptionText.text = "Encryption: " + Player.encryption;
			playerSecurityText.text = "Security: " + Player.security;

			/*** Gets info string for current weapon and hack ***/
			weaponStatsInfo.text = playerRef.GetWeapon().InfoString();
			hackStatsInfo.text = playerRef.GetHack().InfoString();

			/*** Handles Blur effect ***/
			mainCam.camera.enabled = false;
			mainCamWithEffects.camera.enabled = true;
			mainCamWithEffects.GetComponent<Blur>().blur = Mathf.MoveTowards(mainCamWithEffects.GetComponent<Blur>().blur, 5, Time.deltaTime*5f);

			/*** Handles dragging logic ***/
			if(Input.GetMouseButtonDown(0)) {
				mousePressedLocation = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			if(Input.GetMouseButton(0)) {
				dragDelta = (new Vector2(Input.mousePosition.x, Input.mousePosition.y)) - mousePressedLocation;
			}

		} else {
			/*** Handles non-console interaction, disallows interaction with console ***/
			consoleGUI.interactable = false;
			minimap.SetActive(true);

			/*** Handles un-blurring ***/
			mainCamWithEffects.GetComponent<Blur>().blur = Mathf.MoveTowards(mainCamWithEffects.GetComponent<Blur>().blur, 0, Time.deltaTime*5f);
			if(mainCamWithEffects.GetComponent<Blur>().blur == 0) {
				mainCam.camera.enabled = true;
				mainCamWithEffects.camera.enabled = false;
			}

		}

		if(cinematicMode) {
			minimap.camera.enabled = false;
			uiCam.camera.enabled = false;
		} else {
			minimap.camera.enabled = true;
			uiCam.camera.enabled = true;
		}
	}




	/*** Button handlers ***/
	public void HandleDefenseClick() {
		Player.algorithmPoints--;
		Player.defense++;
	}

	public void HandleStrengthClick() {
		Player.algorithmPoints--;
		Player.strength++;
	}

	public void HandleEfficiencyClick() {
		Player.algorithmPoints--;
		Player.efficiency++;
	}

	public void HandleEncryptionClick() {
		Player.algorithmPoints--;
		Player.encryption++;
	}

	public void HandleSecurityClick() {
		Player.algorithmPoints--;
		Player.security++;
	}

	public void InventoryClicked() {
		GetComponent<Animator>().SetBool("ShowingInventory", true);
		GetComponent<Animator>().SetBool("ShowingStats", false);
	}

	public void StatsClicked() {
		GetComponent<Animator>().SetBool("ShowingInventory", false);
		GetComponent<Animator>().SetBool("ShowingStats", true);
	}

	public void HandleInventoryMouseOver(int index) {
		if(index != -1) {
			ColorBlock cb = inventoryItemContainer.transform.GetChild(index).GetComponent<Button>().colors;
			if (dragDelta == Vector2.zero) {
				GetComponent<Animator>().SetBool("MouseOver", true);
				mouseOverInfo.transform.parent.GetComponent<RectTransform>().anchoredPosition = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition
					+ new Vector2(-3.7f, 0.19f);
				mouseOverInfo.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerRef.inventory[index].name;
				mouseOverInfo.transform.GetChild(0).GetComponent<Text>().text = playerRef.inventory[index].InfoString();
				cb.highlightedColor = Color.white;
			} else {
				cb.highlightedColor = Color.blue;
			}
		} else {
			GetComponent<Animator>().SetBool("MouseOver", false);
			mouseOverInfo.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0f);
		}
	}

	public void HandleInventoryMouseDrag(int index) {
		if(dragStart == Vector2.zero) {
			dragStart = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition;
		}
		inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = dragStart + dragDelta/Screen.width*14f;
	}

	public void HandleInventoryMouseEndDrag(int index) {
		inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = dragStart;
		dragDelta = Vector2.zero;
		dragStart = Vector2.zero;
	}

}
 