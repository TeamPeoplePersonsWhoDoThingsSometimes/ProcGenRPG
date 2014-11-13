using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float speed;

	private Animator playerAnim;

	private bool comboTime = false;

	private bool attack1, attack2, attack3;

	private Transform camTransform;

	// Use this for initialization
	void Start () {
		playerAnim = this.GetComponent<Animator>();
		camTransform = transform.parent.GetChild(1).transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		playerAnim.SetBool("Attack1", false);
		playerAnim.SetBool("Attack2", false);
		playerAnim.SetBool("Attack3", false);

		attack1 = playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Base.Slash1") || playerAnim.GetCurrentAnimatorStateInfo(1).IsName("SlashWalking.SlashWalking");
		attack2 = playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Base.Slash2");
		attack3 = playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Base.Slash3");

		playerAnim.SetFloat("Speed",Input.GetAxis("Vertical"));
//		playerAnim.SetFloat("Direction",Input.GetAxis("Horizontal"));
		if(playerAnim.GetFloat("Speed") == 0) {
			playerAnim.transform.Rotate(new Vector3(0f, playerAnim.GetFloat("Direction"), 0f));
		}

//		if(playerAnim.GetFloat("Speed") > 0.5 && Input.GetKey(KeyCode.Q)) {
//			playerAnim.SetTrigger("Roll");
//		}

		if(Input.GetKeyDown(KeyCode.Space)) {
			playerAnim.SetTrigger("Jump");
		}

		if(playerAnim.GetFloat("Speed") > 0.5 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
			if(playerAnim.speed < 1.5f) {
				playerAnim.speed += Time.deltaTime;
			}
		} else {
			playerAnim.speed = 1;
		}
		GetComponent<CapsuleCollider>().height = 0.1787377f - 0.1f*playerAnim.GetFloat("ColliderHeight");
		GetComponent<CapsuleCollider>().center = new Vector3(0,0.09f - 0.09f*playerAnim.GetFloat("ColliderY"), 0f);

		if(playerAnim.GetFloat("Speed") > 0.5) {
			RaycastHit info = new RaycastHit();
			if(Physics.Raycast(new Ray(this.transform.position + new Vector3(0f, 1f, 0f), this.transform.forward), out info, playerAnim.GetFloat("Speed")*1.5f)) {
				playerAnim.SetFloat("Speed",Mathf.Min(Input.GetAxis("Vertical"),0));
			}
		}

		if(attack3) {
			GetComponent<Player>().StartAttack();
		} else {
			GetComponent<Player>().StopAttack();
		}


		if (Input.GetMouseButtonDown(0) && comboTime && !attack1 && attack2) {
			playerAnim.SetBool("Attack3", true);
			comboTime = false;
		}

		if (Input.GetMouseButtonDown(0) && comboTime && attack1 && !attack2) {
			playerAnim.SetBool("Attack2", true);
			comboTime = false;
		}

		if (Input.GetMouseButtonDown(0) && !attack2 && !attack3) {
			playerAnim.SetBool("Attack1", true);
			comboTime = false;
		}

		float mousePosX = Input.mousePosition.x;
		float mousePosY = Input.mousePosition.y;
		float screenX = Screen.width;
		float screenY = Screen.height;
		float angle;
		if (mousePosY < screenY/2) {
			angle = Mathf.Rad2Deg * Mathf.Atan(((mousePosX/screenX*2) - 1)/((mousePosY/screenY*2) - 1))+ 180;
		} else {
			angle = Mathf.Rad2Deg * Mathf.Atan(((mousePosX/screenX*2) - 1)/((mousePosY/screenY*2) - 1));
		}
		transform.eulerAngles = new Vector3(0f, angle + camTransform.eulerAngles.y, 0f);

//		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y + Input.GetAxis("Mouse X")*(2-playerAnim.GetFloat("Speed")), 0f)), 2000*Time.deltaTime);

//		playerAnim.SetFloat("Direction",(mousePosX - screenX/2f)/(screenX/2f));
	}

	void SetComboTime() {
		comboTime = !comboTime;
	}

}
