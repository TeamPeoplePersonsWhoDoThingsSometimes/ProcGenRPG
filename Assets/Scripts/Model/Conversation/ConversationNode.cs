using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace conversationNode {
	public class ConversationNode {

		public class Alternative {
			string text;
			private ConversationNode target;
			private long uid;//the target uid, needed during instantiation when target is not yet built
			Dictionary<List<StatusCheckable>, string> onAlternativeEvents;
			
			public Alternative(long uid) {
				this.uid = uid;
				text = "" + uid;
			}
			
			public string getText() {
				return text;
			}
			
			public void setText(string message) {
				text = message;
			}
			
			public ConversationNode getNode() {
				return target;
			}
			
			public long getUID() {
				return uid;
			}
			
			public void setTarget(ConversationNode node) {
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
		private static Dictionary<long, ConversationNode> idMap = new Dictionary<long, ConversationNode>();
		public static Dictionary<string, ConversationNode> strIdMap = new Dictionary<string, ConversationNode>();

		private static long getNextUID() {
			return currentUID++;
		}
		
		public static ConversationNode getNodeByID(long id) {
			ConversationNode node = null;
			idMap.TryGetValue(id, out node);
			return node;
		}
		
		public static ConversationNode getNodeByStringID(string id) {
			ConversationNode node = null;
			strIdMap.TryGetValue(id, out node);
			return node;
		}
		
		public static Dictionary<string, ConversationNode>.KeyCollection getAllStringIDS() {
			return strIdMap.Keys;
		}
		
		private List<StatusBlock> blocks; 
		private Dictionary<long, Alternative> alternatives;
		private long uid;
		private string ID;//non-proto field, used for ui-sub for uid
		private string text;
		private int X, Y;//for ui moveable node saving
		
		
		public ConversationNode() {
			text = "";
			uid = getNextUID();
			ID = "" + uid;
			alternatives = new Dictionary<long, Alternative>();
			blocks = new List<StatusBlock>();
		}
		
		public ConversationNode(string message) {
			alternatives = new Dictionary<long, Alternative>();
			text = message;
			uid = getNextUID();
			ID = "" + uid;
			blocks = new List<StatusBlock>();
		}
		/*
		public ConversationNode(ConversationProtobuf.ConversationNode proto) {
			text = new SimpleStringProperty();
			ID = new SimpleStringProperty();
			text.set(proto.getText());
			alternatives = new HashMap<>();
			blocks = FXCollections.observableArrayList();
			uid = proto.getUid();
			X = proto.getX();
			Y = proto.getY();
			
			for(ConversationProtobuf.Connection c : proto.getConnectionsList()) {
				Alternative alt = new Alternative(c.getNodeId());
				alt.text = c.getText();
				alternatives.put(alt.getUID(), alt);
			}
			
			for(StatusBlockProtocol s : proto.getBlocksList()) {
				blocks.add(new StatusBlock(s));
			}
		}
		*/
		
		public string textProperty() {
			return text;
		}
		
		public string getText() {
			return text;
		}
		
		public void setText(string t) {
			text = t;
		}
		
		public string IDProperty() {
			return ID;
		}
		
		public string getID() {
			return ID;
		}
		
		/**
     * Checks to see if the id exists yet, and sets the id for this node if it does not
     * @param id id to set
     * @return true on successful set
     */
		public bool setID(string id) {
			if (strIdMap.ContainsKey(id) && !ID.Equals(id))
				return false;
			ID = id;
			return true;
		}
		
		public long getUID() {
			return uid;
		}
		
		public int getX() {
			return X;
		}
		
		public void setX(int newX) {
			X = newX;
		}
		
		public int getY() {
			return Y;
		}
		
		public void setY(int newY) {
			Y = newY;
		}
		
		public void newStatusBlock() {
			blocks.Add(new StatusBlock("Block " + (blocks.Count + 1)));
		}
		
		public void newStatusBlock(string blockName) {
			blocks.Add(new StatusBlock(blockName));
		}
		
		public void removeStatusBlock(StatusBlock block) {
			blocks.Remove(block);
		}
		
		public List<StatusBlock> getBlocks() {
			return blocks;
		}
		
		public Alternative newAlternative(ConversationNode target) {
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
}