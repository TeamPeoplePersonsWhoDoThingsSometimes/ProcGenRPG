using UnityEngine;
using System.Collections;

public class Overclock : Hack {

	protected override void PassiveActive ()
	{
		base.PassiveActive ();
		PlayerControl.speedBoost = 2;
	}

	protected override void Update ()
	{
		PlayerControl.speedBoost = 0;
		base.Update ();
	}
}
