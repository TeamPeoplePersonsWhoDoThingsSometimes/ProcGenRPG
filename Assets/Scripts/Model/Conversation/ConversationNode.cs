using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uConversationNode {

	public class Alternative {
		string text;
		private uConversationNode target;
		private long uid;//the target uid, needed during instantiation when target is not yet built
		Dictionary<List<StatusCheckable>, string> onAlternativeEvents;
		
		public Alternative(long uid) {
			this.uid = uid;
			text = "";
		}

		public Alternative(long uid, string t) {
			this.uid = uid;
			text = t;
		}
		
		public string getText() {
			return text;
		}
		
		public void setText(string message) {
			text = message;
		}
		
		public uConversationNode getNode() {
			return target;
		}
		
		public long getUID() {
			return uid;
		}
		
		public void setTarget(uConversationNode node) {
			uid = node.uid;
			target = node;
		}

		public override bool Equals(System.Object other) {
			if (other is Alternative) {
				Alternative alt = (Alternative)other;
				return alt.text.Equals(text);
			} else if (other is string) {
				string str = (string)other;
				return str.Equals(text);
			}
			return false;
		}
		
		public override int GetHashCode() {
			int hash = 5;
			hash = 53 * hash + this.text.GetHashCode();
			return hash;
		}
	}

	private static long currentUID = 0;
	private static Dictionary<long, uConversationNode> idMap = new Dictionary<long, uConversationNode>();
	public static Dictionary<string, uConversationNode> strIdMap = new Dictionary<string, uConversationNode>();

	private static long getNextUID() {
		return currentUID++;
	}
	
	public static uConversationNode getNodeByID(long id) {
		uConversationNode node = null;
		idMap.TryGetValue(id, out node);
		return node;
	}
	
	public static uConversationNode getNodeByStringID(string id) {
		uConversationNode node = null;
		strIdMap.TryGetValue(id, out node);
		return node;
	}
	
	public static Dictionary<string, uConversationNode>.KeyCollection getAllStringIDS() {
		return strIdMap.Keys;
	}
	
	private List<StatusBlock> blocks; 
	private Dictionary<long, Alternative> alternatives;
	private long uid;
	private string text;
	
	
	public uConversationNode() {
		text = "";
		uid = getNextUID();
		alternatives = new Dictionary<long, Alternative>();
		blocks = new List<StatusBlock>();
	}
	
	public uConversationNode(string message) {
		alternatives = new Dictionary<long, Alternative>();
		text = message;
		uid = getNextUID();
		blocks = new List<StatusBlock>();
	}

	public uConversationNode(ConversationNode proto) {
		text = "";
		alternatives = new Dictionary<long, Alternative>();
		blocks = new List<StatusBlock>();
		uid = proto.Uid;

		foreach (Connection c in proto.ConnectionsList) {
			Alternative alt = new Alternative(c.NodeId, c.Text);
			alternatives.Add(alt.getUID(), alt);
		}
		
		foreach (StatusBlockProtocol s in proto.BlocksList) {
			blocks.Add(new StatusBlock(s));
		}
	}
	
	public string textProperty() {
		return text;
	}
	
	public string getText() {
		return text;
	}
	
	public void setText(string t) {
		text = t;
	}
	
	public long getUID() {
		return uid;
	}
	
	public void newStatusBlock(string blockName) {
		blocks.Add(new StatusBlock(blockName));
	}
	
	public List<StatusBlock> getBlocks() {
		return blocks;
	}
	
	public Alternative newAlternative(uConversationNode target) {
		Alternative alt = new Alternative(target.getUID());
		alt.setTarget(target);
		alternatives.Add(alt.getUID(), alt);
		return alt;
	}
	
	/**
     * Add an alternative option to this node such that the string choice
     * maps to the Conversation node target
     * @param choice the option that selects this alternative
     * @param target the destination node
     */
	public void addAlternative(Alternative alt) {
		alternatives.Add(alt.getUID(), alt);
	}
	
	/**
     * Gets all the nodes that may be reached by alternatives from this node
     * @return all of this node's alternative targets
     */
	public Dictionary<long, Alternative> getAlternatives() {
		return alternatives;
	}
	
	/**
     * Remove the alternative associated with the given choice
     * @param choice the choice associated with the alternative to remove
     */
	public void removeAlternative(long uid) {
		if(uid < 0) {
			alternatives.Clear();
			return;
		}
		
		if(alternatives.ContainsKey(uid)) {
			alternatives.Remove(uid);
		}
	}
}
