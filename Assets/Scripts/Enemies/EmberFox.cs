using UnityEngine;
using System.Collections;

public class EmberFox : Enemy {

	// Use this for initialization
	void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
	}

	public void Attack() {
		GameObject temp = (GameObject)Instantiate(enemyAttack, transform.position + new Vector3(0,1f,0), transform.localRotation);
		temp.GetComponent<Attack>().SetDamage(2);
		temp.GetComponent<Attack>().SetCrit(0.1f);
	}
}
