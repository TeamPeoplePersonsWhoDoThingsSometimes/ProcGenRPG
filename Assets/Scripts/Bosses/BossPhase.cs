using UnityEngine;
using System.Collections;

public enum PhaseType {
	Attack,
	Spawn,
	Move,
	Other,
}

public class BossPhase : MonoBehaviour {
	public PhaseType phaseType;

	public string phaseName;

	public bool timeInterval;
	public float timeIntervalValue;

	public bool healthGreaterThan;
	public float healthGreaterThanValue;
	public bool healthLessThan;
	public float healthLessThanValue;
	
	public GameObject phaseObject;

	private float timer;

	void Start() {
		timer = timeIntervalValue;
	}

	void Update() {
		timer -= Time.deltaTime;
	}

	public bool ConditionMet() {
		bool forreturn = true;
		
		Boss boss = GetComponent<Boss>();

		if(healthGreaterThan && boss.GetHealthPercentage() <= healthGreaterThanValue) {
			forreturn = false;
		}
		if(healthLessThan && boss.GetHealthPercentage() >= healthLessThanValue) {
			forreturn = false;
		}
			
		if(timeInterval && timer > 0) {
			forreturn = false;
		} else if (timer <= 0) {
			timer = timeIntervalValue;
		}

		return forreturn;
	}

	public void HandlePhase() {
		Boss boss = GetComponent<Boss>();
		switch(phaseType) {
			case PhaseType.Attack:
				boss.PhaseAttack(phaseName, phaseObject);
				break;
			case PhaseType.Move:
				boss.PhaseMove(phaseName);
				break;
			case PhaseType.Spawn:
				boss.PhaseSpawn(phaseName, phaseObject);
				break;
			case PhaseType.Other:
				boss.PhaseOther(phaseName, phaseObject);
				break;
		}
	}
}
