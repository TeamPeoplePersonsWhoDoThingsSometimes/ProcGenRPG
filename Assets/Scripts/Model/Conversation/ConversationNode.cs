using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uConversationNode {

	public class Alternative {
		string text;
		private uConversationNode target;
		private long uid;//the target uid, needed during instantiation when target is not yet built
		List<List<StatusCheckable>> alternativeRequirment;
		private int priority;
		
		public Alternative(long uid) {
			this.uid = uid;
			text = "";
			alternativeRequirment = new List<List<StatusCheckable>>();
		}
		
		public Alternative(long uid, string t) {
			this.uid = uid;
			text = t;
			alternativeRequirment = new List<List<StatusCheckable>>();
		}
		
		public Alternative(long uid, string t, List<List<StatusCheckable>> reqs) {
			this.uid = uid;
			text = t;
			alternativeRequirment = reqs;
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

		public int getPriority() {
			return priority;
		}

		public void setPriority(int p) {
			priority = p;
		}

		public bool isValidAlternative() {
			Debug.Log("Checking Alternative: " + this.text + " " + alternativeRequirment.Count);
			if(alternativeRequirment.Count == 0) {
				return true;
			}

			foreach (List<StatusCheckable> block in alternativeRequirment) {

				bool passed = true;
				foreach (StatusCheckable status in block) {
					if (!status.isStatusMet()) {
						passed = false;
					}
				}

				if (passed) {
					return true;
				}
			}

			Debug.Log("Checking Alternative: " + this.text + " is FALSE");

			return false;

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
	private string ID;

	private void cacheIds() {
		idMap.Add (uid, this);
		strIdMap.Add (ID, this);
	}
	
	public uConversationNode() {
		text = "";
		uid = getNextUID();
		ID = "" + uid;
		alternatives = new Dictionary<long, Alternative>();
		blocks = new List<StatusBlock>();
		cacheIds ();
	}
	
	public uConversationNode(string message) {
		alternatives = new Dictionary<long, Alternative>();
		text = message;
		uid = getNextUID();
		ID = "" + uid;
		blocks = new List<StatusBlock>();
		cacheIds ();
	}

	public uConversationNode(ConversationNode proto) {
		alternatives = new Dictionary<long, Alternative>();
		blocks = new List<StatusBlock>();
		uid = proto.Uid;
		if (proto.Name != "null") {
			ID = proto.Name;
		} else {
			ID = "" + uid;
		}

		if (proto.Text != "null") {
			text = proto.Text;
		} else {
			text = "";
		}

		foreach (Connection c in proto.ConnectionsList) {
			List<List<StatusCheckable>> reqs = new List<List<StatusCheckable>>();

			foreach (RequirementSet block in c.RequirementSetsList) {
				StatusCheckableFactory factory = new StatusCheckableFactory();
				List<StatusCheckable> checks = new List<StatusCheckable>();
				foreach (StatusCheckableProtocol p in block.RequirementsList) {
					checks.Add(factory.getStatusCheckableFromProtocol(p));
				}
				reqs.Add(checks);
			}

			Alternative alt = new Alternative(c.NodeId, c.Text, reqs);

			if (c.HasPriority) {
				alt.setPriority(c.Priority);
			} else {
				alt.setPriority(0);
			}
			alternatives.Add(alt.getUID(), alt);
		}
		
		foreach (StatusBlockProtocol s in proto.BlocksList) {
			blocks.Add(new StatusBlock(s));
		}

		cacheIds ();
	}

	public List<string> getAlternativeStrings() {
		List<string> forreturn = new List<string>();
		foreach(Alternative a in getAlternatives().Values) {
			if(!forreturn.Contains(a.getText()) && a.isValidAlternative()) {
				forreturn.Add(a.getText());
			}
		}
		return forreturn;
	}

	public uConversationNode GoToAlternative(string s) {
		uConversationNode forreturn = null;
//		foreach(Alternative a in getAlternatives().Values) {
//			uConversationNode tempNode = uConversationNode.getNodeByID(a.getUID());
//			Debug.Log(a.getText() + " -> " + tempNode.getText());
//			if(((forreturn != null && forreturn.getPrioritizedAlternative < tempNode.blocks.Count) || (forreturn == null)) && s.Equals(a.getText()) && tempNode.ABlockSatisfied()) {
//			if(s.Equals(a.getText())) {
//				Alternative a = getPrioritizedAlternative();
//				forreturn = tempNode;
//			}
//		}
		return getNodeByID((getPrioritizedAlternative(s).getUID()));
	}

	public bool ABlockSatisfied() {
		Debug.Log("ABOUT TO CHECK BLOCKS: " + getBlocks().Count);
		foreach(StatusBlock sb in getBlocks()) {
			if(sb.StatusesMet()) {
				Debug.Log("HERE" + this.text);
				return true;
			}
		}
		if(getBlocks().Count == 0) {
			return true;
		}
		return false;
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
	 * Gets the highest valid priority alternative
	 */
	public Alternative getPrioritizedAlternative(string text) {
		int highPriority = 0;
		Alternative highAlternative = null;
//		Debug.Log("HUR: " + text);
		foreach (KeyValuePair<long, Alternative> e in alternatives) {
			Alternative a = e.Value;
//			Debug.Log("Checking " + e.Value.getText() + " " + a.isValidAlternative());
			if (a.getText().Equals(text) && a.isValidAlternative()) {
				if (highAlternative == null || highPriority == 0 || a.getPriority() < highPriority) {
					highPriority = a.getPriority();
					highAlternative = a;
				}
			}
		}

		return highAlternative;
	}

	public DirectObject getDirectObject() {
		return new DirectObject(this.ID, this.ID);
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
