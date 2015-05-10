using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class PlayerCanvas : MonoBehaviour {

	public static bool cinematicMode, mapMode;

	public Sprite common, uncommon, rare, anomaly;

	public static List<GameObject> enemieswithhealthbars;
	public static bool updateInventoryUI, updateQuestUI = true;

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
	private Text playerStrengthText, playerDefenseText, playerEfficiencyText, playerSecurityText, playerEncryptionText, algorithmPointsText, weaponStatsInfo, hackStatsInfo, weaponName, hackName;

	private Image weaponRarityBar, hackRarityBar;
	private Text weaponRarityText, hackRarityText;

	private Text integrityPercentage, RMAPercentage, integrityValue, RMAValue, blockingValue;
	private Image integrityBar, RMABar;

	public static bool inConsole = false;

	private GameObject minimap, mainCam, worldCam, uiCam;
	private Vector3 playerCanvasOffset;

	private RectTransform quickAccessBar, activeWeaponIcon, activeHackIcon;

	private GameObject VRCursor;

	private GameObject enemyHealthBars;

	private GameObject inventoryItemContainer, mouseOverInfo;

	private Text questInfo;
	private GameObject questButton, questButtonHolder;
	
	private Vector2 dragDelta = Vector2.zero;
	private Vector2 mousePressedLocation;
	private Vector2 dragStart;
	private int mouseOverInventoryItem;
	private int mouseDragInventoryItem;
	private bool readyToDrop;

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
		worldCam = GameObject.Find("WorldMapCam");

		playerAnim = GameObject.Find("PlayerObj").GetComponent<Animator>();
		playerRef = GameObject.Find("PlayerObj").GetComponent<Player>();

		inGameGUI = transform.GetChild(1).GetComponent<CanvasGroup>();
		consoleGUI = transform.GetChild(2).GetComponent<CanvasGroup>();
	
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
		if(playerRef.GetWeapon() != null) {
			tempWeaponXPVal = playerRef.GetWeapon().GetBytes();
			curWeapon.text = playerRef.GetWeapon().GetName();
		}
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
		weaponName = GameObject.Find("WeaponStats").GetComponent<Text>();
		hackName = GameObject.Find("HackStats").GetComponent<Text>();
		hackStatsInfo = GameObject.Find("HackStatInfo").GetComponent<Text>();
		
		hackRarityBar = GameObject.Find("HackRarityBar").GetComponent<Image>();
		weaponRarityBar = GameObject.Find("WeaponRarityBar").GetComponent<Image>();
		weaponRarityText = GameObject.Find("WeaponRarityText").GetComponent<Text>();
		hackRarityText = GameObject.Find("HackRarityText").GetComponent<Text>();

		integrityBar = GameObject.Find("IntegrityBar").GetComponent<Image>();
		integrityPercentage = GameObject.Find("IntegrityPercentText").GetComponent<Text>();
		integrityValue = GameObject.Find("IntegrityValueText").GetComponent<Text>();
		RMABar = GameObject.Find("RMABar").GetComponent<Image>();

		RMAPercentage = GameObject.Find("RMAPercentText").GetComponent<Text>();
		RMAValue = GameObject.Find("RMAValueText").GetComponent<Text>();
		blockingValue = GameObject.Find("BlockingValueText").GetComponent<Text>();

		quickAccessBar = GameObject.Find("QuickAccessBar").GetComponent<RectTransform>();
		activeWeaponIcon = GameObject.Find("ActiveWeaponIcon").GetComponent<RectTransform>();
		activeHackIcon = GameObject.Find("ActiveHackIcon").GetComponent<RectTransform>();

		enemyHealthBars = GameObject.Find("EnemyHealthBars");

		VRCursor = GameObject.Find("VRCursor");

		inventoryItemContainer = GameObject.Find("InventoryItemContainer");
		mouseOverInfo = GameObject.Find("MouseOverInfo");

		questInfo = GameObject.Find("QuestInfo").GetComponent<Text>();
		questButton = GameObject.Find("QuestButtonPrefab");
		questButton.SetActive(false);
		questButtonHolder = GameObject.Find("QuestButtonHolder");

		if(QualitySettings.GetQualityLevel() <= (int)QualityLevel.Good) {
			Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = false;
			Camera.main.GetComponent<DepthOfField>().enabled = false;
		}
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
		/* Weird bug fix?! */
		if(Vector2.Distance(strengthButton.GetComponent<RectTransform>().anchoredPosition, new Vector2(-1.2f, 30.5f)) > 1) {
			strengthButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.2f, 30.5f);
			defenseButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.2f, 16.4f);
			efficiencyButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.2f, 2f);
			securityButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.2f, -12.3f);
			encryptionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.2f, -26.4f);
		}

		/*** Updates QuickAccessItems ***/
		List<Item> tempPlayerQuickAccess = playerRef.quickAccessItems;
		for(int i = 0; i < 10; i++) {
			if(i < tempPlayerQuickAccess.Count && tempPlayerQuickAccess[i] != null) {
				quickAccessBar.transform.GetChild(i).GetComponent<Image>().sprite = GetSprite(tempPlayerQuickAccess[i].RarityVal);
				if(tempPlayerQuickAccess[i].RarityVal.Equals(Rarity.Anomaly)) {
					quickAccessBar.transform.GetChild(i).GetComponent<Image>().color = new Color(Random.value*0.5f+0.5f,Random.value*0.5f+0.5f,Random.value*0.5f+0.5f);
				}
				if(playerRef.GetWeapon() != null && playerRef.GetWeapon().Equals(tempPlayerQuickAccess[i])) {
					activeWeaponIcon.SetParent(quickAccessBar.GetChild(i), false);
				}
				if(playerRef.GetHack() != null && playerRef.GetHack().Equals(tempPlayerQuickAccess[i])) {
					activeHackIcon.SetParent(quickAccessBar.GetChild(i), false);
				}

				if(tempPlayerQuickAccess[i].GetComponent<Hack>() != null) {
					quickAccessBar.transform.GetChild(i).FindChild("HackReload").GetComponent<RectTransform>().localScale = new Vector3(1, tempPlayerQuickAccess[i].GetComponent<Hack>().GetPercentReload(),1f);
				} else {
					quickAccessBar.transform.GetChild(i).FindChild("HackReload").GetComponent<RectTransform>().localScale = new Vector3(1f,0f,1f);
				}
				quickAccessBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = tempPlayerQuickAccess[i].icon;
			} else {
				quickAccessBar.transform.GetChild(i).GetComponent<Image>().sprite = new Sprite();
				quickAccessBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
				quickAccessBar.transform.GetChild(i).FindChild("HackReload").GetComponent<RectTransform>().localScale = new Vector3(1f,0f,1f);
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
				if(playerRef.inventory[i] != null) {
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().sprite = GetSprite(playerRef.inventory[i].RarityVal);
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().color = Color.white;
					inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = playerRef.inventory[i].icon;
					inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
					inventoryItemContainer.transform.GetChild(i).GetComponent<EventTrigger>().enabled = true;
				}
			}
			if(i < 21)
			{
				for(;i<21;i++)
				{
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().sprite = null;
					inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
					inventoryItemContainer.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.clear;
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().color = Color.black;
					inventoryItemContainer.transform.GetChild(i).GetComponent<EventTrigger>().enabled = false;
				}
			}
			updateInventoryUI = false;
		}

		/*** Quest UI ***/
		if(updateQuestUI && MasterDriver.Instance != null && MasterDriver.bossLevel != true) {
			int tempCounter = 0;
			int i = 0;
			List<Quest> activeQuests = MasterDriver.Instance.MasterQuestListener().getActiveQuests();
			if(activeQuests.Count > 0) {
				for (; i < questButtonHolder.transform.childCount && tempCounter < activeQuests.Count; i++) {
					if (questButtonHolder.transform.GetChild(i).gameObject.activeSelf) {
						questButtonHolder.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = activeQuests[tempCounter].getName();
						tempCounter++;
					}
				}
			
				while (tempCounter < activeQuests.Count) {
					GameObject temp = (GameObject)GameObject.Instantiate(questButton);
					temp.GetComponent<RectTransform>().SetParent(questButtonHolder.transform,false);
					temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					temp.GetComponent<RectTransform>().localScale = Vector3.one;
					temp.SetActive(true);
					temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -25 - 58*(i-1));
					questButtonHolder.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = activeQuests[tempCounter].getName() + ": " + (int)(activeQuests[tempCounter].getPercentComplete()*100) + "%";
					tempCounter++;
					i++;
				}

				while (i < questButtonHolder.transform.childCount) {
					if(!questButtonHolder.transform.GetChild(questButtonHolder.transform.childCount-1).gameObject.activeSelf) {
						break;
					}
					Destroy(questButtonHolder.transform.GetChild(questButtonHolder.transform.childCount-1).gameObject);
				}

				for(int j = 1; j < questButtonHolder.transform.childCount; j++) {
					if(j == 1) {
						Quest temp = MasterDriver.Instance.MasterQuestListener().getActiveQuests()[j - 1];
						questButtonHolder.transform.GetChild(j).GetComponent<Button>().interactable = false;
						string stepDesc = temp.getCurrentStepDescription();
						questInfo.text = temp.getCurrentStepName() + " (" + (int)(temp.getCurStepPercentage()*100) + "%):\n\n" + stepDesc;
					} else {
						questButtonHolder.transform.GetChild(j).GetComponent<Button>().interactable = true;
					}
				}
			}

			updateQuestUI = false;
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
//		RMAPercentage.text = (playerRef.GetRMAPercentage()*100).ToString("F2") + "%";
		RMAPercentage.text = playerRef.GetRMARegen();
		RMAValue.text = playerRef.GetRMAValueText();
		blockingValue.text = playerRef.GetBlocking();
		integrityValue.text = playerRef.GetIntegrityValueText();
		integrityBar.rectTransform.localScale = new Vector3(playerRef.GetIntegrityPercentage(),1f);
