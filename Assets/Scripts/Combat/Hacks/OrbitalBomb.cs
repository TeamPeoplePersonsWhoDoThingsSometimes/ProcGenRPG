using UnityEngine;
using System.Collections;

public class OrbitalBomb : Hack {

    protected override void OneShotActivated()
    {
		Debug.Log("HERE");
        base.OneShotActivated();
		Debug.Log("did base");
        GameObject tempAttack = (GameObject) Instantiate(attack, Player.playerPos.position + Player.playerPos.forward.normalized * 2 + new Vector3(0f, 2f), Quaternion.identity);
		tempAttack.GetComponent<Attack>().SetDamage(damage + (Player.strength*2));
        tempAttack.GetComponent<Attack>().SetCrit(critChance);
		Debug.Log("Tried spawning");
    }

}
