using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	private static GameObject hitInfo, byteObject, commonItemDrop, uncommonItemDrop, rareItemDrop, anomalyItemDrop;

	public static string ByteToString(int bytes) {
		if(bytes > (1000*1000*500)) {
			return ((float)bytes/1000000000f).ToString("F2") + "gb";
		}

		if(bytes > (1000*500)) {
			return ((float)bytes/1000000f).ToString("F2") + "mb";
		}

		return ((float)bytes/1000f).ToString("F2") + "kb";
	}

	public static int VersionToInt(string version) {
//		return ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])));
		return int.Parse(version.Split('.')[0] + version.Split('.')[1] + version.Split('.')[2]);
	}

	public static int ComparableVersionInt(string version) {
		return int.Parse(version.Split('.')[0])*100 + int.Parse(version.Split('.')[1]);
	}

	public static string ModVersionBy(string version, int value) {
		return version.Split('.')[0] + "." + (int.Parse(version.Split('.')[1]) + value) + "." + version.Split('.')[2];
	}

	public static GameObject GetCommonItemDrop() {
		if(commonItemDrop == null) {
			commonItemDrop = (GameObject) Resources.Load("ItemDrops/CommonItemDrop");
		}
		return commonItemDrop;
	}

	public static GameObject GetByteObject() {
		if(byteObject == null) {
			byteObject = (GameObject) Resources.Load("Info/Byte");
		}
		return byteObject;
	}

	public static GameObject GetUncommonItemDrop() {
		if(uncommonItemDrop == null) {
			uncommonItemDrop = (GameObject) Resources.Load("ItemDrops/UncommonItemDrop");
		}
		return uncommonItemDrop;
	}

	public static GameObject GetRareItemDrop() {
		if(rareItemDrop == null) {
			rareItemDrop = (GameObject) Resources.Load("ItemDrops/RareItemDrop");
		}
		return rareItemDrop;
	}

	public static GameObject GetAnomalyItemDrop() {
		if(anomalyItemDrop == null) {
			anomalyItemDrop = (GameObject) Resources.Load("ItemDrops/AnomalyItemDrop");
		}
		return anomalyItemDrop;
	}

	public static GameObject GetItemDrop(GameObject item) {
		GameObject temp = null;
		switch(item.GetComponent<Item>().RarityVal) {
			case Rarity.Common:
				temp = GetCommonItemDrop();
				break;
			case Rarity.Uncommon:
				temp = GetUncommonItemDrop();
				break;
			case Rarity.Rare:
				temp = GetRareItemDrop();
				break;
			case Rarity.Anomaly:
				temp = GetAnomalyItemDrop();
				break;
		}
		temp.GetComponent<ItemDropObject>().item = item;
		return temp;
	}

}
