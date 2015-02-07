using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace conversation {
	public class Conversation {

		private List<ConversationNode> nodeList;
		private int nodes;
		private string name;
		private string creator;

		public Conversation() {
			nodes = 0;
			name = "";
			creator = "";
			nodeList = new List<ConversationNode>();
		}

		public Conversation(string creatorName) {
			nodes = 0;
			name = "New Conversation";
			creator = creatorName;
			nodeList = new List<ConversationNode>();
		}
	}
}