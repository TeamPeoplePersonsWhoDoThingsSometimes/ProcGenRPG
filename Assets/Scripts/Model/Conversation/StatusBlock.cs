using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusBlock {

	private string name;
	private List<SpawnCommand> blockCommands;
	private List<StatusCheckable> statuses;
	
	private void init(string name) {
		this.name = name;
		blockCommands = new List<SpawnCommand> ();
		statuses = new List<StatusCheckable> ();
	}
	
	public StatusBlock() {
		init("");
	}
	
	public StatusBlock(string name) {
		init(name);
	}
	
	public StatusBlock(StatusBlockProtocol proto) {
		init(proto.Name);

		foreach(SpawnCommandProtocol s in proto.CommandsList) {
			blockCommands.Add(new SpawnCommand(s));
		}

		StatusCheckableFactory factory = new StatusCheckableFactory ();
		foreach (StatusCheckableProtocol s in proto.StatusesList) {
			statuses.Add (factory.getStatusCheckableFromProtocol(s));
		}
	}

	public bool StatusesMet() {
		bool forreturn = true;
		foreach(StatusCheckable sc in getStatuses()) {
			if(!sc.isStatusMet()) {
				forreturn = false;
			}
		}
		return forreturn;
	}
	
	public void newCommand() {
		blockCommands.Add(new SpawnCommand());
	}
	
	public void removeCommand(SpawnCommand command) {
		blockCommands.Remove(command);
	}
	
	public List<SpawnCommand> getCommands() {
		return blockCommands;
	}

	public List<StatusCheckable> getStatuses() {
		return statuses;
	}

	public override string ToString() {
		return name;
	}
}
