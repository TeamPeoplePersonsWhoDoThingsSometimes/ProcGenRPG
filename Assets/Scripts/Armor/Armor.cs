using UnityEngine;
using System.Collections;

public enum ArmorType {
	Head,
	Chest,
	Arm,
	Leg
}

public class Armor : Item {

	public ArmorType armorType;
	public int armorRating;

	// Use this for initialization
	void Start () {
//		if(armorType == ArmorType.Leg && !(transform.childCount == 2 && transform.GetChild(0).childCount == 3 && transform.GetChild(1).childCount == 3))
//		{
//			Debug.LogError("Incorrect leg armor setup!!");
//		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
