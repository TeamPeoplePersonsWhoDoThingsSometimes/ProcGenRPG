using UnityEngine;
using System.Collections;

public class MasterDriver : MonoBehaviour {

    Map currentMap;
    Area currentArea;


	// Use this for initialization
	void Start ()
    {

        currentMap = new Map(26574);

        currentMap.debugDisplayMap();

        Debug.Log("Startup time: " + Time.realtimeSinceStartup);
	}
	

    //TODO: Create startNewGame and loadGame methods.
}