//		integrityPercentage.text = (playerRef.GetIntegrityPercentage()*100).ToString("F2") + "%";
		integrityPercentage.text = playerRef.GetIntegrityRegen();
		byteText.text = "Bytes: " + Utility.ByteToString(playerRef.GetBytes());

		/*** Updates xp bar and draws background yellow xp scale ***/
		byteXP.rectTransform.localScale = new Vector3(playerRef.XPPercentage(), 1f, 1f);
		playerName.text = playerRef.GetName();
		if(byteXPSmooth.rectTransform.localScale.x < playerRef.XPPercentage()) {
			byteXPSmooth.rectTransform.localScale = new Vector3(Mathf.MoveTowards(byteXPSmooth.rectTransform.localScale.x,playerRef.XPPercentage(),Time.deltaTime/10f), 1f, 1f);
		} else {
			byteXPSmooth.rectTransform.localScale = new Vector3(playerRef.XPPercentage(), 1f, 1f);
		}

		/*** Opens Console and sets animator control ***/
		if(!PlayerControl.immobile && (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.P))) {
			FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI01",Player.playerPos.position,PlayerPrefs.GetFloat("MasterVolume"));
			inConsole = !inConsole;
			if(CityHelp.helpMode == 1) {
				CityHelp.helpMode = 2;
			}
			if(CityHelp.helpMode == 7) {
				CityHelp.helpMode = 8;
			}

		}
		GetComponent<Animator>().SetBool("ShowingConsole", inConsole);

		/*** Handles logic when in console ***/
		if(inConsole) {
			/*** Disables minimap and allows interaction with UI ***/
			minimap.SetActive(false);
			consoleGUI.interactable = true;
			worldCam.GetComponent<Camera>().enabled = false;

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
			if(playerRef.GetWeapon() != null) {
				weaponStatsInfo.text = playerRef.GetWeapon().InfoString();
				weaponName.text = playerRef.GetWeapon().GetName();
				switch(playerRef.GetWeapon().RarityVal) {
					case Rarity.Common:
						weaponRarityBar.color = new Color(0f,118f/255f,1f,100f/255f);
						break;
					case Rarity.Uncommon:
						weaponRarityBar.color = new Color(1f,1f,0f,100f/255f);
						break;
					case Rarity.Rare:
						weaponRarityBar.color = new Color(1f,0f,1f,100f/255f);
						break;
					case Rarity.Anomaly:
						weaponRarityBar.color = new Color(Random.value*0.5f+0.5f,Random.value*0.5f+0.5f,Random.value*0.5f+0.5f);
						break;
				}
				weaponRarityText.text = playerRef.GetWeapon().RarityVal.ToString();
			} else {
				weaponStatsInfo.text = "";
				weaponName.text = "";
				weaponRarityBar.color = new Color(1f,1f,1f,100f/255f);
				weaponRarityText.text = "";
			}
			if(playerRef.GetHack() != null) {
				hackStatsInfo.text = playerRef.GetHack().InfoString();
				hackName.text = playerRef.GetHack().name;
				switch(playerRef.GetHack().RarityVal) {
					case Rarity.Common:
						hackRarityBar.color = new Color(0f,118f/255f,1f,100f/255f);
						break;
					case Rarity.Uncommon:
						hackRarityBar.color = new Color(1f,1f,0f,100f/255f);
						break;
					case Rarity.Rare:
						hackRarityBar.color = new Color(1f,0f,1f,100f/255f);
						break;
					case Rarity.Anomaly:
						hackRarityBar.color = new Color(Random.value*0.5f+0.5f,Random.value*0.5f+0.5f,Random.value*0.5f+0.5f);
						break;
				}
				hackRarityText.text = playerRef.GetHack().RarityVal.ToString();
			} else {
				hackStatsInfo.text = "";
				hackName.text = "";
				hackRarityBar.color = new Color(1f,1f,1f,100f/255f);
				hackRarityText.text = "";
			}

			/*** Handles Blur effect ***/
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				if(mainCam.GetComponent<VignetteAndChromaticAberration>().intensity < 3f) {
					mainCam.GetComponent<VignetteAndChromaticAberration>().intensity += Time.deltaTime*2f;
					mainCam.GetComponent<DepthOfField>().useFocalTransform = false;
				}
			}

			/*** Handles dragging logic ***/
			if(Input.GetMouseButtonDown(0)) {
				mousePressedLocation = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			if(Input.GetMouseButton(0)) {
				dragDelta = (new Vector2(Input.mousePosition.x, Input.mousePosition.y)) - mousePressedLocation;
			}

			if(Input.GetMouseButton(1) && itemToBeDeleted != -1) {
				rightMouseButtonHeld += Time.deltaTime;
				Color temp = inventoryItemContainer.transform.GetChild(itemToBeDeleted).GetComponent<Image>().color;
				inventoryItemContainer.transform.GetChild(itemToBeDeleted).GetComponent<Image>().color = new Color(temp.r+0.01f,temp.g-0.01f, temp.b-0.01f);
				if(rightMouseButtonHeld > 2) {
					playerRef.DeleteItem(itemToBeDeleted);
					rightMouseButtonHeld = 0f;
					itemToBeDeleted = 0;
					HandleInventoryMouseOver(-1);
				}
			} else if(!Input.GetMouseButton(1) && itemToBeDeleted != -1) {
//				inventoryItemContainer.transform.GetChild(itemToBeDeleted).GetComponent<Image>().color = Color.white;
				itemToBeDeleted = 0;
				rightMouseButtonHeld = 0;
			}

		} else {

			/*** Handles non-console interaction, disallows interaction with console ***/
			consoleGUI.interactable = false;
			minimap.SetActive(true);
			worldCam.GetComponent<Camera>().enabled = true;

			/*** Handles un-blurring ***/
			if(QualitySettings.GetQualityLevel() > (int)QualityLevel.Good) {
				if(mainCam.GetComponent<VignetteAndChromaticAberration>().intensity > 0) {
					mainCam.GetComponent<VignetteAndChromaticAberration>().intensity -= Time.deltaTime*2f;
					mainCam.GetComponent<DepthOfField>().useFocalTransform = true;
				}
			}

		}

		if(cinematicMode) {
			minimap.GetComponent<Camera>().enabled = false;
			uiCam.GetComponent<Camera>().enabled = false;
		} else {
			minimap.GetComponent<Camera>().enabled = true;
			uiCam.GetComponent<Camera>().enabled = true;
		}

		if(mouseOverInventoryItem != -1 && !dragDelta.Equals(Vector2.zero) && mouseOverInventoryItem != mouseDragInventoryItem) {
			inventoryItemContainer.transform.GetChild(mouseOverInventoryItem).GetComponent<Image>().color = Color.green;
			readyToDrop = true;
		} else {
			readyToDrop = false;
		}

		Cursor.visible = inConsole || cinematicMode || mapMode;
	}


	public void HandleQuestSelect(GameObject obj) {
		questButtonHolder.transform.GetChild(obj.transform.GetSiblingIndex()).GetComponent<Button>().interactable = false;
		for(int j = 0; j < questButtonHolder.transform.childCount; j++) {
			if(j != obj.transform.GetSiblingIndex()) {
				questButtonHolder.transform.GetChild(j).GetComponent<Button>().interactable = true;
			}
		}
//		Debug.Log("HEREEEE" + (obj.transform.GetSiblingIndex()-1) + " " + MasterDriver.Instance.MasterQuestListener().getActiveQuests().Count);
		Quest temp = MasterDriver.Instance.MasterQuestListener().getActiveQuests()[obj.transform.GetSiblingIndex()-1];
		string stepDesc = temp.getCurrentStepDescription();
		questInfo.text = temp.getCurrentStepName() + " (" + (int)(temp.getCurStepPercentage()*100) + "%):\n\n" + stepDesc;
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
		GetComponent<Animator>().SetBool("ShowingQuests", false);
		if(CityHelp.helpMode == 6) {
			CityHelp.helpMode = 7;
		}
	}

	public void StatsClicked() {
		GetComponent<Animator>().SetBool("ShowingInventory", false);
		GetComponent<Animator>().SetBool("ShowingStats", true);
		GetComponent<Animator>().SetBool("ShowingQuests", false);
	}

	public void QuestsClicked() {
		GetComponent<Animator>().SetBool("ShowingInventory", false);
		GetComponent<Animator>().SetBool("ShowingStats", false);
		GetComponent<Animator>().SetBool("ShowingQuests", true);
		if(CityHelp.helpMode == 2) {
			CityHelp.helpMode = 3;
		}
	}

	public void HandleInventoryMouseOver(int index) {
		if(index != -1 && dragDelta == Vector2.zero) {
			GetComponent<Animator>().SetBool("MouseOver", true);
			mouseOverInfo.transform.parent.GetComponent<RectTransform>().anchoredPosition = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition
				+ new Vector2(1.1f, -1.35f);
			mouseOverInfo.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerRef.inventory[index].name;
			mouseOverInfo.transform.GetChild(0).GetComponent<Text>().text = playerRef.inventory[index].InfoString();
			inventoryItemContainer.transform.GetChild(index).GetComponent<Image>().color = Color.white;
		} else {
			GetComponent<Animator>().SetBool("MouseOver", false);
			if(mouseOverInventoryItem != -1) {
				inventoryItemContainer.transform.GetChild(mouseOverInventoryItem).GetComponent<Image>().color = Color.white;
			}
			mouseOverInfo.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0f);
		}
		mouseOverInventoryItem = index;
	}

	public void HandleInventoryMouseDrag(int index) {
		mouseDragInventoryItem = index;
		if(dragStart == Vector2.zero) {
			dragStart = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition;
		}
		inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = dragStart + dragDelta/Screen.width*14f;
		inventoryItemContainer.transform.GetChild(index).GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void HandleInventoryMouseEndDrag(int index) {
		if(readyToDrop) {
//			inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = inventoryItemContainer.transform.GetChild(mouseOverInventoryItem).GetComponent<RectTransform>().anchoredPosition;
//			inventoryItemContainer.transform.GetChild(mouseOverInventoryItem).GetComponent<RectTransform>().anchoredPosition = dragStart;
			Item tempItem = playerRef.inventory[index];
			playerRef.inventory[index] = playerRef.inventory[mouseOverInventoryItem];
			playerRef.inventory[mouseOverInventoryItem] = tempItem;
			updateInventoryUI = true;
//			inventoryItemContainer.transform.GetChild(index).SetSiblingIndex(mouseOverInventoryItem);
//			inventoryItemContainer.transform.GetChild(mouseOverInventoryItem-1).SetSiblingIndex(index);
		}

		inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = dragStart;
		inventoryItemContainer.transform.GetChild(index).GetComponent<CanvasGroup>().blocksRaycasts = true;
		dragDelta = Vector2.zero;
		dragStart = Vector2.zero;
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/weaponEquip01",Player.playerPos.position,PlayerPrefs.GetFloat("MasterVolume")/2f);
	}

	private float rightMouseButtonHeld = 0f;
	private int itemToBeDeleted = -1;

	public void ItemPressed(int index) {
		itemToBeDeleted = index;
	}

	public void PlayMouseOverSound() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI03",Player.playerPos.position,PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void PlayMouseClickSound() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI02",Player.playerPos.position,PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void HelpClicked() {
		PlayerControl.immobile = true;
		transform.GetChild(3).gameObject.SetActive(true);
		transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
		transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
		transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
		transform.GetChild(3).GetChild(3).gameObject.SetActive(false);
		transform.GetChild(3).GetChild(4).gameObject.SetActive(true);

		inGameGUI.alpha = 0;
		consoleGUI.alpha = 0;
//		transform.GetChild(1).gameObject.SetActive(false);
//		transform.GetChild(2).gameObject.SetActive(false);
	}

	public void HelpConfirmed() {
		if(transform.GetChild(3).GetChild(0).gameObject.activeSelf) {
			transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(3).gameObject.SetActive(false);
		} else if(transform.GetChild(3).GetChild(1).gameObject.activeSelf) {
			transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(3).GetChild(3).gameObject.SetActive(false);
		} else if(transform.GetChild(3).GetChild(2).gameObject.activeSelf) {
			transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).GetChild(3).gameObject.SetActive(true);
		} else if(transform.GetChild(3).GetChild(3).gameObject.activeSelf) {
			PlayerControl.immobile = false;
			transform.GetChild(3).gameObject.SetActive(false);

			inGameGUI.alpha = 1;
			consoleGUI.alpha = 1;
//			transform.GetChild(1).gameObject.SetActive(true);
//			transform.GetChild(2).gameObject.SetActive(true);
		}
	}

}
 