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

	public StatusBlockSave getStatusBlockSave() {
		StatusBlockSave.Builder builder = StatusBlockSave.CreateBuilder ();

		foreach (StatusCheckable s in statuses) {
			StatusSave.Builder sBuilder = StatusSave.CreateBuilder();
			sBuilder.SetAlreadyMet (s.isStatusMet());
			s.setBuilderWithData(ref sBuilder);
			builder.AddStats(sBuilder.Build ());
		}

		return builder.Build ();
	}

	public void setStatusBlockFromSave(StatusBlockSave saveData) {
		for (int i = 0; i < statuses.Count; i++) {
			statuses[i].setFromData(saveData.StatsList[i]);
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
