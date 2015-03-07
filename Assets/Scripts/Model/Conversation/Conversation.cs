using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uConversation {

	private List<uConversationNode> nodeList;
	private int nodes;
	private string name;
	private string creator;

	public uConversation() {
		nodes = 0;
		name = "";
		creator = "";
		nodeList = new List<uConversationNode>();
	}

	public uConversation(string creatorName) {
		nodes = 0;
		name = "New Conversation";
		creator = creatorName;
		nodeList = new List<uConversationNode>();
	}

	public uConversation(Conversation proto) {
		name = proto.Name;
		nodes = 0;
		creator = proto.Creator;
		nodeList = new List<uConversationNode> ();

		IList<ConversationNode> nodeProtos = proto.AllNodesList;
		foreach (ConversationNode p in nodeProtos) {
			nodeList.Add(new uConversationNode(p));
		}
	}

	public List<uConversationNode> getNodeList() {
		return nodeList;
	}

	public void addNode(uConversationNode node) {
		foreach (uConversationNode c in nodeList) {
			foreach(long uid in c.getAlternatives().Keys) {
				uConversationNode.Alternative a = null;
		        c.getAlternatives().TryGetValue(uid, out a);
				if(a.getUID() == node.getUID()) {
					a.setTarget(node);
				}
			}
			
			foreach(long uid in node.getAlternatives().Keys) {
				uConversationNode.Alternative a = null;
			    node.getAlternatives().TryGetValue(uid, out a);
				if(a.getUID() == c.getUID()) {
					a.setTarget(c);
				}
			}
		}
		
		nodeList.Add(node);
		nodes++;
	}

	public void removeNode(uConversationNode node) {
		nodeList.Remove(node);
		nodes--;
	}
	
	public int getNodesProperty() {
		return nodes;
	}
	
	public string getNameProperty() {
		return name;
	}
	
	public string getCreatorProperty() {
		return creator;
	}
	
	public int getAmountOfNodes() {
		return nodes;
	}
	
	public string getName() {
		return name;
	}
	
	public string getCreator() {
		return creator;
	}
	
	public void setName(string n) {
		name = n;
	}
}
