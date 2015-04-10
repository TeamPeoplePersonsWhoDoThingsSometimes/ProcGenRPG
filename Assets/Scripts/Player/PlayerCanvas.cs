using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class PlayerCanvas : MonoBehaviour {

	public static bool cinematicMode;

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
	private Text playerStrengthText, playerDefenseText, playerEfficiencyText, playerSecurityText, playerEncryptionText, algorithmPointsText, weaponStatsInfo, hackStatsInfo;

	private Text integrityPercentage, RMAPercentage;
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
		for(int i = 0; i < playerRef.quickAccessItems.Count; i++) {
			if(playerRef.quickAccessItems[i] != null) {
				quickAccessBar.transform.GetChild(i).GetComponent<Image>().sprite = GetSprite(playerRef.quickAccessItems[i].RarityVal);
				if(playerRef.GetWeapon() != null && playerRef.GetWeapon().Equals(playerRef.quickAccessItems[i])) {
					activeWeaponIcon.SetParent(quickAccessBar.GetChild(i), false);
				}
				if(playerRef.GetHack() != null && playerRef.GetHack().Equals(playerRef.quickAccessItems[i])) {
					activeHackIcon.SetParent(quickAccessBar.GetChild(i), false);
				}

				if(playerRef.quickAccessItems[i].GetComponent<Hack>() != null) {
					quickAccessBar.transform.GetChild(i).FindChild("HackReload").GetComponent<RectTransform>().localScale = new Vector3(1, playerRef.quickAccessItems[i].GetComponent<Hack>().GetPercentReload(),1f);
				} else {
					quickAccessBar.transform.GetChild(i).FindChild("HackReload").GetComponent<RectTransform>().localScale = new Vector3(1f,0f,1f);
				}
				quickAccessBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = playerRef.quickAccessItems[i].icon;
			} else {
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
					inventoryItemContainer.transform.GetChild(i).GetComponent<Image>().color = Color.black;
					inventoryItemContainer.transform.GetChild(i).GetComponent<EventTrigger>().enabled = false;
				}
			}
			updateInventoryUI = false;
		}

		/*** Quest UI ***/
		if(updateQuestUI && MasterDriver.Instance != null) {
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
		RMAPercentage.text = (playerRef.GetRMAPercentage()*100).ToString("F2") + "%";
		integrityBar.rectTransform.localScale = new Vector3(playerRef.GetIntegrityPercentage(),1f);
		integrityPercentage.text = (playerRef.GetIntegrityPercentage()*100).ToString("F2") + "%";
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
		if(Input.GetKeyDown(KeyCode.BackQuote)) {
			FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI01",Player.playerPos.position);
			inConsole = !inConsole;
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
			}
			if(playerRef.GetHack() != null) {
				hackStatsInfo.text = playerRef.GetHack().InfoString();
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
	}

	public void HandleInventoryMouseOver(int index) {
		if(index != -1 && dragDelta == Vector2.zero) {
			GetComponent<Animator>().SetBool("MouseOver", true);
			mouseOverInfo.transform.parent.GetComponent<RectTransform>().anchoredPosition = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition
				+ new Vector2(-3.7f, 0.19f);
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
		if(dragStart == Vector2.zero && index != 0) {
			dragStart = inventoryItemContainer.transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition;
		} else if(index == 0) {
			dragStart = Vector2.zero;
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
		FMOD_StudioSystem.instance.PlayOneShot("event:/player/weaponEquip01",Player.playerPos.position);
	}

	public void PlayMouseOverSound() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI03",Player.playerPos.position);
	}

	public void PlayMouseClickSound() {
		FMOD_StudioSystem.instance.PlayOneShot("event:/UISounds/UI02",Player.playerPos.position);
	}

}
 