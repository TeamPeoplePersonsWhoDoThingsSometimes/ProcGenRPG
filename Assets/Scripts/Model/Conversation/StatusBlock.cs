using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusBlock {

	private string name;
	private List<SpawnCommand> blockCommands;
	
	private void init(string name) {
		this.name = name;
		blockCommands = new List<SpawnCommand>();
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

	/*
	public StatusBlockProtocol getStatusAsProtobuf() {
		StatusBlockProtocol.Builder builder = StatusBlockProtocol.newBuilder();
		
		builder.SetName(name);
		
		foreach(SpawnCommand c in blockCommands) {
			builder.addCommands(c.getSpawnCommandAsProto());
		}
		
		return builder.build();
	}
	*/
	
	public override string ToString() {
		return name;
	}
}
