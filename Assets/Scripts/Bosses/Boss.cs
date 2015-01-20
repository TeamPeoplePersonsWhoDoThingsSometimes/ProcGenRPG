using UnityEngine;
using System.Collections;

public class Boss : Enemy {
	
	private BossPhase[] phases;

	protected void Start () {
		base.Start();

		phases = GetComponents<BossPhase>();
	}

	protected void Update () {
		base.Update();

		for (int i = 0; i < phases.Length; i++) {
			if (phases[i].ConditionMet()) {
				phases[i].HandlePhase();
			}
		}
	}

	protected override void DoIdle ()
	{

	}

	protected override void HandleDeath ()
	{
		base.HandleDeath();
	}

	protected override void HandleDetectedPlayer ()
	{

	}

	protected override void HandleEffect ()
	{
		base.HandleEffect();
	}

	protected override void HandleKnockback ()
	{

	}

	protected override void HandlePlayerDetection ()
	{

	}

	public virtual void PhaseAttack(string phaseName, GameObject phaseObject) {

	}

	public virtual void PhaseMove(string phaseName) {

	}

	public virtual void PhaseSpawn(string phaseName, GameObject phaseObject) {

	}

}
