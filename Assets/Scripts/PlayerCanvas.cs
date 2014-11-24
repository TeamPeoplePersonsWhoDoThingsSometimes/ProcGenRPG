using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCanvas : MonoBehaviour {

	private Animator playerAnim;

	private Player playerRef;

	private CanvasGroup inGameGUI;
	private Image bg, a1, a2, cursor;
	private Image byteXP;
	private Text byteText, playerName;

	private CanvasGroup consoleGUI;
	private Text consoleText;

	private RectTransform weaponXPGroup;
	private Text curWeapon;
	private Text weaponXPPercentage;
	private Image weaponXPImg;
	private float weaponXPTimeOffset;
	private int tempWeaponXPVal;

	private Button strengthButton, defenseButton, efficiencyButton, securityButton, encryptionButton;
	private Text playerStrengthText, playerDefenseText, playerEfficiencyText, playerSecurityText, playerEncryptionText, algorithmPointsText;

	private Text integrityPercentage, RMAPercentage;
	private Image integrityBar, RMABar;

	public static bool inConsole = false;

	private Camera minimap;
	private Vector3 playerCanvasOffset;

	// Use this for initialization
	void Start () {
		playerCanvasOffset = this.transform.position - Player.playerPos.position;

		minimap = GameObject.Find("MiniMapCam").camera;

		playerAnim = GameObject.Find("PlayerObj").GetComponent<Animator>();
		playerRef = GameObject.Find("PlayerObj").GetComponent<Player>();

		inGameGUI = transform.GetChild(0).GetComponent<CanvasGroup>();
		consoleGUI = transform.GetChild(1).GetComponent<CanvasGroup>();

		consoleText = GameObject.Find("ConsoleText").GetComponent<Text>();
	
		a1 = GameObject.Find("Attack1").GetComponent<Image>();
		a2 = GameObject.Find("Attack2").GetComponent<Image>();
		bg = GameObject.Find("AttackBG").GetComponent<Image>();
		cursor = GameObject.Find("AttackPointer").GetComponent<Image>();

		byteText = GameObject.Find("ByteText").GetComponent<Text>();
		playerName = GameObject.Find("PlayerName").GetComponent<Text>();
		playerName.text = playerRef.GetName();
		byteXP = GameObject.Find("ByteXP").GetComponent<Image>();

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

		integrityBar = GameObject.Find("IntegrityBar").GetComponent<Image>();
		integrityPercentage = GameObject.Find("IntegrityPercentText").GetComponent<Text>();
		RMABar = GameObject.Find("RMABar").GetComponent<Image>();
		RMAPercentage = GameObject.Find("RMAPercentText").GetComponent<Text>();
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

	void Update () {

		if(playerRef.GetWeapon() != null) {
			if(tempWeaponXPVal != playerRef.GetWeapon().GetBytes()) {
				curWeapon.text = playerRef.GetWeapon().GetName();
				weaponXPTimeOffset = 4f;
				tempWeaponXPVal = playerRef.GetWeapon().GetBytes();
			}

			if (weaponXPTimeOffset > 0) {
				weaponXPTimeOffset -= Time.deltaTime;
				weaponXPGroup.animation.Play("weaponXPAnim");
			} else {
				if(Time.time > 2) {
					weaponXPGroup.animation.Play("weaponXPAnimClose");
				}
			}
			weaponXPImg.rectTransform.localScale = new Vector3(playerRef.GetWeapon().GetVersionPercent(), 1, 1);
			weaponXPPercentage.text = (playerRef.GetWeapon().GetVersionPercent()*100).ToString("F2") + "%";
		}

		RMABar.rectTransform.localScale = new Vector3(playerRef.GetRMAPercentage(), 1f);
		RMAPercentage.text = (playerRef.GetRMAPercentage()*100).ToString("F2") + "%";
		integrityBar.rectTransform.localScale = new Vector3(playerRef.GetIntegrityPercentage(),1f);

		playerName.text = playerRef.GetName();

		byteText.text = "Bytes: " + Utility.ByteToString(playerRef.GetBytes());

		byteXP.rectTransform.localScale = new Vector3(playerRef.XPPercentage(), 1f, 1f);

		consoleText.text = playerRef.GetName();

		if(Input.GetKeyDown(KeyCode.BackQuote)) {
			inConsole = !inConsole;
		}

		if(inConsole) {
			inGameGUI.alpha = 0f;
			minimap.enabled = false;
			consoleGUI.alpha = 1f;
			consoleGUI.interactable = true;
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
			playerDefenseText.text = "Defense: " + Player.defense;
			playerStrengthText.text = "Strength: " + Player.strength;
			playerEfficiencyText.text = "Efficiency: " + Player.efficiency;
			playerEncryptionText.text = "Encryption: " + Player.encryption;
			playerSecurityText.text = "Security: " + Player.security;
		} else {
			consoleGUI.interactable = false;
			inGameGUI.alpha = 1f;
			minimap.enabled = true;
			consoleGUI.alpha = 0f;
		}
	}

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

}
 