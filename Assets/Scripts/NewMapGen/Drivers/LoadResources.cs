using UnityEngine;
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

	public GameObject CommonItemDrop;
	public GameObject UncommonItemDrop;
	public GameObject RareItemDrop;
	public GameObject Chest;

    public Tile portal;

    public static LoadResources Instance;

    void Awake()
    {
        // First, check if there are any other instances conflicting.
        if (Instance != null && Instance != this)
        {
            // If so, destroy other instances.
            Destroy(this.gameObject);
        }

        //Save our singleton instance.
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

    }
	
}
