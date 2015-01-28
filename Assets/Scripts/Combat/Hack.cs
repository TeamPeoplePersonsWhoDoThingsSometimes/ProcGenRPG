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

	// Use this for initialization
	void Start () {
		if(passive && oneShot) {
			Debug.LogError("Hack cannot be both passive and oneshot\nhack will not operate properly");
		}
		tempFiringRate = firingRate;
	}
	
	// Update is called once per frame
	void Update () {
		tempFiringRate -= Time.deltaTime;

		if (isCalled) {
			passiveActive = !passiveActive;

			if (passiveActive && passive) {
				playerref.ExpendRMA(RMACost);
				PassiveActive();
			}

			if (tempFiringRate <= 0 && oneShot) {
				if (playerref.ExpendRMA(RMACost)) {
					OneShotActivated();
				}
			}
			isCalled = false;
		}
	}

	/**
	 * What should happen when the hack is active
	 */
	protected virtual void PassiveActive() {

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
		string forreturn = "Type: " + (passive ? "Passive" : "Active") +
			"\n\nRarity: " + this.RarityVal +
				"\n\nBase Damage: " + damage.ToString("F2") +
				"\n\nKnockback: " + attack.GetComponent<Attack>().knockback.ToString("F2") +
				"\n\nDescription: " + description;

		return forreturn;
	}

}
