using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	private static GameObject hitInfo, byteObject, commonItemDrop, uncommonItemDrop, rareItemDrop;

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
		return ((int.Parse(version.Split('.')[0]))*100 + (int.Parse(version.Split('.')[1]))*10 + (int.Parse(version.Split('.')[2])));
	}

	public static string IntToVersion(int version) {
		string versionString = version.ToString();
		string forreturn = versionString.Substring(0, 1) + "." + versionString.Substring(1, 1) + "." + versionString.Substring(2, 1);
		return forreturn;
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

}
