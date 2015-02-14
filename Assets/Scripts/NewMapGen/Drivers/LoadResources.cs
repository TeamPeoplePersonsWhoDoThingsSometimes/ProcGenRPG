﻿using UnityEngine;
using System.Collections;

public class LoadResources : MonoBehaviour {

    public Sprite twoWay;
    public Sprite threeWay;
    public Sprite fourWay;
    public Sprite end;
    public Sprite corner;

    public GameObject spriteHolder;

    public GameObject grassyPath;
    public GameObject dungeon;
    public GameObject city;

    public static LoadResources instance;

	// Use this for initialization
	void OnEnable () {

        //UNSAFE, TEMPORARY SINGLETON
	    instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
