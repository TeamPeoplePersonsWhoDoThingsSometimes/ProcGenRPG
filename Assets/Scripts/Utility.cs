using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {


	public static string ByteToString(int bytes) {
		if(bytes > (1000*1000*500)) {
			return ((float)bytes/1000000000f).ToString("F2") + "gb";
		}

		if(bytes > (1000*500)) {
			return ((float)bytes/1000000f).ToString("F2") + "mb";
		}

		return ((float)bytes/1000f).ToString("F2") + "kb";
	}

}
