using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using conversationNode;

namespace conversation {
	public class Conversation {

		private List<conversationNode.ConversationNode> nodeList;
		private int nodes;
		private string name;
		private string creator;

		public Conversation() {
			nodes = 0;
			name = "";
			creator = "";
			nodeList = new List<conversationNode.ConversationNode>();
		}

		public Conversation(string creatorName) {
			nodes = 0;
			name = "New Conversation";
			creator = creatorName;
			nodeList = new List<conversationNode.ConversationNode>();
		}

		public List<conversationNode.ConversationNode> getNodeList() {
			return nodeList;
		}

		public void addNode(conversationNode.ConversationNode node) {
			foreach(conversationNode.ConversationNode c in nodeList) {
				foreach(long uid in c.getAlternatives().Keys) {
					conversationNode.ConversationNode.Alternative a = null;
			        c.getAlternatives().TryGetValue(uid, out a);
					if(a.getUID() == node.getUID()) {
						a.setTarget(node);
					}
				}
				
				foreach(long uid in node.getAlternatives().Keys) {
					conversationNode.ConversationNode.Alternative a = null;
				    node.getAlternatives().TryGetValue(uid, out a);
					if(a.getUID() == c.getUID()) {
						a.setTarget(c);
					}
				}
			}
			
			nodeList.Add(node);
			nodes++;
		}

		public void removeNode(conversationNode.ConversationNode node) {
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
}