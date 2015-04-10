using UnityEngine;
using System.Collections;

public class OrbitalBomb : Hack {

    protected override void OneShotActivated()
    {
        base.OneShotActivated();
        GameObject tempAttack = (GameObject) Instantiate(attack, Player.playerPos.position + Player.playerPos.forward.normalized * 2 + new Vector3(0f, 2f), Quaternion.identity);
        tempAttack.GetComponent<Attack>().SetDamage(damage);
        tempAttack.GetComponent<Attack>().SetCrit(critChance);
    }

}
