using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadResources : MonoBehaviour {

	public static string ConversationFile = "./Assets/Resources/out.conv";

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

	[HideInInspector]
	public List<uConversation> Conversations;

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

		//Runtime load operations
		//Note: Quests are loaded in the questListener constructor
		loadConversations ();
    }

	private void loadConversations() {
		//first read the package from the file, then unwrap it
		System.IO.FileStream fs = new System.IO.FileStream (ConversationFile, System.IO.FileMode.Open);
		ConversationPackage package = ConversationPackage.ParseFrom (fs);
		
		List<Conversation> conversationProtocols = new List<Conversation>();
		conversationProtocols.AddRange(package.ConversationsList);
		Conversations = new List<uConversation> ();
		foreach (Conversation c in conversationProtocols) {
			Conversations.Add(new uConversation(c));
		}
	}
	
}
