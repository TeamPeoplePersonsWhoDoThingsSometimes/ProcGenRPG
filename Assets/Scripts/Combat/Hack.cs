using UnityEngine;
using System.Collections;

public class Hack : Item {

	public float RMACost;
	public bool passive;
	public bool oneShot;
	public float damage;
	public float critChance;
	public float firingRate;
	public GameObject attack;
	private float tempFiringRate;

	private bool passiveActive;
	private bool isCalled;

	private Player playerref;

	private float tempCounter;
	// Use this for initialization
	void Start () {
		if(passive && oneShot) {
			Debug.LogError("Hack cannot be both passive and oneshot\nhack will not operate properly");
		}
		tempFiringRate = firingRate;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		tempFiringRate -= Time.deltaTime;

		if (isCalled) {
		
			if (tempFiringRate <= 0 && oneShot) {
				if (playerref.ExpendRMA(RMACost + (Player.strength/2f))) {
					OneShotActivated();
				}
			} else if(passive && tempFiringRate <= 0) {
				passiveActive = !passiveActive;
				tempFiringRate = 1;
			}
			isCalled = false;
		}

		if (passiveActive && passive) {
			PassiveActive();
		}
	}

	public float GetPercentReload() {
		if(tempFiringRate <= 0) {
			return 0;
		} else {
			return (firingRate - tempFiringRate)/firingRate;
		}
	}

	/**
	 * What should happen when the hack is active
	 */
	protected virtual void PassiveActive() {
		playerref.ExpendRMA(RMACost*Time.deltaTime);
	}

	/**
	 * What should happen when the one shot is activated
	 */
	protected virtual void OneShotActivated() {
		tempFiringRate = firingRate;
	}

	public void Call(Player pRef) {
		playerref = pRef;
		isCalled = true;
	}

	public override string InfoString() {
		string forreturn =
			"RMA\n" + RMACost + " (+" + (Player.strength/2f).ToString("F2") + ")" +
			"\n\nDMG\n" + damage + " (+" + (Player.strength*2) + ")" +
			"\n\nINF\n" + (passive ? "Passive: " : "Active: ") + description.Replace("DMG",(damage+(Player.strength*2)).ToString()).Replace("rma",RMACost.ToString());

		return forreturn;
	}

}
